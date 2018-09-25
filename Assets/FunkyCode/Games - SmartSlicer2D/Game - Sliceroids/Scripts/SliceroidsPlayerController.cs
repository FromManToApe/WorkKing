using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceroidsPlayerController : MonoBehaviour {
	Rigidbody2D body;

	public GameObject bullet;

	void Start () {
		body = GetComponent<Rigidbody2D>();
	}
	
	void Update () {
		if (Input.GetKey(KeyCode.A)) {
			body.AddTorque(1);
		}

		if (Input.GetKey(KeyCode.D)) {
			body.AddTorque(-1);
		}

		if (body.angularVelocity > 300)
			body.angularVelocity = 300;

		if (body.angularVelocity < -300)
			body.angularVelocity = -300;

		if (Input.GetKey(KeyCode.W)) {
			body.AddForce(transform.TransformVector(new Vector3(0, 5, 0)));
		}

		if (Input.GetKey(KeyCode.S)) {
			body.AddForce(transform.TransformVector(new Vector3(0, -5, 0)));
		}

		if (Input.GetKeyDown(KeyCode.Space)) {
			GameObject bulletGameObject = Instantiate(bullet) as GameObject;
			bulletGameObject.transform.rotation = transform.rotation;
			bulletGameObject.transform.position = transform.position;
			bulletGameObject.transform.Translate(0, 2.1f, 0);
		}
	}
}
