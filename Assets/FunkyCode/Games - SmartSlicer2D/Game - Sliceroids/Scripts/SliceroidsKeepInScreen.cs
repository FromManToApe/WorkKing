using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceroidsKeepInScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float ratio = (float)Screen.width / Screen.height;
		Vector3 position = transform.position;

		if (transform.position.x < Camera.main.transform.position.x - Camera.main.orthographicSize * ratio) {
			position.x = Camera.main.transform.position.x + Camera.main.orthographicSize * ratio;
		}

		if (transform.position.y < Camera.main.transform.position.y - Camera.main.orthographicSize) {
			position.y = Camera.main.transform.position.y + Camera.main.orthographicSize;
		}

		if (transform.position.x > Camera.main.transform.position.x + Camera.main.orthographicSize * ratio) {
			position.x = Camera.main.transform.position.x - Camera.main.orthographicSize * ratio;
		}

		if (transform.position.y > Camera.main.transform.position.y + Camera.main.orthographicSize) {
			position.y = Camera.main.transform.position.y - Camera.main.orthographicSize;
		}

		transform.position = position;
	}
}
