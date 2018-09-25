using System.Collections.Generic;
using UnityEngine;

public class SpriteMesh2D : MonoBehaviour {
	// Should Be Invisible
	public Texture2D texture;
	public Color color;
	public Vector2 scale;
	public Vector2 uvOffset = Vector2.zero;
	public PolygonTriangulator2D.Triangulation triangulation = PolygonTriangulator2D.Triangulation.Advanced;

	private string sortingLayerName;
	private int sortingLayerID;
	private int sortingOrder;

	private bool added = false;

	void Update () {
		if (added == false) {
			SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer> ();
			if (spriteRenderer == null) {
				if (texture != null) {
					added = true;

					Polygon2D.CreateFromCollider (gameObject).CreateMesh (gameObject, scale, uvOffset, triangulation);

					MeshRenderer meshRenderer = GetComponent<MeshRenderer> ();
					if (meshRenderer == null)
						meshRenderer = gameObject.AddComponent<MeshRenderer> ();
					
					meshRenderer.material = new Material (Shader.Find ("Sprites/Default"));
					meshRenderer.material.mainTexture = texture;
					meshRenderer.material.color = color;

					meshRenderer.sortingLayerName = sortingLayerName;
					meshRenderer.sortingLayerID = sortingLayerID;
					meshRenderer.sortingOrder = sortingOrder;
				}
			} else {
				texture = spriteRenderer.sprite.texture;
				color = spriteRenderer.color;

				float spriteSheetU = (float)(spriteRenderer.sprite.texture.width) / spriteRenderer.sprite.rect.width;
				float spriteSheetV = (float)(spriteRenderer.sprite.texture.height) / spriteRenderer.sprite.rect.height;

				float offsetX = (float)spriteRenderer.sprite.rect.x / spriteRenderer.sprite.texture.width;
				float offsetY = (float)spriteRenderer.sprite.rect.y / spriteRenderer.sprite.texture.height;
				
				float offsetU = ((float)spriteRenderer.sprite.rect.width / spriteRenderer.sprite.texture.width) / 2;
				float offsetV = ((float)spriteRenderer.sprite.rect.height / spriteRenderer.sprite.texture.height) / 2;

				scale = new Vector2(spriteSheetU * spriteRenderer.sprite.rect.width / spriteRenderer.sprite.pixelsPerUnit, spriteSheetV * spriteRenderer.sprite.rect.height / spriteRenderer.sprite.pixelsPerUnit);
				uvOffset = new Vector2(offsetX + offsetU - 0.5f, offsetY + offsetV - 0.5f);
				
				sortingLayerID = spriteRenderer.sortingLayerID;
				sortingLayerName = spriteRenderer.sortingLayerName;
				sortingOrder = spriteRenderer.sortingOrder;

				Destroy (spriteRenderer);
			}
		}
	}

}
