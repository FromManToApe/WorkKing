using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

// Controller
public class Slicer2DController : MonoBehaviour {
	public enum SliceType {Linear, LinearCut, Complex, ComplexTracked, Point, Polygon, Explode, Create};
	public enum SliceRotation {Random, Vertical, Horizontal}
	public enum CreateType {Slice, PolygonType}

	public bool addForce = true;
	public float addForceAmount = 5f;

	public bool sliceIfPossible = false;

	[Tooltip("Slice type represents algorithm complexity")]
	public SliceType sliceType = SliceType.Complex;
	public Slice2DLayer sliceLayer = Slice2DLayer.Create();

	public Polygon2D slicePolygon = Polygon2D.Create (Polygon2D.PolygonType.Pentagon);

	[Tooltip("Minimum distance between points (SliceType: Complex")]
	private float minVertsDistance = 1f;

	// Polygon Destroyer type settings
	public Polygon2D.PolygonType polygonType = Polygon2D.PolygonType.Circle;
	public float polygonSize = 1;
	public bool polygonDestroy = false;

	// Polygon Creator
	public Material material;
	public CreateType createType = CreateType.Slice;

	// Complex Slicer
	public Slicer2D.SliceType complexSliceType = Slicer2D.SliceType.SliceHole;

	// Linear Cut Slicer
	public float linearCutSize = 0.5f;

	// Slicer Visuals
	public bool drawSlicer = true;
	public float lineWidth = 1.0f;
	public float zPosition = 0f;
	public Color slicerColor = Color.black;

	// Point Slicer
	public SliceRotation sliceRotation = SliceRotation.Random;

	// Events Input Handler
	public static List<Vector2f> complexSlicerPointsList = new List<Vector2f>();
	public static Pair2f linearPair = Pair2f.Zero();
	public static LinearCut linearCutLine = new LinearCut();
	public static ComplexSlicerTracker complexTracker = new ComplexSlicerTracker();

	public static Slicer2DController instance;
	private bool mouseDown = false;

	public static Color[] slicerColors = {Color.black, Color.green, Color.yellow , Color.red, new Color(1f, 0.25f, 0.125f)};

	public delegate void Slice2DResultEvent(Slice2D slice);
	private event Slice2DResultEvent sliceResultEvent;
	public void AddResultEvent(Slice2DResultEvent e) { sliceResultEvent += e; }

	public void Awake()
	{
		instance = this;
	}

	public static Vector2f GetMousePosition()
	{
		return(new Vector2f (Camera.main.ScreenToWorldPoint (Input.mousePosition)));
	}

	public void SetSliceType(int type)
	{
		sliceType = (SliceType)type;
	}

	public void SetLayerType(int type)
	{
		if (type == 0) {
			sliceLayer.SetLayerType((Slice2DLayer.Type)0);
		} else {
			sliceLayer.SetLayerType((Slice2DLayer.Type)1);
			sliceLayer.DisableLayers ();
			sliceLayer.SetLayer (type - 1, true);
		}
	}

	public void SetSlicerColor(int colorInt)
	{
		slicerColor = slicerColors [colorInt];
	}

	public void OnRenderObject() {
		Vector2f pos = GetMousePosition ();

		if (drawSlicer == false)
			return;

		Max2D.SetBorder (true);
		Max2D.SetLineMode(Max2D.LineMode.Smooth);
		Max2D.SetLineWidth (lineWidth * .5f);

		if (mouseDown) {
			Max2D.SetColor (slicerColor);

			switch (sliceType) {
				case SliceType.Complex:
					if (complexSlicerPointsList.Count > 0) {
						Max2D.DrawStrippedLine (complexSlicerPointsList, minVertsDistance, zPosition);
						Max2D.DrawLineSquare (complexSlicerPointsList.Last(), 0.5f, zPosition);
						Max2D.DrawLineSquare (complexSlicerPointsList.First (), 0.5f, zPosition);
					}
					break;

				case SliceType.ComplexTracked:
					if (complexSlicerPointsList.Count > 0) {
						Max2D.DrawLineSquare (pos, 0.5f, zPosition);

						foreach(ComplexSlicerTrackerObject tracker in complexTracker.trackerList) {
							if (tracker.slicer != null && tracker.tracking) {
								Max2D.DrawSlice(VectorList2f.ToWorldSpace(tracker.slicer.transform, tracker.pointsList), tracker.slicer.transform.position.z - 0.001f);
							}
						}
					}
					break;

				case SliceType.Create:
					if (createType == CreateType.Slice) {
						if (complexSlicerPointsList.Count > 0) {
							Max2D.DrawStrippedLine (complexSlicerPointsList, minVertsDistance, zPosition, true);
							Max2D.DrawLineSquare (complexSlicerPointsList.Last(), 0.5f, zPosition);
							Max2D.DrawLineSquare (complexSlicerPointsList.First (), 0.5f, zPosition);
						}
					} else {
						Max2D.DrawStrippedLine (Polygon2D.Create(polygonType, polygonSize).pointsList, minVertsDistance, zPosition, true, pos);
					}
					break;
				
				case SliceType.Linear:
					Max2D.DrawLine (linearPair.A, linearPair.B, zPosition);
					Max2D.DrawLineSquare (linearPair.A, 0.5f, zPosition);
					Max2D.DrawLineSquare (linearPair.B, 0.5f, zPosition);
					break;

				case SliceType.LinearCut:
					linearCutLine = LinearCut.Create(linearPair, linearCutSize);
					Max2D.DrawStrippedLine (linearCutLine.GetPointsList(), 0, zPosition, true);

					break;

				case SliceType.Point:
					break;

				case SliceType.Explode:
					break;

				case SliceType.Polygon:
					slicePolygon = Polygon2D.Create (polygonType, polygonSize);
					Max2D.DrawStrippedLine (slicePolygon.pointsList, minVertsDistance, zPosition, false, pos);
					break;
				
				default:
					break; 
			}
		}
	}

	public void LateUpdate()
	{
		Vector2f pos = GetMousePosition ();

		switch (sliceType) {	
			case SliceType.Linear:
				UpdateLinear (pos);
				break;

			case SliceType.LinearCut:
				
				UpdateLinearCut (pos);
				
				break;

			case SliceType.Complex:
				UpdateComplex (pos);
				break;

			case SliceType.ComplexTracked:
				UpdateComplexTracked(pos);
				break;

			case SliceType.Point:
				UpdatePoint (pos);
				break;

			case SliceType.Explode:			
				UpdateExplode (pos);
				break;

			case SliceType.Create:
				UpdateCreate (pos);
				break;

			case SliceType.Polygon:
				UpdatePolygon (pos);
				break;

			default:
				break; 
		}
	}
		
	private void UpdateLinear(Vector2f pos)
	{
		if (Input.GetMouseButtonDown (0)) {
			linearPair.A.Set (pos);
			mouseDown = true;
		}

		if (mouseDown && Input.GetMouseButton (0)) {
			linearPair.B.Set (pos);
		
			if (sliceIfPossible) {
				if (LinearSlice (linearPair)) {
					mouseDown = false;
					linearPair.A.Set (pos);
				}
			}
		}

		if (mouseDown == true && Input.GetMouseButton (0) == false) {
			mouseDown = false;
			LinearSlice (linearPair);
		}
	}

	private void UpdateLinearCut(Vector2f pos)
	{
		if (Input.GetMouseButtonDown (0)) 
			linearPair.A.Set (pos);

		if (Input.GetMouseButton (0)) {
			linearPair.B.Set (pos);
			mouseDown = true;
		}

		if (mouseDown == true && Input.GetMouseButton (0) == false) {
			mouseDown = false;
			Slicer2D.LinearCutSliceAll (linearCutLine, sliceLayer);
		}
	}

	private void UpdateComplex(Vector2f pos)
	{
		if (Input.GetMouseButtonDown (0)) {
			complexSlicerPointsList.Clear ();
			complexSlicerPointsList.Add (pos);
			mouseDown = true;
		}

		if (complexSlicerPointsList.Count < 1) {
			return;
		}

		if (Input.GetMouseButton (0)) {
			Vector2f posMove = new Vector2f (complexSlicerPointsList.Last ());
			bool added = false;
			while ((Vector2f.Distance (posMove, pos) > minVertsDistance)) {
				float direction = Vector2f.Atan2 (pos, posMove);
				posMove.Push (direction, minVertsDistance);
				complexSlicerPointsList.Add (new Vector2f (posMove));
				added = true;
			}

			if (sliceIfPossible == true && added) {
				if (ComplexSlice (complexSlicerPointsList) == true) {
					mouseDown = false;
					complexSlicerPointsList.Clear ();
				}
			}
		}

		if (mouseDown == true && Input.GetMouseButton (0) == false) {
			mouseDown = false;
			Slicer2D.complexSliceType = complexSliceType;
			ComplexSlice (complexSlicerPointsList);
			complexSlicerPointsList.Clear ();
		}
	}

	private void UpdateComplexTracked(Vector2f pos)
	{
		if (Input.GetMouseButtonDown (0)) {
			complexSlicerPointsList.Clear ();
			complexTracker.trackerList.Clear ();
			complexSlicerPointsList.Add (pos);
		}
						
		if (Input.GetMouseButton (0) && complexSlicerPointsList.Count > 0) {
			Vector2f posMove = new Vector2f (complexSlicerPointsList.Last ());

			while ((Vector2f.Distance (posMove, pos) > minVertsDistance)) {
				float direction = Vector2f.Atan2 (pos, posMove);
				posMove.Push (direction, minVertsDistance);
				Slicer2D.complexSliceType = complexSliceType;
				complexSlicerPointsList.Add (new Vector2f (posMove));
				complexTracker.Update(posMove.vector, 0);
			}

			mouseDown = true;
			
			complexTracker.Update(posMove.vector, minVertsDistance);

		} else {
			mouseDown = false;
		}
	}

	private void UpdatePoint(Vector2f pos)
	{
		if (Input.GetMouseButtonDown (0)) 
			PointSlice(pos);
	}

	private void UpdatePolygon(Vector2f pos)
	{
		mouseDown = true;

		if (Input.GetMouseButtonDown (0))
			PolygonSlice (pos);
	}

	private void UpdateExplode(Vector2f pos)
	{
		if (Input.GetMouseButtonDown (0))
			ExplodingSlice(pos);
	}

	private void UpdateCreate(Vector2f pos)
	{
		if (Input.GetMouseButtonDown (0)) {
			complexSlicerPointsList.Clear ();
			complexSlicerPointsList.Add (pos);
		}

		if (createType == CreateType.Slice) {
			if (Input.GetMouseButton (0)) {
				if (complexSlicerPointsList.Count == 0 || (Vector2f.Distance (pos, complexSlicerPointsList.Last ()) > minVertsDistance))
					complexSlicerPointsList.Add (pos);

				mouseDown = true;
			}

			if (mouseDown == true && Input.GetMouseButton (0) == false) {
				mouseDown = false;
				CreatorSlice (complexSlicerPointsList);
			}
		} else {
			mouseDown = true;
			if (Input.GetMouseButtonDown (0))
				PolygonCreator (pos);
		}
	}

	private bool LinearSlice(Pair2f slice)
	{
		List<Slice2D> results = Slicer2D.LinearSliceAll (slice, sliceLayer);
		bool result = false;

		foreach (Slice2D id in results) 
			if (id.gameObjects.Count > 0)
				result = true;

		if (addForce == true) {
			float sliceRotation = Vector2f.Atan2 (slice.B, slice.A);

			foreach (Slice2D id in results) {
				foreach (GameObject gameObject in id.gameObjects) {
					Rigidbody2D rigidBody2D = gameObject.GetComponent<Rigidbody2D> ();
					if (rigidBody2D)
						foreach (Vector2f p in id.collisions) {
							Vector2 force = new Vector2 (Mathf.Cos (sliceRotation) * addForceAmount, Mathf.Sin (sliceRotation) * addForceAmount);
							Physics2DHelper.AddForceAtPosition(rigidBody2D, force, p.vector);
						}
				}
				if (sliceResultEvent != null) {
					sliceResultEvent(id);
				}
			}
		}

		return(result);
	}

	private bool ComplexSlice(List <Vector2f> slice)
	{
		List<Slice2D> results = Slicer2D.ComplexSliceAll (slice, sliceLayer);
		bool result = false;

		foreach (Slice2D id in results) 
			if (id.gameObjects.Count > 0)
				result = true;

		if (addForce == true)
			foreach (Slice2D id in results) {
				foreach (GameObject gameObject in id.gameObjects) {
					Rigidbody2D rigidBody2D = gameObject.GetComponent<Rigidbody2D> ();
					if (rigidBody2D) {
						List<Pair2f> list = Pair2f.GetList (id.collisions);
						float forceVal = 1.0f / list.Count;
						foreach (Pair2f p in list) {
							float sliceRotation = -Vector2f.Atan2 (p.B, p.A);
							Vector2 force = new Vector2 (Mathf.Cos (sliceRotation) * addForceAmount, Mathf.Sin (sliceRotation) * addForceAmount);
							Physics2DHelper.AddForceAtPosition(rigidBody2D, forceVal * force, (p.A.vector + p.B.vector) / 2f);
						}
					}
				}
				if (sliceResultEvent != null) {
					sliceResultEvent(id);
				}
			}
		return(result);
	}

	private void PointSlice(Vector2f pos)
	{
		float rotation = 0;

		switch (sliceRotation) {	
			case SliceRotation.Random:
				rotation = UnityEngine.Random.Range (0, Mathf.PI * 2);
				break;

			case SliceRotation.Vertical:
				rotation = Mathf.PI / 2f;
				break;

			case SliceRotation.Horizontal:
				rotation = Mathf.PI;
				break;
		}

		List<Slice2D> results = Slicer2D.PointSliceAll (pos, rotation, sliceLayer);
		foreach (Slice2D id in results) {
			if (sliceResultEvent != null) {
				sliceResultEvent(id);
			}
		}
	}
		
	private void PolygonSlice(Vector2f pos)
	{
		Polygon2D slicePolygonDestroy = null;
		if (polygonDestroy == true) {
			slicePolygonDestroy = Polygon2D.Create (polygonType, polygonSize * 1.1f);
		}
		Slicer2D.PolygonSliceAll(pos, Polygon2D.Create (polygonType, polygonSize), slicePolygonDestroy, sliceLayer);
	}

	private void ExplodingSlice(Vector2f pos)
	{
		List<Slice2D> results =	Slicer2D.ExplodingSliceAll (pos, sliceLayer);
		if (addForce == true)
			foreach (Slice2D id in results) {
				foreach (GameObject gameObject in id.gameObjects) {
					Rigidbody2D rigidBody2D = gameObject.GetComponent<Rigidbody2D> ();
					if (rigidBody2D) {
						float sliceRotation = Vector2f.Atan2 (pos, new Vector2f (gameObject.transform.position));
						Rect rect = Polygon2D.CreateFromCollider (gameObject).GetBounds ();
						Physics2DHelper.AddForceAtPosition(rigidBody2D, new Vector2 (Mathf.Cos (sliceRotation) * addForceAmount / 10f, Mathf.Sin (sliceRotation) * addForceAmount/ 10f), rect.center);
					}
				}
				if (sliceResultEvent != null) {
					sliceResultEvent(id);
				}
			}
	}

	private void ExplodeAll()
	{
		List<Slice2D> results =	Slicer2D.ExplodeAll (sliceLayer);
		if (addForce == true)
			foreach (Slice2D id in results) {
				foreach (GameObject gameObject in id.gameObjects) {
					Rigidbody2D rigidBody2D = gameObject.GetComponent<Rigidbody2D> ();
					if (rigidBody2D) {
						float sliceRotation = Vector2f.Atan2 (new Vector2f(0, 0), new Vector2f (gameObject.transform.position));
						Rect rect = Polygon2D.CreateFromCollider (gameObject).GetBounds ();
						Physics2DHelper.AddForceAtPosition(rigidBody2D, new Vector2 (Mathf.Cos (sliceRotation) * addForceAmount / 10f, Mathf.Sin (sliceRotation) * addForceAmount/ 10f), rect.center);
					}
				}
				if (sliceResultEvent != null) {
					sliceResultEvent(id);
				}
			}
	}

	private void CreatorSlice(List <Vector2f> slice)
	{
		Polygon2D newPolygon = Slicer2D.CreatorSlice (slice);
		if (newPolygon != null) {
			CreatePolygon (newPolygon);
		}
	}

	private void PolygonCreator(Vector2f pos)
	{
		Polygon2D newPolygon = Polygon2D.Create (polygonType, polygonSize).ToOffset (pos);
		CreatePolygon (newPolygon);
	}

	private void CreatePolygon(Polygon2D newPolygon)
	{
		GameObject newGameObject = new GameObject ();
		newGameObject.transform.parent = transform;
		newGameObject.AddComponent<Rigidbody2D> ();
		newGameObject.AddComponent<ColliderLineRenderer2D> ().color = Color.black;

		Slicer2D smartSlicer = newGameObject.AddComponent<Slicer2D> ();
		smartSlicer.textureType = Slicer2D.TextureType.Mesh;
		smartSlicer.material = material;

		newPolygon.CreateCollider (newGameObject);
		newPolygon.CreateMesh (newGameObject, new Vector2 (1, 1), Vector2.zero, PolygonTriangulator2D.Triangulation.Advanced);
	}
}