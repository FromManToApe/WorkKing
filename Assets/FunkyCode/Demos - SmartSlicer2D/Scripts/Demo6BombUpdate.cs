using System.Collections.Generic;
using UnityEngine;

public class Demo6BombUpdate : MonoBehaviour {
	private float timer = 0;

	void Update()
	{
		timer += Time.deltaTime;
		if (timer > 5.0)
			Destroy (gameObject);
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.name.Contains ("Terrain")) {
			Vector2f pos = new Vector2f (transform.position);

			Polygon2D.defaultCircleVerticesCount = 15;;
			Polygon2D slicePolygon = Polygon2D.Create (Polygon2D.PolygonType.Circle, 2f);

			Polygon2D slicePolygonDestroy = null;
			Polygon2D sliceDestroy = null;

			slicePolygonDestroy = Polygon2D.Create (Polygon2D.PolygonType.Circle, 2.5f);
			sliceDestroy = new Polygon2D ();

			foreach (Vector2f id in slicePolygonDestroy.pointsList) 
				sliceDestroy.AddPoint (new Vector2f (id.vector + pos.vector));

			Polygon2D slice = new Polygon2D ();
			foreach (Vector2f id in slicePolygon.pointsList) 
				slice.AddPoint (new Vector2f (id.vector + pos.vector));

			foreach (Slicer2D id in Slicer2D.GetList()) {
				Slice2D result = id.PolygonSlice2 (slice); // Why not use Slice All?
				if (result.polygons.Count > 0) {
					foreach (Polygon2D p in new List<Polygon2D>(result.polygons))
						if (sliceDestroy.PolyInPoly (p) == true)
							result.polygons.Remove (p);

					if (result.polygons.Count > 0)
						id.PerformResult (result.polygons, new Slice2D());
					else
						Destroy (id.gameObject);
				}
			}
			Destroy (gameObject);

			Polygon2D.defaultCircleVerticesCount = 25;
		}
	}
}
