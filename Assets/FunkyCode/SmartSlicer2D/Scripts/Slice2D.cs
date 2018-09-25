using System.Collections.Generic;
using UnityEngine;

public class Slice2D {
	public enum SliceType {Undefined, Linear, Complex, Polygon, Exploding, ExplodingPoint, Point, LinearCut};
	
	public SliceType sliceType = SliceType.Undefined;
	public List<Vector2f> slice = new List<Vector2f>();
	public List<Vector2f> collisions = new List<Vector2f>();
	public List<Polygon2D> polygons = new List<Polygon2D>();
	public List<GameObject> gameObjects = new List<GameObject>();

	public void AddCollision(Vector2f point)
	{
		collisions.Add (point);
	}
	
	// Private
	private void AddGameObject(GameObject gameObject)
	{
		gameObjects.Add (gameObject);
	}

	public void AddGameObjects(List<GameObject> gameObjects)
	{
		foreach (GameObject gameObject in gameObjects) 
		{
			AddGameObject (gameObject);
		}
	}

	public void AddPolygon(Polygon2D polygon)
	{
		polygons.Add (polygon);
	}

	public void RemovePolygon(Polygon2D polygon)
	{
		polygons.Remove (polygon);
	}

	// Complex Slice
	public static Slice2D Create(List<Vector2f> newSlice)
	{
		Slice2D slice2D = Create(SliceType.Complex);
		slice2D.slice = new List<Vector2f>(newSlice);
		return(slice2D);
	}

	// Linear Slice
	public static Slice2D Create(Pair2f newSlice)
	{
		Slice2D slice2D = Create(SliceType.Linear);
		slice2D.slice = new List<Vector2f>();
		slice2D.slice.Add(newSlice.A);
		slice2D.slice.Add(newSlice.B);
		return(slice2D);
	}

	// Linear Cut Slice
	public static Slice2D Create(LinearCut newSlice)
	{
		Slice2D slice2D = Create(SliceType.LinearCut);
		slice2D.slice = new List<Vector2f>();
		
		if (newSlice.pairCut != null) {
			slice2D.slice.Add(newSlice.pairCut.A);
			slice2D.slice.Add(newSlice.pairCut.B);
		} else {
			Debug.LogError("Null Linear Cut Slice");
		}
		
		return(slice2D);
	}

	// Point Slice
	public static Slice2D Create(Vector2f point, float rotation)
	{
		Slice2D slice2D = Create(SliceType.Point);
		slice2D.slice = new List<Vector2f>();
		slice2D.slice.Add(point);
		return(slice2D);
	}

	// Polygon Slice
	public static Slice2D Create(Polygon2D slice)
	{
		Slice2D slice2D = Create(SliceType.Polygon);
		slice2D.slice = new List<Vector2f>(slice.pointsList);
		return(slice2D);
	}

	// Exploding Point Slice
	public static Slice2D Create(Vector2f point)
	{
		Slice2D slice2D = Create(SliceType.ExplodingPoint);
		slice2D.slice = new List<Vector2f>();
		slice2D.slice.Add(point);
		return(slice2D);
	}

	public static Slice2D Create(SliceType sliceType)
	{
		Slice2D slice2D = new Slice2D ();
		slice2D.sliceType = sliceType;
		return(slice2D);
	}
}


