using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoColliderSlice : MonoBehaviour {

	void Update () {
		float timer = Time.realtimeSinceStartup;
		Polygon2D poly = Polygon2D.CreateFromCollider(gameObject).ToWorldSpace(transform);

		Slicer2D.ComplexSliceAll(poly.pointsList, Slice2DLayer.Create());
	
		Destroy(gameObject);

		Debug.Log(name + " in " + (Time.realtimeSinceStartup - timer) * 1000 + "ms");
	}
}
