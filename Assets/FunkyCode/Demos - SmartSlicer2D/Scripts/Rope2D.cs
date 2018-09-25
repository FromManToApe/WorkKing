using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope2D : MonoBehaviour {
	public GameObject anchorBody;
	public GameObject connectedBody;

	public Color color;

	private List<GameObject> nodes = new List<GameObject> ();

	void Start () {
		Vector2f position = new Vector2f(anchorBody.transform.position);

		GameObject prev = anchorBody;

		int ropeId = 1;
		GameObject gObject = null;
		while (Vector2f.Distance (position, new Vector2f(connectedBody.transform.position)) > .5f) {
			float direction = Vector2f.Atan2 (new Vector2f(connectedBody.transform.position), position);

			gObject = new GameObject();
			gObject.AddComponent<HingeJoint2D>().connectedBody = prev.GetComponent<Rigidbody2D>();

			gObject.transform.parent = transform;
			gObject.transform.position = position.vector;
			gObject.name = "Rope " + ropeId;
			gObject.AddComponent<JointRenderer2D> ().color = color;
			gObject.AddComponent<CircleCollider2D> ().radius = .25f;

			nodes.Add(gObject);

			position.Push (direction, .5f);

			prev = gObject;
			ropeId++;
		}

		if (gameObject != null)
			gObject.AddComponent<HingeJoint2D>().connectedBody = connectedBody.GetComponent<Rigidbody2D>();

	}
}
