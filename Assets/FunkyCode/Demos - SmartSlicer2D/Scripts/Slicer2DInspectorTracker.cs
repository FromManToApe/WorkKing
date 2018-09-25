using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DInspectorTracker : MonoBehaviour {
	public float originalSize = 0;
	void Start () {
		if (originalSize == 0) {
			originalSize = Polygon2D.CreateFromCollider(gameObject).ToWorldSpace(transform).GetArea();
		}
	}

}
