using UnityEngine;
//ModelEntity requires a BoxCollider componet in the owner.
public class ModelEntity : MonoBehaviour {
	
	GameObject self;

	public void setModel(GameObject model) {
		if(self != null) {
			Destroy(self);
		}
		self = Instantiate(model, transform.position, transform.rotation) as GameObject;
		self.transform.parent = transform;		
		self.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);}
}
