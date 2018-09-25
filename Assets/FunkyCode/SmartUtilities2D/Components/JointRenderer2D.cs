using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class JointRenderer2D : MonoBehaviour {
	public Color color = Color.white;
	public float lineWidth = 1;
	public bool smooth = true;

	private float lineOffset = -0.001f;
	private List<Joint2D> joints = new List<Joint2D>();

	public void Start() {
		joints = Joint2D.GetJoints(gameObject);
	}

	public void OnRenderObject() {
		if (Camera.current != Camera.main) {
			return;
		}

		foreach(Joint2D joint in joints) {
			if (joint.gameObject == null) {
				continue;
			}
			if (joint.anchoredJoint2D == null) {
				continue;
			}
			if (joint.anchoredJoint2D.isActiveAndEnabled == false) {
				continue;
			}
			if (joint.anchoredJoint2D.connectedBody == null) {
				continue;
			}

			Max2D.SetLineWidth (lineWidth);
			Max2D.SetColor (color);
			Max2D.SetLineMode(Max2D.LineMode.Smooth);
			Max2D.SetBorder (false);

			switch (joint.jointType) {
				case Joint2D.Type.HingeJoint2D:
					Max2D.DrawLine (new Vector2f (transform.TransformPoint (joint.anchoredJoint2D.anchor)), new Vector2f (joint.anchoredJoint2D.connectedBody.transform.TransformPoint (Vector2.zero)), transform.position.z + lineOffset);
					break;

				default:
					Max2D.DrawLine (new Vector2f (transform.TransformPoint (joint.anchoredJoint2D.anchor)), new Vector2f (joint.anchoredJoint2D.connectedBody.transform.TransformPoint (joint.anchoredJoint2D.connectedAnchor)), transform.position.z + lineOffset);	
					break;
			}
		}

	}
}