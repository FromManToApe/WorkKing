using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DInspector : MonoBehaviour {
	Vector3 inspectorPosition = Vector3.zero;
	float originalSize = 0;
	float currentSize = 0;
	int sliced = 0;
	
	void OnGUI() {
		if (Vector3.zero == inspectorPosition) {
			return;
		}


		Vector2 p = Camera.main.WorldToScreenPoint (inspectorPosition);
		TextWithShadow(p.x, p.y, "Original Size: " + (int)originalSize + " (100%)");

		inspectorPosition.y += 1;
		p = Camera.main.WorldToScreenPoint (inspectorPosition);
		TextWithShadow(p.x, p.y, "Current Size: " + (int)currentSize + " (" + Mathf.Floor((currentSize / originalSize) * 100) + "%)");

		inspectorPosition.y += 1;
		p = Camera.main.WorldToScreenPoint (inspectorPosition);
		TextWithShadow(p.x, p.y, "Sliced: " + sliced + " times");
	}

	public void TextWithShadow(float x, float y, string text) {
		GUIStyle textStyle2 = GUI.skin.GetStyle("Label");
    	textStyle2.alignment = TextAnchor.UpperCenter;
		textStyle2.normal.textColor = Color.black;

		GUI.Label(new Rect(x - 99, Screen.height - y + 1, 200, 20), text, textStyle2);

		GUIStyle textStyle = GUI.skin.GetStyle("Label");
    	textStyle.alignment = TextAnchor.UpperCenter;
		textStyle.normal.textColor = Color.white;

		GUI.Label(new Rect(x - 100, Screen.height - y, 200, 20), text, textStyle);
	}
	
	public void OnRenderObject() {
		if (Camera.current != Camera.main) {
			return;
		}

		Max2D.SetLineWidth (0.25f);
		Max2D.SetColor (Color.black);
		Max2D.SetBorder (false);
		Max2D.SetLineMode(Max2D.LineMode.Smooth);

		inspectorPosition = Vector3.zero;

		Vector2f pos = new Vector2f(Camera.main.ScreenToWorldPoint (Input.mousePosition));
		foreach(Slicer2D slicer in Slicer2D.GetList()) {
			Polygon2D poly = slicer.polygon.ToWorldSpace(slicer.transform);
			if (poly.PointInPoly(pos)) {
				Rect rect = poly.GetBounds();

				Max2D.DrawLineRectf(rect.x, rect.y, rect.width, rect.height);
				Max2D.DrawLinef(rect.center.x, rect.center.y, rect.center.x, rect.center.y + rect.height / 2 + 1);
				
				inspectorPosition = new Vector2(rect.center.x, rect.center.y + rect.height / 2);
				
				originalSize = slicer.GetComponent<Slicer2DInspectorTracker>().originalSize;
				currentSize = poly.GetArea();
				sliced = slicer.sliceCounter;
			}
		}
	}
}
