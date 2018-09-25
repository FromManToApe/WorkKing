using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SliceroidsColliderLineRenderer2D : MonoBehaviour {
	public Color color = Color.white;
	public float lineWidth = 1;
	public bool smooth = true;

	private bool connectedLine = true;

	private float lineOffset = -0.001f;
	private Polygon2D poly = new Polygon2D ();

	public void Start()
	{
		poly = Polygon2D.CreateFromCollider (gameObject);

		if (GetComponent<EdgeCollider2D>() != null) {
			connectedLine = false;
		}
	}

	public void OnRenderObject()
	{
		if (Camera.current != Camera.main) {
			return;
		}

		float ratio = (float)Screen.width / Screen.height;
		float z = transform.position.z + lineOffset;
		
		Max2D.SetLineWidth (lineWidth);
		Max2D.SetColor (color);
		Max2D.SetBorder (false);
		Max2D.SetLineMode(Max2D.LineMode.Glow);

		Max2D.DrawPolygon (poly.ToWorldSpace (transform), z, connectedLine);
		Max2D.DrawPolygon (poly.ToWorldSpace (transform).ToOffset(new Vector2f(0, 2 * Camera.main.orthographicSize)), z, connectedLine);
		Max2D.DrawPolygon (poly.ToWorldSpace (transform).ToOffset(new Vector2f(0, 2 * -Camera.main.orthographicSize)), z, connectedLine);
		
		Max2D.DrawPolygon (poly.ToWorldSpace (transform).ToOffset(new Vector2f(ratio * 2 * Camera.main.orthographicSize, 0)), z, connectedLine);
		Max2D.DrawPolygon (poly.ToWorldSpace (transform).ToOffset(new Vector2f(ratio * 2 * -Camera.main.orthographicSize, 0)), z, connectedLine);
	}
}
