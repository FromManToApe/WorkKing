using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceroidsAsteroids : MonoBehaviour {
	float spawnTimer;

	void Start () {
		CreateAsteroid();
		CreateAsteroid();
		CreateAsteroid();
		CreateAsteroid();

	}

	void Update() {

	}

	void CreateAsteroid() {
		GameObject newGameObject = new GameObject();
		newGameObject.name = "Aseteroid";

		float ratio = (float)Screen.width / Screen.height;
		newGameObject.transform.position = new Vector2(Random.Range(-Camera.main.orthographicSize * ratio, Camera.main.orthographicSize * ratio), Camera.main.orthographicSize);

		Polygon2D polygon = new Polygon2D();

		float rot = 0;

		while (rot < 360) {
			float dist = Random.Range(50, 100);
			polygon.AddPoint(Mathf.Cos(Mathf.Deg2Rad * rot) * dist / 100f, Mathf.Sin(Mathf.Deg2Rad * rot) * dist / 100f);
			rot += Random.Range(10, 45);
		}

		polygon.CreateCollider(newGameObject).isTrigger = true;
  
		//newGameObject.AddComponent<SliceroidsKeepInScreen>();

		SliceroidsColliderLineRenderer2D lineRenderer = newGameObject.AddComponent<SliceroidsColliderLineRenderer2D>();
		lineRenderer.color = new Color(56f / 255,1,239f / 255, 1f);

		newGameObject.AddComponent<Rigidbody2D>().AddForce(Vector2f.RotToVec(Vector2f.Atan2(new Vector2f(Vector2.zero), new Vector2f(newGameObject.transform.position))).vector * 100);
		newGameObject.AddComponent<Slicer2D>().textureType = Slicer2D.TextureType.None;
	}
}
