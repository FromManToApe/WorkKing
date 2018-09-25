using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Max2DOptimizeSetPassCalls : MonoBehaviour {
	void OnEnable() {
		if (GetComponent<Camera>() == null) {
			Debug.LogError("Camera Component Is Missing!");
		}
		Max2D.SetBatching(true);
	}

	void OnDisable() {
		Max2D.SetBatching(false);
	}
	
	public void OnPreRender() {
		Max2D.ResetSetPass();
	}
}
