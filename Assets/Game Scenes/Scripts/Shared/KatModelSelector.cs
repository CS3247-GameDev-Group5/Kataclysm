using UnityEngine;
using System.Collections.Generic;

public class KatModelSelector : MonoBehaviour {

	public GameObject spawnPoint;
	public bool isReady = false;
	public int playerNumber = 0;

	Color[] colorCodes = {Color.cyan, Color.red, Color.green, Color.magenta};

	GameObject currentObject;
	int currentModel;
	ModelManager manager;
	private bool isAxisInUse = false;
	InputDevice storedInput;

	// Use this for initialization
	void Start () {
		manager = GameObject.FindObjectOfType<ModelManager>();
		if(manager == null) {
			Debug.LogError("Please add a ModelManager into the scene.");
		}
		currentModel = Random.Range (0, manager.getModelCount());

		// Default SpawnPoint if none
		if (spawnPoint == null) {
			spawnPoint = this.transform.gameObject;
		}

		// Spawn Random Kat
		currentObject = Instantiate (manager.getModelWithCollider((ModelManager.Shapes)currentModel), spawnPoint.transform.position,
			spawnPoint.transform.rotation);
		currentObject.transform.parent = this.transform;
		ChangeColor (currentObject, playerNumber);
	}
	
	// Update is called once per frame
	void Update () {
		// Horizontal Axis - Cycle through cat models
		if( storedInput.GetAxisRaw(MappedAxis.MenuHorizontal) < 0)
		{
			if(isAxisInUse == false)
			{
				// Call your event function here.
				isAxisInUse = true;
				getPrevKat ();
			}
		}
		if( storedInput.GetAxisRaw(MappedAxis.MenuHorizontal) > 0)
		{
			if(isAxisInUse == false)
			{
				// Call your event function here.
				isAxisInUse = true;
				getNextKat ();
			}
		}
		if( storedInput.GetAxisRaw(MappedAxis.MenuHorizontal) == 0)
		{
			isAxisInUse = false;
		}
	}

	public void setVirtualInputController(InputDevice input) {
		storedInput = input;
	}

	// Return current model index (Used with ModelManager)
	public int getCurrentModel() {
		return currentModel;
	}

	void getPrevKat() {
		// Check if is ready
		if (isReady) {
			return;
		}
		// Decrement Pointer
		currentModel -= 1;
		if (currentModel < 0) {
			currentModel = manager.getModelCount() - 1;
		}
		//reset position
		transform.position = spawnPoint.transform.position;
		// Replace object
		GameObject newObject = Instantiate (manager.getModelWithCollider((ModelManager.Shapes)currentModel), currentObject.transform.parent.position,
			currentObject.transform.parent.rotation);
		newObject.transform.parent = this.transform;
		Destroy (currentObject);
		currentObject = newObject;
		ChangeColor (currentObject, playerNumber);
	}

	void getNextKat() {
		// Check if is ready
		if (isReady) {
			return;
		}
		// Increment Pointer
		currentModel += 1;
		if (currentModel >= manager.getModelCount()) {
			currentModel = 0;
		}
		//reset position
		transform.position = spawnPoint.transform.position;
		// Replace object
		GameObject newObject = Instantiate (manager.getModelWithCollider((ModelManager.Shapes)currentModel), currentObject.transform.parent.position,
			currentObject.transform.parent.rotation);
		newObject.transform.parent = this.transform;
		Destroy (currentObject);
		currentObject = newObject;
		ChangeColor (currentObject, playerNumber);
	}

	void ChangeColor(GameObject obj, int playerNumber) {
		List<Renderer> rendererList = GetRenderers (obj);
		foreach (Renderer render in rendererList) {
			render.material.color = colorCodes [playerNumber];
		}
	}

	List<Renderer> GetRenderers(GameObject obj) {
		List<Renderer> rendererList = new List<Renderer>();
		foreach (Renderer objectRenderer in obj.GetComponentsInChildren<Renderer>()) {
			rendererList.Add(objectRenderer);
		}
		return rendererList;
	}
}
