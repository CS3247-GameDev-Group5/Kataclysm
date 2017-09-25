using UnityEngine;

public class ModelManager : MonoBehaviour {

	public GameObject[] modelsWithCollider;
	public enum Shapes {Cube, Sphere, Cylinder};

	public GameObject getModelWithCollider(Shapes modelEnum) {
		return modelsWithCollider[(int)modelEnum];
	}
	public int getModelCount() {
		return modelsWithCollider.Length;
	}
}
