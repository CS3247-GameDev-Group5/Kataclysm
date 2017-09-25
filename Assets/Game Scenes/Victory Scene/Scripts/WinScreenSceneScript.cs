using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenSceneScript : MonoBehaviour {

	public GameObject winTextObject;
	public Vector3 textPosition;
	public Quaternion textRotation;
	public Canvas canvas;
	public float delay;
	public GameObject fadeOutObject;
	public GameObject FallingKats;
	public AudioClip winningMeowSound;

	Color[] colorCodes = {Color.cyan, Color.red, Color.green, Color.magenta};
	AudioSource winningMeow;
	// Use this for initialization
	void Start () {
		// Destroy persistent objects
		try {
			GameObject lobbyManager = GameObject.Find("LobbyManager");
			Destroy(lobbyManager);
		} catch {
			// Do nothing
		}
		try {
			GameObject levelManagerObject = GameObject.Find("LevelManager");
			LevelManager levelManager = levelManagerObject.GetComponent<LevelManager>();
			// Extract data from level manager here

			// Destroy after use
			Destroy(levelManagerObject);
		} catch {
			// Do nothing
		}
		// Winning Meow~
		winningMeow = this.gameObject.AddComponent<AudioSource>();
		winningMeow.clip = winningMeowSound;
		winningMeow.loop = true;
		winningMeow.Play ();

		StartCoroutine (MoveWinTextIn ());
	}
	
	// Update is called once per frame
	void Update () {
		//foreach (var input in VirtualController.inputDevices) {
		if (Input.anyKeyDown) {
			//print("Pressed Start!");
			winningMeow.Stop();
			LoadNextStage ();
			//break;
		}
		//}
	}
		
	void activateTextObject() {
		winTextObject.SetActive(true);
	}

	IEnumerator MoveWinTextIn() {
		yield return new WaitForSeconds(delay);
		activateTextObject ();
	}
		
	IEnumerator FadeToScene(float delay, string scene) {
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene (scene);
	}

	public void LoadNextStage() {
		Instantiate(fadeOutObject);
		StartCoroutine (FadeToScene (1.0f, "Lobby"));
	}
		
}
