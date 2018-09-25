using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColliderLineRenderer2D : MonoBehaviour {
	public Max2D.LineMode lineMode = Max2D.LineMode.Smooth;
	public Color color = Color.white;
	public float lineWidth = 1;
	public bool smooth = true;

	private bool connectedLine = true; // Prototype

	private float lineOffset = -0.001f;
	private Polygon2D poly = new Polygon2D();

	public void Start()
	{
		poly = Polygon2D.CreateFromCollider (gameObject);

		if (GetComponent<EdgeCollider2D>() != null)
			connectedLine = false;
	}

	public void OnRenderObject()
	{
		if (Camera.current != Camera.main) {
			return;
		}

		Max2D.SetLineWidth (lineWidth);
		Max2D.SetColor (color);
		Max2D.SetBorder (false);
		Max2D.SetLineMode(lineMode);

		Max2D.DrawPolygon (poly.ToWorldSpace (transform), transform.position.z + lineOffset, connectedLine);
	}
}