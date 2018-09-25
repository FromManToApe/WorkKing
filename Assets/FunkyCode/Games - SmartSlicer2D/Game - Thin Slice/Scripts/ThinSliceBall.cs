using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThinSliceBall : MonoBehaviour {
	private Vector2 direction;
	public float speed = 0.1f;

	void OnEnable() { ballList.Add (this);}
	void OnDisable() { ballList.Remove (this);}
	static private List<ThinSliceBall> ballList = new List<ThinSliceBall>();
	static public List<ThinSliceBall> GetList(){ return(new List<ThinSliceBall>(ballList));}

	void SetDirection(Vector3 newDirection) {
		direction = newDirection;
		direction.Normalize();
	}

	void Start () {
		SetDirection(Random.insideUnitCircle);
	}
	
	// This manages ball movement and collisions with level walls
	void UpdateMovement() {
		transform.Translate(direction * speed);

		// Balls vs Map Collisions
		foreach(Slicer2D slicer in Slicer2D.GetList()) {
			foreach (Pair2f id in Pair2f.GetList(slicer.polygon.ToWorldSpace(slicer.transform).pointsList)) {
				if (MathHelper.LineIntersectCircle(id, new Vector2f(transform.position), 1f) == true) {
					transform.Translate(direction * -speed);
					SetDirection(MathHelper.ReflectAngle(direction, Vector2f.Atan2(id.A, id.B)));
					transform.Translate(direction * speed);
				}
			}
		}

		// Balls vs Balls Collision
		foreach(ThinSliceBall ball in ThinSliceBall.GetList()) {
			if (ball == this) {
				continue;
			}

			if (Vector2.Distance(transform.position, ball.transform.position) < 2) {
				ball.direction = Vector2f.RotToVec(Vector2f.Atan2(transform.position, ball.transform.position)- Mathf.PI).vector;
				direction = Vector2f.RotToVec(Vector2f.Atan2(transform.position, ball.transform.position)).vector;
				
				ball.transform.Translate(ball.direction * ball.speed);
				transform.Translate(direction * speed);
			}
		}
	}

	void UpdateSlicerCollisions() {
		if (MathHelper.SliceIntersectCircle(Slicer2DController.complexSlicerPointsList, new Vector2f(transform.position), 1f )) {
			ThinSliceGameManager.CreateParticles();

			// Remove Current Slicing Process
			Slicer2DController.complexSlicerPointsList.Clear();
		}
	}

	void Update () {
		UpdateMovement();

		UpdateSlicerCollisions();
	}
}
