using System.Collections.Generic;
using UnityEngine;

public class Demo6Manager : MonoBehaviour {
	public GameObject bombPrefab;
	public GameObject bouncerPrefab;
	public Transform parent;

	void Update () {
		Vector2f pos = Slicer2DController.GetMousePosition ();

		if (Input.GetMouseButtonDown (0)) {
			GameObject g = Instantiate (bombPrefab) as GameObject;
			g.transform.position = new Vector3 (pos.vector.x, pos.vector.y, -5f);
			g.transform.parent = transform;
		}

		if (Input.GetMouseButtonDown (1)) {
			GameObject g = Instantiate (bouncerPrefab) as GameObject;
			g.transform.position = new Vector3 (pos.vector.x, pos.vector.y, -5f);
			g.transform.parent = transform;
		}
	}
}
