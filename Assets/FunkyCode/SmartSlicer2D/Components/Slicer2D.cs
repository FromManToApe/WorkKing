using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Slicer2D : MonoBehaviour {
	public enum SlicingLayer {Layer1 = 0, Layer2 = 1, Layer3 = 2, Layer4 = 3, Layer5 = 4, Layer6 = 5, Layer7 = 6, Layer8 = 7}; 	// Add Default Layer?
	public enum SliceType {Regular, SliceHole, FillSlicedHole};
	public enum TextureType {Sprite, Mesh, None};

	public static SliceType complexSliceType = SliceType.Regular;
	public static int explosionPieces = 15; // In Use?

	//[Tooltip("Type of texture to generate")]
	public TextureType textureType = TextureType.Sprite;

	public SlicingLayer slicingLayer = SlicingLayer.Layer1;

	private Polygon2D.ColliderType colliderType;

	public PolygonTriangulator2D.Triangulation triangulation = PolygonTriangulator2D.Triangulation.Advanced;

	public bool slicingLimit = false;
	public int sliceCounter = 0;
	public int maxSlices = 10;

	public bool recalculateMass = false;
	
	public Polygon2D polygon = new Polygon2D();

	public Material material;

	// Joint Support
	private Rigidbody2D body2D;
	private List<Joint2D> joints = new List<Joint2D>();
	
	public void RecalculateJoints() {
		if (body2D) {
			joints = Joint2D.GetJointsConnected (body2D);
		}
	}

	void SliceJointEvent(Slice2D sliceResult)
	{
		// Remove Slicer Component Duplicated From Sliced Components
		foreach (GameObject g in sliceResult.gameObjects) {
			List<Joint2D> joints = Joint2D.GetJoints(g);
			foreach(Joint2D joint in joints) {
				if (Polygon2D.CreateFromCollider (g).PointInPoly (new Vector2f (joint.anchoredJoint2D.anchor)) == false) {
					Destroy (joint.anchoredJoint2D);
				} else {
					if (joint.anchoredJoint2D != null && joint.anchoredJoint2D.connectedBody != null) {
						Slicer2D slicer2D = joint.anchoredJoint2D.connectedBody.gameObject.GetComponent<Slicer2D>();
						if (slicer2D != null) {
							slicer2D.RecalculateJoints();
						}
					}
				}
			}
		}
	
		if (body2D == null) {
			return;
		}
		
		// Reconnect Joints To Sliced Bodies
		foreach(Joint2D joint in joints) {
			if (joint.anchoredJoint2D == null) {
				continue;
			}
			foreach (GameObject g in sliceResult.gameObjects) {
				switch (joint.jointType) {
					case Joint2D.Type.HingeJoint2D:
						if (Polygon2D.CreateFromCollider (g).PointInPoly (new Vector2f (Vector2.zero))) {
							joint.anchoredJoint2D.connectedBody = g.GetComponent<Rigidbody2D> ();
						}
						break;

					default:
						if (Polygon2D.CreateFromCollider (g).PointInPoly (new Vector2f (joint.anchoredJoint2D.connectedAnchor))) {
							joint.anchoredJoint2D.connectedBody = g.GetComponent<Rigidbody2D> ();
						}	
						break;
				}
			}
		}
	}

	// Event Handling
	public delegate bool Slice2DEvent(Slice2D slice);
	public delegate void Slice2DResultEvent(Slice2D slice); // Should return Slice2D Object instead of List

	private event Slice2DEvent sliceEvent;
	private event Slice2DResultEvent sliceResultEvent;

	public void AddEvent(Slice2DEvent e) { sliceEvent += e; }
	public void AddResultEvent(Slice2DResultEvent e) { sliceResultEvent += e; }

	static private List<Slicer2D> slicer2DList = new List<Slicer2D>();

	public Polygon2D.ColliderType GetColliderType() {
		return(colliderType);
	}
		
	public int GetLayerID() { return((int)slicingLayer); }

	// Update loop enables ".enabled" component field	
	void Update() {}
	void OnEnable() { slicer2DList.Add (this);}
	void OnDisable() { slicer2DList.Remove (this);}

	static public List<Slicer2D> GetList(){
		return(new List<Slicer2D>(slicer2DList));
	}

	void Start()
	{
		Initialize ();
		RecalculateJoints();
	}

	// Check Before Each Function - Then This Could Be Private
	public void Initialize() {
		colliderType = Polygon2D.GetColliderType (gameObject);

		List<Polygon2D> result = Polygon2D.GetListFromCollider (gameObject);

		// Split collider if there are more polygons than 1
		if (result.Count > 1) {
			PerformResult(result, new Slice2D());
		}

		polygon = Polygon2D.CreateFromCollider (gameObject); // It is already generated!!!!!!!!!!!!

		body2D = GetComponent<Rigidbody2D> ();

		switch (textureType) {
			case TextureType.Mesh:
				Polygon2D.CreateFromCollider(gameObject).CreateMesh (gameObject, new Vector2 (1, 1), Vector2.zero, triangulation);
				MeshRenderer meshRenderer = GetComponent<MeshRenderer> ();
				meshRenderer.material = material;

				break;

			case TextureType.Sprite:
				if (GetComponent<SpriteRenderer> () != null) {
					gameObject.AddComponent<SpriteMesh2D> ();
				}

				break;

			default:
				break;
			}
	}
		
	public Slice2D LinearSlice(Pair2f slice) {
		Slice2D slice2D = Slice2D.Create (slice);
		if (this.isActiveAndEnabled == false) 
			return(slice2D);

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.LinearSlice (colliderPolygon, slice);
			sliceResult.AddGameObjects (PerformResult (sliceResult.polygons, slice2D));
			return(sliceResult);
		}
			
		return(slice2D);
	}
		
	public Slice2D ComplexSlice(List<Vector2f> slice) {
		Slice2D slice2D = Slice2D.Create (slice);
		if (this.isActiveAndEnabled == false)
			return(slice2D);

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.ComplexSlice (colliderPolygon, slice);
			sliceResult.AddGameObjects (PerformResult (sliceResult.polygons, slice2D));

			return(sliceResult);
		}
		
		return(slice2D);
	}

	public Slice2D LinearCutSlice(LinearCut slice) {
		Slice2D slice2D = Slice2D.Create (slice);
		if (this.isActiveAndEnabled == false)
			return(slice2D);

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {

			if (MathHelper.PolyInPoly (slice.GetPointsList(1.01f), colliderPolygon.pointsList) == true) {
				Destroy (gameObject);
				return(slice2D);

			} else {
				Slice2D sliceResult = Slicer2D.LinearCutSlice (colliderPolygon, slice);
				
				foreach(Polygon2D poly in new List<Polygon2D> (sliceResult.polygons)) {
					if (MathHelper.PolyInPoly(slice.GetPointsList(1.001f), poly.pointsList)) {
						sliceResult.RemovePolygon(poly);
					}
				}

				sliceResult.AddGameObjects (PerformResult (sliceResult.polygons, slice2D));

				return(sliceResult);
			}
		}
		
		return(Slice2D.Create (slice));
	}

	public Slice2D PointSlice(Vector2f point, float rotation) {
		Slice2D slice2D = Slice2D.Create (point, rotation);
		if (this.isActiveAndEnabled == false)
			return(slice2D);

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.PointSlice (colliderPolygon, point, rotation);
			sliceResult.AddGameObjects (PerformResult (sliceResult.polygons, slice2D));
			
			return(sliceResult);
		}

		return(slice2D);
	}

	public Slice2D PolygonSlice(Polygon2D slice, Polygon2D sliceDestroy, Polygon2D slicePolygonDestroy) {
		Slice2D slice2D = Slice2D.Create (slice);
		if (this.isActiveAndEnabled == false)
			return(slice2D);
		
		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.PolygonSlice (colliderPolygon, slice);

			if (sliceResult.polygons.Count > 0) { //  || ComplexSlicer.success == true !!!!!!!!!!!!!!!!!!!
				if (slicePolygonDestroy != null)
					foreach (Polygon2D p in new List<Polygon2D>(sliceResult.polygons))
						if (sliceDestroy.PolyInPoly (p) == true)
							sliceResult.RemovePolygon (p);

				if (sliceResult.polygons.Count > 0) {
					// Check If Slice Result Is Correct
					sliceResult.AddGameObjects (PerformResult (sliceResult.polygons, slice2D));

				} else if (slicePolygonDestroy != null)
					Destroy (gameObject);
	
				return(sliceResult);
			}
		}

		return(slice2D);
	}

	public Slice2D ExplodingPointSlice(Vector2f point) {
		Slice2D slice2D = Slice2D.Create (point);
		if (this.isActiveAndEnabled == false)
			return(slice2D);

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.ExplodingSlice (colliderPolygon, point);
			sliceResult.AddGameObjects (PerformResult (sliceResult.polygons, slice2D));
			
			return(sliceResult);
		}

		return(slice2D);
	}

	public Slice2D Explode() {
		Slice2D slice2D = Slice2D.Create (Slice2D.SliceType.Exploding);
		if (this.isActiveAndEnabled == false)
			return(slice2D);

		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) {
			Slice2D sliceResult = Slicer2D.Explode (colliderPolygon);
			sliceResult.AddGameObjects (PerformResult (sliceResult.polygons, slice2D));
			
			return(sliceResult);
		}

		return(slice2D);
	}

	// Does not return GameObjects
	public Slice2D PolygonSlice2(Polygon2D slice) {
		Polygon2D colliderPolygon = GetPolygonToSlice ();
		if (colliderPolygon != null) 
			return(Slicer2D.PolygonSlice (colliderPolygon, slice));

		return(Slice2D.Create (slice));
	}

	public List<GameObject> PerformResult(List<Polygon2D> result, Slice2D slice)
	{
		List<GameObject> resultGameObjects = new List<GameObject> ();

		if (result.Count < 1) {
			return(resultGameObjects);
		}

		if (sliceEvent != null) {
			slice.polygons = result;
			if (sliceEvent (slice) == false) {
				return(resultGameObjects);
			}
		}

		float originArea = 1f;

		if (recalculateMass) {
			originArea = Polygon2D.CreateFromCollider (gameObject).GetArea();
		}

		Rigidbody2D originalRigidBody = GetComponent<Rigidbody2D>();

		int name_id = 1;
		foreach (Polygon2D id in result) {
			GameObject gObject = Instantiate (gameObject) as GameObject;

			resultGameObjects.Add (gObject);
	
			foreach (Behaviour childCompnent in gObject.GetComponentsInChildren<Behaviour>()) {
				foreach (Behaviour child in GetComponentsInChildren<Behaviour>()) {
					if (child.GetType() == childCompnent.GetType()) { // Same Components Fail
						childCompnent.enabled = child.enabled;
						break;
					}
				}
			}
				
			Slicer2D slicer = gObject.GetComponent<Slicer2D> ();
			slicer.sliceCounter = sliceCounter + 1;
			slicer.maxSlices = maxSlices;

			gObject.name = name + " (" + name_id + ")";
			gObject.transform.parent = transform.parent;
			gObject.transform.position = transform.position;
			gObject.transform.rotation = transform.rotation;
			gObject.transform.localScale = transform.localScale;

			if (originalRigidBody) {
				Rigidbody2D newRigidBody = gObject.GetComponent<Rigidbody2D> ();
				newRigidBody.velocity = originalRigidBody.velocity;
				newRigidBody.angularVelocity = originalRigidBody.angularVelocity;
				
				if (recalculateMass) {
					float newArea = id.ToLocalSpace(transform).GetArea ();
					newRigidBody.mass = originalRigidBody.mass * (newArea / originArea);
				}
			}

			switch (textureType) {
				case TextureType.Sprite:
					if (gameObject.GetComponent<SpriteRenderer> () != null && gObject.GetComponent<SpriteMesh2D> () == null)
						gObject.AddComponent<SpriteMesh2D> ();

					break;

				case TextureType.Mesh:
				case TextureType.None:
				default:
					break;
			}

			PhysicsMaterial2D material = gObject.GetComponent<Collider2D> ().sharedMaterial;
			bool isTrigger = gObject.GetComponent<Collider2D>().isTrigger;

			switch (colliderType){ // if Collider <> Polygon Collider Destroy
				case Polygon2D.ColliderType.Box:
					Destroy (gObject.GetComponent<BoxCollider2D> ());
					break;
				case Polygon2D.ColliderType.Circle:
					Destroy(gObject.GetComponent<CircleCollider2D>());
					break;
				case Polygon2D.ColliderType.Capsule:
					Destroy(gObject.GetComponent<CapsuleCollider2D>());
					break;
				default:
					break;
			}

			id.ToLocalSpace (gObject.transform).CreateCollider (gObject);
			gObject.GetComponent<PolygonCollider2D> ().sharedMaterial = material;
			gObject.GetComponent<PolygonCollider2D> ().isTrigger = isTrigger;

			name_id += 1;
		}
			
		Destroy (gameObject);

		if (resultGameObjects.Count > 0) {
			slice.gameObjects = resultGameObjects;
			SliceJointEvent (slice);
			if ((sliceResultEvent != null)) {
				sliceResultEvent (slice);
			}
		}

		return(resultGameObjects);
	}

	public bool MatchLayers(Slice2DLayer sliceLayer)
	{
		return((sliceLayer == null || sliceLayer.GetLayerType() == Slice2DLayer.Type.All) || sliceLayer.GetLayerState(GetLayerID ()));
	}
		
	static public Slice2D LinearSlice(Polygon2D polygon, Pair2f slice) { return(LinearSlicer.Slice (polygon, slice)); }
	static public Slice2D PointSlice(Polygon2D polygon, Vector2f point, float rotation) { return(LinearSlicerExtended.SliceFromPoint (polygon, point, rotation)); }
	static public Slice2D ComplexSlice(Polygon2D polygon, List<Vector2f> slice) { return(ComplexSlicer.Slice (polygon, slice)); }
	static public Slice2D LinearCutSlice(Polygon2D polygon, LinearCut linearCut) { return(ComplexSlicerExtended.LinearCutSlice (polygon, linearCut)); }
	static public Slice2D PolygonSlice(Polygon2D polygon, Polygon2D polygonB) { return(ComplexSlicerExtended.Slice (polygon, polygonB)); }
	static public Slice2D ExplodingSlice(Polygon2D polygon, Vector2f point) { return(LinearSlicerExtended.PointExplode (polygon, point)); }
	static public Slice2D Explode(Polygon2D polygon) { return(LinearSlicerExtended.Explode (polygon)); }
	static public Polygon2D CreatorSlice(List<Vector2f> slice) { return(ComplexSlicerExtended.CreateSlice (slice)); }

	static public List<Slice2D> LinearSliceAll(Pair2f slice, Slice2DLayer layer)
	{
		List<Slice2D> result = new List<Slice2D> ();
		foreach (Slicer2D id in GetList ())
			if (id.MatchLayers (layer)) 
			{
					Slice2D sliceResult = id.LinearSlice (slice);
					if (sliceResult.gameObjects.Count > 0) 
						result.Add (sliceResult);
			}

		return(result);
	}

	static public List<Slice2D> PointSliceAll(Vector2f slice, float rotation, Slice2DLayer layer)
	{
		List<Slice2D> result = new List<Slice2D> ();
		foreach (Slicer2D id in GetList())
			if (id.MatchLayers (layer)) 
			{
				Slice2D sliceResult = id.PointSlice (slice, rotation);
				if (sliceResult.gameObjects.Count > 0) 
					result.Add (sliceResult);
			}

		return(result);
	}

	static public List<Slice2D> ComplexSliceAll(List<Vector2f> slice, Slice2DLayer layer)
	{
		List<Slice2D> result = new List<Slice2D> ();
		foreach (Slicer2D id in GetList())
			if (id.MatchLayers (layer)) 
			{
				Slice2D sliceResult = id.ComplexSlice (slice);
				if (sliceResult.gameObjects.Count > 0) 
					result.Add (sliceResult);
			}
				
		return(result);
	}

	static public List<Slice2D> LinearCutSliceAll(LinearCut linearCut, Slice2DLayer layer)
	{
		List<Slice2D> result = new List<Slice2D> ();
		foreach (Slicer2D id in GetList())
			if (id.MatchLayers (layer)) {
				Slice2D sliceResult = id.LinearCutSlice (linearCut);
				if (sliceResult.gameObjects.Count > 0) 
					result.Add (sliceResult);
			}
				
		return(result);
	}

	// Shouldn't Have Position
	static public List<Slice2D> PolygonSliceAll(Vector2f position, Polygon2D slicePolygon, Polygon2D slicePolygonDestroy, Slice2DLayer layer)
	{
		Polygon2D sliceDestroy = null;
		Polygon2D slice = slicePolygon.ToOffset (position);

		if (slicePolygonDestroy != null) {
			sliceDestroy = slicePolygonDestroy.ToOffset (position);
		}

		List<Slice2D> result = new List<Slice2D> ();
		foreach (Slicer2D id in GetList()) {
			if (id.MatchLayers (layer))  {
				result.Add (id.PolygonSlice (slice, slicePolygon, sliceDestroy));
			}
		}
		
		return(result);
	}
	
	static public List<Slice2D> ExplodingSliceAll(Vector2f point, Slice2DLayer layer)
	{
		List<Slice2D> result = new List<Slice2D> ();
		foreach (Slicer2D id in GetList())
			if (id.MatchLayers (layer)) {
				Slice2D sliceResult = id.ExplodingPointSlice (point);
				if (sliceResult.gameObjects.Count > 0)
					result.Add (sliceResult);
			}

		return(result);
	}

	static public List<Slice2D> ExplodeAll(Slice2DLayer layer)
	{
		List<Slice2D> result = new List<Slice2D> ();
		foreach (Slicer2D id in GetList())
			if (id.MatchLayers (layer))  {
				Slice2D sliceResult = id.Explode ();
				if (sliceResult.gameObjects.Count > 0)
					result.Add (sliceResult);
			}

		return(result);
	}
		
	private Polygon2D GetPolygonToSlice()
	{
		if (sliceCounter >= maxSlices && slicingLimit) {
			return(null);
		}

		return(Polygon2D.CreateFromCollider (gameObject, colliderType).ToWorldSpace (gameObject.transform));
	}
}