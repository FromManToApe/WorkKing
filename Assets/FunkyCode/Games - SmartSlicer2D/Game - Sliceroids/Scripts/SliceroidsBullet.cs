using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceroidsBullet : MonoBehaviour {
	ColliderLineRenderer2D lineRenderer;

	float timer = 0;

	void Start() {
		lineRenderer = GetComponent<ColliderLineRenderer2D>();
		timer = Time.realtimeSinceStartup;
	}

	void Update () {
		transform.Translate(0, 0.1f, 0);

		if (timer + 2 < Time.realtimeSinceStartup) {
			if (timer + 3 > Time.realtimeSinceStartup) {
				lineRenderer.color.a = (timer + 3) - Time.realtimeSinceStartup;
			} else {
				Destroy(gameObject);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D c) {
		if (c.tag == "Player") {
			return;
		}

		c.GetComponent<Slicer2D>().Explode();
		Destroy(gameObject);
	}
}
