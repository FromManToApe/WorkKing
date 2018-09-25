using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour {
	TimeHelper timer;

	void Start () {
		timer = TimeHelper.Create();
	}
	
	void Update () {
		if (timer.GetMillisecs() > 2000) {
			Destroy(gameObject);
		}
	}
}
