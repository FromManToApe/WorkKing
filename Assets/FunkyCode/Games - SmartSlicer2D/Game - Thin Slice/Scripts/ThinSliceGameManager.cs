using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ThinSliceGameManager : MonoBehaviour {
	private float startingArea = 0; // Original Size Of The Map

	public float leftArea = 100;	// Percents Of Map Left
	public Text text;				// UI Text Object

	static public ThinSliceGameManager instance;

	void Start () {
		foreach(Slicer2D slicer in  Slicer2D.GetList()) {
			startingArea += Polygon2D.CreateFromCollider(slicer.gameObject).ToWorldSpace(slicer.transform).GetArea();
		}
		instance = this;
	}

	// Recalculate area that is left
	public void UpdateText() {
		leftArea = 0;
		foreach(Slicer2D slicer in Slicer2D.GetList()) {
			Polygon2D poly = Polygon2D.CreateFromCollider(slicer.gameObject);

			leftArea += poly.ToWorldSpace(slicer.gameObject.transform).GetArea();
		}

		leftArea = ((leftArea) / startingArea) * 100f;
		text.text = "Left: " + (int)leftArea + "%";
	}

	static public void CreateParticles() {
		Max2DParticles.CreateSliceParticles(Slicer2DController.complexSlicerPointsList);

		float size = 0.5f;
		Vector2 f = Slicer2DController.complexSlicerPointsList.First().vector;
		f.x -= size / 2;
		f.y -= size / 2;

		List<Vector2f> list = new List<Vector2f>();
		list.Add( new Vector2f (f.x, f.y));
		list.Add( new Vector2f (f.x + size, f.y));
		list.Add( new Vector2f (f.x + size, f.y + size));
		list.Add( new Vector2f (f.x, f.y + size));
		list.Add( new Vector2f (f.x, f.y));
		Max2DParticles.CreateSliceParticles(list).stripped = false;

		f = Slicer2DController.complexSlicerPointsList.Last().vector;
		f.x -= size / 2;
		f.y -= size / 2;

		list = new List<Vector2f>();
		list.Add( new Vector2f (f.x, f.y));
		list.Add( new Vector2f (f.x + size, f.y));
		list.Add( new Vector2f (f.x + size, f.y + size));
		list.Add( new Vector2f (f.x, f.y + size));
		list.Add( new Vector2f (f.x, f.y));
		Max2DParticles.CreateSliceParticles(list).stripped = false;
	}
}