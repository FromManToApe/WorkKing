﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Max2DParticles : MonoBehaviour {
	public List<LineParticle> lines = new List<LineParticle>();
	public bool stripped = true;

	public static Max2DParticles CreateSliceParticles (List<Vector2f> slice) {
		GameObject newGameObject = new GameObject();
		newGameObject.name = "Particles";
		Max2DParticles particles = newGameObject.AddComponent<Max2DParticles>();

		foreach(Pair2f line in Pair2f.GetList(slice, false)) {
			particles.lines.Add(new LineParticle(line));
		}

		newGameObject.AddComponent<DestroyTimer>();

		return(particles);
	}
	
	public void OnRenderObject() {

		if (stripped) {
			foreach(LineParticle line in lines) {
				line.Update();
				List<Vector2f> list = new List<Vector2f>();
				list.Add(line.GetPair().A);
				list.Add(line.GetPair().B);
				Max2D.DrawStrippedLine (list, 1, -2);
			}
		} else {
			foreach(LineParticle line in lines) {
				line.Update();
				List<Vector2f> list = new List<Vector2f>();
				list.Add(line.GetPair().A);
				list.Add(line.GetPair().B);
				Max2D.DrawLine(line.GetPair().A, line.GetPair().B, -2);
			}
		}
	}
}

public class LineParticle {
	float direction;
	float length;
	Vector2 position;

	Vector2 velocity;
	float angularVelocity = 0;

	public LineParticle(Pair2f pair) {
		length = Vector2f.Distance(pair.A, pair.B) / 2;
		direction = Vector2f.Atan2(pair.A, pair.B);
		position = (pair.A.vector + pair.B.vector) / 2f;

		velocity = new Vector2(Random.Range(-0.1f, 0.1f), 0.1f);
		angularVelocity = Random.Range(-1, 1);
	}

	public Pair2f GetPair() {
		Vector2f A = new Vector2f(position.x + Mathf.Cos(direction) * length, position.y + Mathf.Sin(direction) * length);
		Vector2f B = new Vector2f(position.x + Mathf.Cos(direction) * -length, position.y + Mathf.Sin(direction) * -length);
		return(new Pair2f(A, B));
	}

	public void Update() {
		position += velocity * 0.2f;
		velocity.y += -0.02f;
		direction += angularVelocity / (180.0f / Mathf.PI);
	}
}
