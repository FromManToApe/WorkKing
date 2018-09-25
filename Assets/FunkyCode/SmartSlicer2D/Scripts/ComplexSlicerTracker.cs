using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ComplexSlicerTracker {
	public List<ComplexSlicerTrackerObject> trackerList = new List<ComplexSlicerTrackerObject>();

	public void Update(Vector2 position, float minVertsDistance = 1f) {
		foreach(Slicer2D slicer in Slicer2D.GetList()) {
			ComplexSlicerTrackerObject tracker = GetSlicerTracker(slicer);
			if (tracker == null) {
				tracker = new ComplexSlicerTrackerObject();
				tracker.slicer = slicer;
				trackerList.Add(tracker);
			}

		//	Debug.Log(trackerList.Count);
			Vector2f trackedPos = new Vector2f(slicer.transform.transform.InverseTransformPoint(position));
			if (tracker.lastPosition != null) {
				if (slicer.polygon.PointInPoly(trackedPos)) {
					if (tracker.tracking == false) {
						tracker.pointsList.Add(tracker.lastPosition);
					}

					tracker.tracking = true;

					if ((Vector2f.Distance (trackedPos, tracker.pointsList.Last ()) > minVertsDistance / 4f)) {
						tracker.pointsList.Add(trackedPos);
					}

				} else if (tracker.tracking == true) {
					tracker.tracking = false;
					tracker.pointsList.Add(trackedPos);

					foreach(Vector2f point in tracker.pointsList) {
						point.vector = slicer.transform.TransformPoint(point.vector);
					}

					slicer.ComplexSlice(tracker.pointsList);
					trackerList.Remove(tracker);
				}
			}

			if (tracker != null) {
				tracker.lastPosition = trackedPos;
			}
		}
	}

	public ComplexSlicerTrackerObject GetSlicerTracker( Slicer2D slicer) {
		foreach(ComplexSlicerTrackerObject tracker in trackerList) {
			if (tracker.slicer == slicer) {
				return(tracker);
			}
		}
		return(null);
	}
}

public class ComplexSlicerTrackerObject {
	public Slicer2D slicer;
	public Vector2f lastPosition;
	public List<Vector2f> pointsList = new List<Vector2f>();
	public bool tracking = false;
}