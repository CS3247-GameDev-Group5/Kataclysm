using UnityEngine;

public class PlayerTestLoader : MonoBehaviour {

	public GameObject player;
	public GameObject modelManagerGameObject;
	public int modelId = 0;
	public int virtualControllerId = 0;

	ModelManager modelManager;
	// Use this for initialization
	void Start () {		
		modelManager = modelManagerGameObject.GetComponent<ModelManager>();
		/// Spawn Model
		GameObject playerKat = Instantiate (player, transform.position, transform.rotation);
		ModelEntity modelEntity = playerKat.GetComponent<ModelEntity> ();
		modelEntity.setModel (modelManager.getModelWithCollider ((ModelManager.Shapes)modelId));
		// Configure player controller
		PlayerController controller = playerKat.GetComponent<PlayerController> ();
		controller.storeDevice(VirtualController.inputDevices[virtualControllerId]);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject getPlayer()	{
		return player;
	}

	public string hello()	{
		return "HELLO";
	}
}
