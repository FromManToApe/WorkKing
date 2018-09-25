using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class ComplexSlicerExtended {
	
	// Can Be Used In Advanced SliceInside
	static public Polygon2D CreateSlice(List<Vector2f> slice) 
	{
		if (slice.Count () > 2 && MathHelper.SliceIntersectItself(slice) == false)
			return(new Polygon2D (slice));

		return(null);
	}

	// Polygon Slice - TODO: Return No Polygon if it's eaten by polygon slice
	static public Slice2D Slice(Polygon2D polygon, Polygon2D polygonSlice)
	{
		Slice2D result = Slice2D.Create (polygon);
		Slicer2D.SliceType tempSliceType = Slicer2D.complexSliceType;
		Slicer2D.complexSliceType = Slicer2D.SliceType.SliceHole;

		polygonSlice.Normalize ();
		polygon.Normalize ();

		// Eat a polygon completely
		// Complex Slicer does not register slice in this case
		if (polygonSlice.PolyInPoly (polygon) == true) {
			result.AddPolygon (polygon);
			return(result);
		}

		if (polygon.PolyInPoly (polygonSlice) == true) {
			polygon.AddHole (polygonSlice);
			result.AddPolygon (polygon);
			return(result);
		}

		// Act as Regular Slice
		Vector2f startPoint = null;

		foreach (Vector2f id in polygonSlice.pointsList)
			if (polygon.PointInPoly (id) == false) {
				startPoint = id;
				break;
			}


		if (startPoint == null) {
			Debug.LogError ("Slicer2D: Starting Point Error In PolygonSlice");
			return(result);
		}

		polygonSlice.pointsList = VectorList2f.GetListStartingPoint (polygonSlice.pointsList, startPoint);
	
		/*
		List<Vector2f> s = new List<Vector2f> ();
		foreach (Pair2f pair in Pair2f.GetList(polygonSlice.pointsList, false)) {
			List<Vector2f> stackList = polygon.GetListSliceIntersectPoly(pair);
			stackList = VectorList2f.GetListSortedToPoint (stackList, pair.A);
			Vector2f old = pair.A;
			s.Add (old);

			foreach (Vector2f id in stackList) {
				s.Add (new Vector2f((old.GetX() + id.GetX()) / 2, (old.GetY() + id.GetY()) / 2));
				old = id;
			}
		}

		polygonSlice.pointsList = s;
		*/

		polygonSlice.AddPoint (startPoint);

		// Not necessary
		if (polygon.SliceIntersectPoly (polygonSlice.pointsList) == false)
			return(result);
		
		// Slice More Times?
		result = ComplexSlicer.Slice (polygon, new List<Vector2f> (polygonSlice.pointsList));

		if (result.polygons.Count < 1)
			Debug.LogError ("Slicer2D: Returns Empty Polygon Slice");

		Slicer2D.complexSliceType = tempSliceType;

		return(result);
	}

	static public Slice2D LinearCutSlice(Polygon2D polygon, LinearCut linearCut) {
		List<Vector2f> slice = linearCut.GetPointsList();
		Slice2D result = Slice2D.Create (linearCut);

		Vector2f startPoint = null;
		foreach (Vector2f id in slice)
			if (polygon.PointInPoly (id) == false) {
				startPoint = id;
				break;
			}

		Polygon2D newPolygon = new Polygon2D(slice);

		slice = VectorList2f.GetListStartingPoint (slice, startPoint);
		slice.Add(startPoint);

		if (polygon.PolyInPoly(newPolygon)) {
			polygon.AddHole(newPolygon);
			result.AddPolygon(polygon);
			return(result);
		}

		result = ComplexSlicer.Slice (polygon, slice);
	
		return(result);
	}
}