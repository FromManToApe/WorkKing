using System.Collections.Generic;
using UnityEngine;

public class PolygonTriangulator2D : MonoBehaviour {
	public enum Triangulation {Advanced, Legacy};
	static float precision = 0.001f;

	public static Mesh Triangulate(Polygon2D polygon, Vector2 UVScale, Vector2 UVOffset, Triangulation triangulation)
	{
		Mesh result = null;
		switch (triangulation) {
			case Triangulation.Advanced:

				PreparePolygon(polygon);
				foreach (Polygon2D hole in polygon.holesList) {
					PreparePolygon(hole);
				}
				result = TriangulateAdvanced(polygon, UVScale, UVOffset);

			break;

			case Triangulation.Legacy:
				List<Vector2> list = new List<Vector2>();
				foreach(Vector2f p in polygon.pointsList) {
					list.Add(p.vector);
				}
				result = Triangulator.Create(list.ToArray());
				
			break;
		}

		return(result);
	}

	// Not finished - still has some artifacts
	public static void PreparePolygon(Polygon2D polygon)
	{
		foreach (Pair3f pA in Pair3f.GetList(polygon.pointsList)) {
			foreach (Pair3f pB in  Pair3f.GetList(polygon.pointsList)) {
				if (pA.B != pB.B && Vector2f.Distance(pA.B, pB.B) < precision) {
					pA.B.Push (Vector2f.Atan2 (new Vector2f(pA.A), new Vector2f(pA.B)), precision);
					pA.B.Push (Vector2f.Atan2 (new Vector2f(pA.B), new Vector2f(pA.C)), -precision);
					pB.B.Push (Vector2f.Atan2 (new Vector2f(pB.A), new Vector2f(pB.B)), precision);
					pB.B.Push (Vector2f.Atan2 (new Vector2f(pB.B), new Vector2f(pB.C)), -precision);
				}
			}
		}
	}

	public static Mesh TriangulateAdvanced(Polygon2D polygon, Vector2 UVScale, Vector2 UVOffset)
	{
		polygon.Normalize ();
		TriangulationWrapper.Polygon poly = new TriangulationWrapper.Polygon();

		List<Vector2> pointsList = null;
		List<Vector2> UVpointsList = null;

		Vector3 v = Vector3.zero;

		foreach (Vector2f p in polygon.pointsList) {
			v = p.vector;
			poly.outside.Add (v);
			poly.outsideUVs.Add (new Vector2(v.x / UVScale.x + .5f + UVOffset.x, v.y / UVScale.y + .5f + UVOffset.y));
		}

		foreach (Polygon2D hole in polygon.holesList) {
			pointsList = new List<Vector2> ();
			UVpointsList = new List<Vector2> ();
			foreach (Vector2f p in hole.pointsList) {
				v = p.vector;
				pointsList.Add (v);
				UVpointsList.Add (new Vector2(v.x / UVScale.x + .5f, v.y / UVScale.y + .5f));
			}
			poly.holes.Add (pointsList);
			poly.holesUVs.Add (UVpointsList);
		}

		return(TriangulationWrapper.CreateMesh (poly));
	}
}