using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Slicer2D))]
public class Slicer2DEditor : Editor
{
	override public void OnInspectorGUI()
	{
		Slicer2D script = target as Slicer2D;

		script.textureType = (Slicer2D.TextureType)EditorGUILayout.EnumPopup ("Texture Type", script.textureType);
		script.triangulation = (PolygonTriangulator2D.Triangulation)EditorGUILayout.EnumPopup ("Triangulation", script.triangulation);
		
		if (script.textureType == Slicer2D.TextureType.Mesh) {
			script.material = (Material)EditorGUILayout.ObjectField("Material",script.material, typeof(Material), true);
		}

		script.slicingLayer = (Slicer2D.SlicingLayer)EditorGUILayout.EnumPopup ("Slicing Layer", script.slicingLayer);
		script.slicingLimit = GUILayout.Toggle(script.slicingLimit, "Slicing Limit");

		if (script.slicingLimit) {
			script.maxSlices = EditorGUILayout.IntSlider("Max Slices", script.maxSlices, 1, 10);
		}

		script.recalculateMass = GUILayout.Toggle(script.recalculateMass, "Recalculate Mass");
	}
}