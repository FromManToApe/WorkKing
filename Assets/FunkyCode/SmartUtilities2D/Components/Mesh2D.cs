using UnityEngine;

[ExecuteInEditMode]
public class Mesh2D : MonoBehaviour {
	public PolygonTriangulator2D.Triangulation triangulation = PolygonTriangulator2D.Triangulation.Advanced;

	// Optionable material
	public Material material;

	public string sortingLayerName; 
	public int sortingLayerID;
	public int sortingOrder;

	void Start () {
		if (GetComponents<Mesh2D>().Length > 1) {
			Debug.LogError("SmartSlicer2D error: Multiple 'Mesh2D' components cannot be attached to the same game object");
			return;
		}

		if (GetComponent<Slicer2D>() != null) {
			Debug.LogError("SmartSlicer2D error: 'Mesh2D' and Slicer2D components cannot be attached to the same game object");
			return;
		}

		// Generate Mesh from collider
		Polygon2D.CreateFromCollider (gameObject).CreateMesh(gameObject, Vector2.zero, Vector2.zero, triangulation);

		// Setting Mesh material
		if (material != null) {
			MeshRenderer meshRenderer = GetComponent<MeshRenderer> ();
			meshRenderer.material = material;
		
			meshRenderer.sortingLayerName = sortingLayerName;
			meshRenderer.sortingLayerID = sortingLayerID;
			meshRenderer.sortingOrder = sortingOrder;
		}
	}
}
