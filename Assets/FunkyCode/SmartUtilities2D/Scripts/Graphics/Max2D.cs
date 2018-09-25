using System.Collections.Generic;
using UnityEngine;

public class Max2D {
	public enum LineMode {Smooth, Glow, Default};
	
	private static Material lineMaterial;
	private static Material glowMaterial;
	private static Material defaultMaterial; 

	public static float lineWidth = 0.2f;
	public static Max2D.LineMode lineMode = Max2D.LineMode.Smooth;
	private static bool setBorder = false;
	private static Color setColor = Color.white;

	private static Material setPassMaterial = null;
	private static bool optimizeSetPassCalls = false;

	static public void SetPass(Material material) {
		if (optimizeSetPassCalls == false || material != setPassMaterial) {
			material.SetPass(0);
			
			setPassMaterial = material;
		}
	}

	static public void ResetSetPass() {
		setPassMaterial = null;
	}

	static public void SetBatching(bool set) {
		optimizeSetPassCalls = set;
	}

	static public void Vertex3(Vector2f p, float z)
	{
		GL.Vertex3(p.vector.x, p.vector.y, z);
	}

	static public void SetBorder(bool border)
	{
		setBorder = border;
	}

	static public void SetLineMode(LineMode mode)
	{
		lineMode = mode;
	}

	public static void SetLineWidth (float size)
	{
		lineWidth = Mathf.Max(.01f, size / 5f);
	}

	private static void Check()
	{
		if (lineMaterial == null || glowMaterial == null) {
			lineMaterial = new Material (Shader.Find ("Legacy Shaders/Transparent/VertexLit"));
			lineMaterial.mainTexture = Resources.Load ("Textures/LineTexture") as Texture;
			glowMaterial = new Material (Shader.Find ("Particles/Additive"));
			glowMaterial.mainTexture = Resources.Load ("Textures/LineGlowTexture") as Texture;
			lineMaterial.SetInt("_ZWrite", 1);
			lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
		}
		if (defaultMaterial == null) {
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			defaultMaterial = new Material(shader);
			defaultMaterial.hideFlags = HideFlags.HideAndDontSave;
			defaultMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			defaultMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			defaultMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			defaultMaterial.SetInt("_ZWrite", 0);
		}
	}

	static public void SetColor(Color color)
	{
		Check ();
		lineMaterial.SetColor ("_Emission", color);
		setColor = color;
	}

	static public void DrawMesh(Mesh mesh, Transform transform, Vector2f offset, float z = 0f)
	{
		if (mesh == null)
			return;
		
		GL.PushMatrix ();
		SetPass(defaultMaterial);
		GL.Begin(GL.TRIANGLES);

		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < mesh.triangles.GetLength (0); i++ ) {
			list.Add (transform.TransformPoint(mesh.vertices [mesh.triangles [i]]));
			if (list.Count > 2) {
				Max2DMatrix.DrawTriangle (list [0].x, list [0].y, list [1].x, list [1].y, list [2].x, list [2].y, offset, z);
				list.Clear ();
			}
		}

		GL.End ();
		GL.PopMatrix ();
	}

	static public void DrawTriangle(Vector2f p0, Vector2f p1, Vector2f p2, Vector2f offset, float z = 0f)
	{
		DrawTrianglef (p0.vector.x, p0.vector.y, p1.vector.x, p1.vector.y, p2.vector.x, p2.vector.y, offset, z);
	}

	static public void DrawSquare(Vector2f p, float size, float z = 0f)
	{
		Vector2f p0 = new Vector2f (p.vector.x - size, p.vector.y - size);
		Vector2f p1 = new Vector2f (p.vector.x + size, p.vector.y - size);
		Vector2f p2 = new Vector2f (p.vector.x + size, p.vector.y + size);
		Vector2f p3 = new Vector2f (p.vector.x - size, p.vector.y + size);

		DrawTriangle (p0, p1, p2, new Vector2f(0, 0), z);
		DrawTriangle (p2, p3, p0, new Vector2f(0, 0), z);
	}

	static public void DrawImage(Material material, Vector2f pos, Vector2f size, float z = 0f)
	{
		GL.PushMatrix ();
		SetPass(material);
		GL.Begin (GL.QUADS);

		GL.TexCoord2 (0, 0);
		GL.Vertex3 (pos.vector.x - size.vector.x, pos.vector.y - size.vector.y, z);
		GL.TexCoord2 (0, 1);
		GL.Vertex3 (pos.vector.x - size.vector.x, pos.vector.y + size.vector.y, z);
		GL.TexCoord2 (1, 1);
		GL.Vertex3 (pos.vector.x + size.vector.x, pos.vector.y + size.vector.y, z);
		GL.TexCoord2 (1, 0);
		GL.Vertex3 (pos.vector.x + size.vector.x, pos.vector.y - size.vector.y, z);

		GL.End ();
		GL.PopMatrix ();
	}

	static public void DrawLineSquare(Vector2f p, float size, float z = 0f)
	{
		DrawLineRectf (p.vector.x - size / 2f, p.vector.y - size / 2f, size, size, z);
	}

	static public void DrawLine(Vector2f p0, Vector2f p1, float z = 0f)
	{
		if (setBorder == true) {
			Color tmcColor = setColor;
			float tmpWidth = lineWidth;
			SetColor(Color.black);
			lineWidth = tmpWidth * 2f;
			DrawLinef (p0.vector.x, p0.vector.y, p1.vector.x, p1.vector.y, z);
			SetColor(tmcColor);
			lineWidth = tmpWidth;
			DrawLinef (p0.vector.x, p0.vector.y, p1.vector.x, p1.vector.y, z);
			lineWidth = tmpWidth;
		} else {
			DrawLinef(p0.vector.x, p0.vector.y, p1.vector.x, p1.vector.y, z);
		}
	}
		
	static public void DrawLinef(float x0, float y0, float x1, float y1, float z = 0f)
	{
		Check ();

		if (lineMode == LineMode.Smooth)
			DrawSmoothLine (new Pair2f (new Vector2f (x0, y0), new Vector2f (x1, y1)), z);
		else {
			GL.PushMatrix();
			SetPass(defaultMaterial);
			GL.Begin(GL.LINES);
			GL.Color(setColor);

			Max2DMatrix.DrawLine (x0, y0, x1, y1, z);

			GL.End();
			GL.PopMatrix();
		}
	}
		
	static public void DrawTrianglef(float x0, float y0, float x1, float y1, float x2, float y2, Vector2f offset, float z = 0f)
	{
		GL.PushMatrix();
		SetPass(defaultMaterial);
		GL.Begin(GL.TRIANGLES);
		GL.Color(setColor);

		Max2DMatrix.DrawTriangle(x0, y0, x1, y1, x2, y2, offset, z);

		GL.End();
		GL.PopMatrix();
	}

	static public void DrawLineRectf(float x, float y, float w, float h, float z = 0f)
	{
		if (lineMode == LineMode.Smooth) {
			GL.PushMatrix ();
			SetPass(lineMaterial);
			GL.Begin (GL.QUADS);

			if (setBorder == true) {
				Color tmcColor = setColor;
				float tmpWidth = lineWidth;

				SetColor (Color.black);
				lineWidth = tmpWidth * 2f;

				Max2DMatrix.DrawLineImage (new Pair2f (new Vector2f (x, y), new Vector2f (x + w, y)), z);
				Max2DMatrix.DrawLineImage (new Pair2f (new Vector2f (x, y), new Vector2f (x, y + h)), z);
				Max2DMatrix.DrawLineImage (new Pair2f (new Vector2f (x + w, y), new Vector2f (x + w, y + h)), z);
				Max2DMatrix.DrawLineImage (new Pair2f (new Vector2f (x, y + h), new Vector2f (x + w, y + h)), z);

				SetColor (tmcColor);
				lineWidth = tmpWidth;
			}

			float tmpLine = lineWidth;
			lineWidth = tmpLine * 1f;

			SetColor (setColor);

			Max2DMatrix.DrawLineImage (new Pair2f(new Vector2f(x, y), new Vector2f(x + w, y)), z);
			Max2DMatrix.DrawLineImage (new Pair2f(new Vector2f(x, y), new Vector2f(x, y + h)), z);
			Max2DMatrix.DrawLineImage (new Pair2f(new Vector2f(x + w, y), new Vector2f(x + w, y+ h)), z);
			Max2DMatrix.DrawLineImage (new Pair2f(new Vector2f(x, y + h), new Vector2f(x + w, y+ h)), z);

			GL.End();
			GL.PopMatrix();

			lineWidth = tmpLine;

		} else {
			DrawLine (new Vector2f (x, y), new Vector2f (x + w, y), z);
			DrawLine (new Vector2f (x + w, y), new Vector2f (x + w, y + h), z);
			DrawLine (new Vector2f (x + w, y + h),	new Vector2f (x, y + h), z);
			DrawLine (new Vector2f (x, y + h), new Vector2f (x, y), z);
		}
	}

	static public void DrawSlice(List< Vector2f> slice, float z = 0f)
	{
		foreach (Pair2f p in Pair2f.GetList(slice, false)) 
			DrawLine (p.A, p.B, z);
	}

	static public void DrawPolygonList(List<Polygon2D> polyList, float z = 0f)
	{
		foreach (Polygon2D p in polyList)
			DrawPolygon (p, z);
	}

	static public void DrawStrippedLine(List<Vector2f> pointsList, float minVertsDistance, float z = 0f, bool full = false, Vector2f offset = null)
	{
		if (offset == null)
			offset = new Vector2f (0, 0);

		Vector2f vA = null, vB = null;

		if (setBorder == true) {
			Color tmcColor = setColor;
			float tmpWidth = lineWidth;

			GL.PushMatrix();
			SetColor (Color.black);
			SetPass(lineMaterial);
			GL.Begin(GL.QUADS);

			lineWidth = 2f * tmpWidth;

			foreach (Pair2f id in Pair2f.GetList(pointsList, full)) {
				vA = new Vector2f (id.A.vector + offset.vector);
				vB = new Vector2f (id.B.vector + offset.vector);

				vA.Push (Vector2f.Atan2 (id.A, id.B), -minVertsDistance / 4);
				vB.Push (Vector2f.Atan2 (id.A, id.B), minVertsDistance / 4);

				Max2DMatrix.DrawLineImage (new Pair2f(vA, vB), z);
			}

			GL.End();
			GL.PopMatrix();

			SetColor (tmcColor);
			lineWidth = tmpWidth;
		}

		GL.PushMatrix();
		SetPass(lineMaterial);
		GL.Begin(GL.QUADS);

		foreach (Pair2f id in Pair2f.GetList(pointsList, full)) {
			vA = new Vector2f (id.A.vector + offset.vector);
			vB = new Vector2f (id.B.vector + offset.vector);

			vA.Push (Vector2f.Atan2 (id.A, id.B), -minVertsDistance / 4);
			vB.Push (Vector2f.Atan2 (id.A, id.B), minVertsDistance / 4);

			Max2DMatrix.DrawLineImage (new Pair2f(vA, vB), z);
		}

		GL.End();
		GL.PopMatrix();
	}

	static public void DrawSmoothLine(Pair2f pair, float z = 0f)
	{
		GL.PushMatrix();
		SetPass(lineMaterial);
		GL.Begin(GL.QUADS);

		Max2DMatrix.DrawLineImage (pair, z);

		GL.End();
		GL.PopMatrix();
	}

	static public void DrawPolygon(Polygon2D poly, float z = 0f, bool connect = true)
	{
		Check ();

		switch (lineMode) {
			case LineMode.Smooth:
				GL.PushMatrix ();
				SetPass(lineMaterial);
				GL.Begin(GL.QUADS);

				Max2DMatrix.DrawSliceImage (poly.pointsList, z, connect);

				GL.End();
				GL.PopMatrix();

				break;

			case LineMode.Default:
				GL.PushMatrix();
				SetPass(defaultMaterial);
				GL.Begin(GL.LINES);
				GL.Color(setColor);

				Max2DMatrix.DrawSlice(poly.pointsList, z, connect);

				GL.End ();
				GL.PopMatrix();
			
				break;

			case LineMode.Glow:
				GL.PushMatrix ();
				Color color = setColor;
				color.a /= 2f;
				glowMaterial.SetColor("_TintColor", color);
				SetPass(glowMaterial);
				
				GL.Begin(GL.QUADS);

				Max2DMatrix.DrawSliceImage (poly.pointsList, z, connect);

				GL.End();
				GL.PopMatrix();

				break;
		}

		foreach (Polygon2D p in poly.holesList)
			DrawPolygon (p, z);
	}
}