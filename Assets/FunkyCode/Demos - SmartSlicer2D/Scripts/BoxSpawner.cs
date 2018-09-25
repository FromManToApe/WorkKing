using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour {
	public GameObject spawnObject;
	public TimeHelper time;
	void SpawnBox() {
		GameObject box = Instantiate(spawnObject, transform) as GameObject;
		box.transform.parent = transform;
		box.transform.localPosition = new Vector3(0, 10, 0);
	}

	void Start () {
		 SpawnBox();
		 time = TimeHelper.Create();
	}
	
	void Update () {
		if (time.GetMillisecs() > 750) {
			SpawnBox();
			time = TimeHelper.Create();
		}
	}
}
