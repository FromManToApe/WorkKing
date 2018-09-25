using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinSliceEdge : MonoBehaviour {

	void OnEnable() { edgeList.Add (this);}
	void OnDisable() { edgeList.Remove (this);}
	static private List<ThinSliceEdge> edgeList = new List<ThinSliceEdge>();
	static public List<ThinSliceEdge> GetList(){ return(new List<ThinSliceEdge>(edgeList));}

	public bool ItersectsWithMap() {
		Polygon2D polyA = Polygon2D.CreateFromCollider(gameObject).ToWorldSpace(gameObject.transform);

		bool intersect = false;
		foreach(Slicer2D slicer in Slicer2D.GetList()) {
			Polygon2D polyB = Polygon2D.CreateFromCollider(slicer.gameObject).ToWorldSpace(slicer.gameObject.transform);

			if (MathHelper.SliceIntersectPoly(polyA.pointsList, polyB.pointsList)) {
				intersect = true;
			}

			foreach(Vector2f p in polyA.pointsList) {
				if (MathHelper.PointInPoly(p, polyB.pointsList)) {
					return(true);
				}
			}
		}
		return(intersect);
	}
}
