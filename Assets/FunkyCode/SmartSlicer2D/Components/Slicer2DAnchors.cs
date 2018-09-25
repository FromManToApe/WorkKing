using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DAnchors : MonoBehaviour {
	public enum AnchorType {AttachRigidbody, CancelSlice}; // DestroySlice
	public Collider2D[] anchorColliders = new Collider2D[1];
	public AnchorType anchorType = AnchorType.AttachRigidbody;

	private List<Polygon2D> polygons = new List<Polygon2D>();
	private List<Collider2D> colliders = new List<Collider2D>();

	void Start () {
		bool addEvents = false;

		foreach(Collider2D collider in anchorColliders) {
			addEvents = true;
		}

		if (addEvents == false) {
			return;
		}

		Slicer2D slicer = GetComponent<Slicer2D> ();
		if (slicer != null) {
			slicer.AddResultEvent (OnSliceResult);
			slicer.AddEvent (OnSlice);
		}

		foreach(Collider2D collider in anchorColliders) {
			polygons.Add(Polygon2D.CreateFromCollider (collider.gameObject));
			colliders.Add(collider);
		}
	}

	Polygon2D GetPolygonInWorldSpace(Polygon2D poly) {
		return(poly.ToWorldSpace(colliders[polygons.IndexOf(poly)].transform));
	}

	bool OnSlice(Slice2D sliceResult)
	{
		switch (anchorType) {
			case AnchorType.CancelSlice:
				foreach (Polygon2D polyA in new List<Polygon2D>(sliceResult.polygons)) {
					bool perform = true;
					foreach(Polygon2D polyB in polygons) {
						if (MathHelper.PolyCollidePoly (polyA.pointsList, GetPolygonInWorldSpace(polyB).pointsList) ) {
							perform = false;
						}
					}
					if (perform) {
						return(false);
					}
				}
				break;
				/* 
			case AnchorType.DestroySlice:
				foreach (Polygon2D polyA in new List<Polygon2D>(sliceResult.polygons)) {
					bool perform = true;
					foreach(Polygon2D polyB in polygons) {
						if (MathHelper.PolyCollidePoly (polyA.pointsList, GetPolygonInWorldSpace(polyB).pointsList) ) {
							sliceResult.polygons.Remove(polyB);
						}
					}
					
				}
				break;
				*/

			default:
				break;
		}
		return(true);
	}

	void OnSliceResult(Slice2D sliceResult)
	{
		if (polygons.Count < 1)
			return;

		foreach (GameObject p in sliceResult.gameObjects) {
			Polygon2D polyA = Polygon2D.CreateFromCollider (p).ToWorldSpace (p.transform);
			bool perform = true;

			foreach(Polygon2D polyB in polygons) {
				if (MathHelper.PolyCollidePoly (polyA.pointsList, GetPolygonInWorldSpace(polyB).pointsList)) {
					perform = false;
				}
			}

			if (perform) {
				switch (anchorType) {
					case AnchorType.AttachRigidbody:
						if (p.GetComponent<Rigidbody2D> () == null) {
							p.AddComponent<Rigidbody2D> ();
						}

						p.GetComponent<Rigidbody2D> ().isKinematic = false;
						break;
					default:
						break;
				}
			}
		}
	}
}
