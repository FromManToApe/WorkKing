using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Representation of 2D points
/// </summary>
public class Vector2f {
	public Vector2 vector;

	/// <summary>
	/// Representation of 2D points
	/// </summary>
	public Vector2f(float x, float y)
	{
		vector = new Vector2 (x, y);
	}

	/// <summary>
	/// Representation of 2D points
	/// </summary>
	public Vector2f(Vector2f point)
	{
		vector = new Vector2(point.vector.x, point.vector.y);
	}
		
	/// <summary>
	/// Representation of 2D points
	/// </summary>
	public Vector2f(Vector2 point)
	{
		vector = new Vector2(point.x, point.y);
	}

	public Vector2f Copy()
	{
		return(new Vector2f(vector));
	}
		
	/// <summary>
	/// Set x and y components of an existing 2D vector
	/// </summary>
	public void Set(float x, float y)
	{
		vector.x = x;
		vector.y = y;
	}

	/// <summary>
	/// Set x and y components of an existing 2D vector
	/// </summary>
	public void Set (Vector2f point)
	{
		vector.x = point.vector.x;
		vector.y = point.vector.y;
	}

	/// <summary>
	/// Push 2D vector coordinates by given rotation and distance 
	/// </summary>
	public void Push(float rot, float distance)
	{
		Inc(Mathf.Cos(rot) * distance, Mathf.Sin(rot) * distance);
	}

	/// <summary>
	/// Increase 2D vector coordinates by given x and y coordinates
	/// </summary>
	public void Inc (float x, float y)
	{
		vector.x += x;
		vector.y += y;
	}

	/// <summary>
	/// Decrease 2D vector coordinates by given x and y coordinates
	/// </summary>
	public void Dec (float x, float y)
	{
		vector.x -= x;
		vector.y -= y;
	}

	/// <summary>
	/// Increase 2D vector coordinates by given 2D vector
	/// </summary>
	public void Inc (Vector2f point)
	{
		vector.x += point.vector.x;
		vector.y += point.vector.y;
	}

	/// <summary>
	/// Decrease 2D vector coordinates by given 2D vector
	/// </summary>
	public void Dec (Vector2f point)
	{
		vector.x -= point.vector.x;
		vector.y -= point.vector.y;
	}

	/// <summary>
	/// Distance between given 2D vectors
	/// </summary>
	public static float Distance(Vector2f a, Vector2f b)
	{
		return(Vector2.Distance(a.vector, b.vector));
	}

	/// <summary>
	/// Angle between two given 2D coordinates
	/// </summary>
	public static float Atan2(Vector2f a, Vector2f b)
	{
		return(Mathf.Atan2 (a.vector.y - b.vector.y, a.vector.x - b.vector.x));
	}

	public static float Atan2(Vector2 a, Vector2 b)
	{
		return(Mathf.Atan2 (a.y - b.y, a.x - b.x));
	}

	public static Vector2f RotToVec(float rotation) {
		return(new Vector2f(Mathf.Cos(rotation), Mathf.Sin(rotation)));
	}
}
