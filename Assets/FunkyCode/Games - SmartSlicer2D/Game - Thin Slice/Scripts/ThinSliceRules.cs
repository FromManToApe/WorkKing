using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinSliceRules : MonoBehaviour {

	void Start () {
		Slicer2D slicer = GetComponent<Slicer2D>();
		slicer.AddEvent(OnSlice);
		slicer.AddResultEvent(AfterSlice);
	}
	
	// Triggered Before Every Slice
	bool OnSlice(Slice2D sliceResult) {
		Polygon2D CutObject = GetCutPolygon(sliceResult);

		if (sliceResult.polygons.Count > 2) {
			return(false); // Disallow double slices
		}

		if (SliceIntesectEdges(Slicer2DController.complexSlicerPointsList) == true) {
			ThinSliceGameManager.CreateParticles();
			Slicer2DController.complexSlicerPointsList.Clear();
			Debug.Log("Slice Intersect Edges");
			return(false);
		}

		if (CutObject == null) {
			ThinSliceGameManager.CreateParticles();
			Slicer2DController.complexSlicerPointsList.Clear();
			return(false);
		}

		return(true);	
	}

	// Triggered On Every Successful Slice
	void AfterSlice(Slice2D sliceResult) {
		GameObject CutObject = GetCutGameObject(sliceResult);
		
		if (CutObject != null) {
			ExplodePolygon(CutObject);
		}

		// Destroy Edge If It Does Not Intersect With Level
		foreach(ThinSliceEdge edge in ThinSliceEdge.GetList()) {
			if (edge.ItersectsWithMap() == false) {
				Destroy(edge.gameObject);
			}
		}

		ThinSliceGameManager.instance.UpdateText();
	}

	// After Slice - Get smallest polygon which does not have balls in it
	GameObject GetCutGameObject(Slice2D sliceResult) {
		float area = 1e+10f;
		GameObject CutObject = null;
		foreach(GameObject resultObject in sliceResult.gameObjects) {
			Polygon2D poly = Polygon2D.CreateFromCollider(resultObject);
			if (poly.GetArea() < area && PolygonHasBallsInside(poly.ToWorldSpace(resultObject.transform)) == false) {
				CutObject = resultObject;
				area = poly.GetArea();
			}
		}
		return(CutObject);
	}

	// Before Slice - Get smallest polygon which does not have balls in it
	Polygon2D GetCutPolygon(Slice2D sliceResult) {
		float area = 1e+10f;
		Polygon2D CutObject = null;
		foreach(Polygon2D poly in sliceResult.polygons) {
			if (poly.GetArea() < area && PolygonHasBallsInside(poly) == false) {
				CutObject = poly;
				area = poly.GetArea();
			}
		}
		return(CutObject);
	}

	// Check if polygon has ball objects inside
	bool PolygonHasBallsInside(Polygon2D poly) {
		foreach(ThinSliceBall ball in ThinSliceBall.GetList()) {
			if (poly.PointInPoly(new Vector2f(ball.transform.position)) == true) {
				return(true);
			}
		}
		return(false);
	}

	bool SliceIntesectEdges(List<Vector2f> slice) {
		foreach(ThinSliceEdge edge in ThinSliceEdge.GetList()) {
			Polygon2D edgePolygon = Polygon2D.CreateFromCollider(edge.gameObject);
			if (MathHelper.SliceIntersectSlice(slice, edgePolygon.ToWorldSpace(edge.transform).pointsList)){
				return(true);
			}
		}

		return(false);
	}

	// Polygon Scatter Particles Effect
	void ExplodePolygon(GameObject CutObject) {
		Slicer2D.explosionPieces = 5;
		Slice2D explosionResult = CutObject.GetComponent<Slicer2D>().Explode();
		
		float z = 0f;

		foreach(GameObject b in explosionResult.gameObjects) {
			z -= 0.01f;

			Slicer2D slicer = b.GetComponent<Slicer2D>();
			slicer.Initialize();
			Destroy(slicer);

			b.AddComponent<DestroyTimer>();

			Rigidbody2D rigidBody2D = b.AddComponent<Rigidbody2D>();
			
			b.transform.Translate(0, 0, 1 + z);

			if (rigidBody2D) {
				Rect rect = Polygon2D.CreateFromCollider (b).GetBounds ();
				float sliceRotation = Vector2f.Atan2 (new Vector2f (rect.center), new Vector2f(b.transform.position));
				Physics2DHelper.AddForceAtPosition(rigidBody2D, new Vector2 (Mathf.Cos (sliceRotation) * 351f, Mathf.Sin (sliceRotation) * 351f), rect.center);
			}
		}
	}
}
