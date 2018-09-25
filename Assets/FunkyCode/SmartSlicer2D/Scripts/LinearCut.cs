using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearCut {
	public Pair2f pairCut;
	float size = 1f;

	static public LinearCut Create(Pair2f pair, float size)
	{
		LinearCut cut = new LinearCut();
		cut.size = size;
		cut.pairCut = pair;
		return(cut);
	}

	public List<Vector2f> GetPointsList(float multiplier = 1f){
		if (pairCut == null) {
			Debug.LogError("WTF");
		}

		float rot = Vector2f.Atan2(pairCut.A, pairCut.B);

		Vector2f a = pairCut.A.Copy();
		Vector2f b = pairCut.A.Copy();
		Vector2f c = pairCut.B.Copy();
		Vector2f d = pairCut.B.Copy();

		a.Push(rot + Mathf.PI / 4, size * multiplier);
		b.Push(rot - Mathf.PI / 4, size * multiplier);
		c.Push(rot + Mathf.PI / 4 + Mathf.PI, size * multiplier);
		d.Push(rot - Mathf.PI / 4 + Mathf.PI, size * multiplier);

		List<Vector2f> result = new List<Vector2f>();
		result.Add(a);
		result.Add(b);
		result.Add(c);
		result.Add(d);
		//result.Add(a);

		return(result);
	}
}
