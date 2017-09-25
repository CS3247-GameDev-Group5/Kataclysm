using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class PlayButtonPress : MonoBehaviour {

	public GameObject fadeOutObject;
	public Button playButton;

	void Start() {
		playButton = GetComponent<Button> ();
	}

	void Update() {
		foreach (var input in VirtualController.inputDevices) {
			if (input.GetButtonDown(MappedButton.Start)) {
				playButton.onClick.Invoke ();
				break;
			}
		}
	}

	IEnumerator FadeToScene(float delay, string scene) {
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene (scene);
	}

	IEnumerator FadeToScene(float delay, int scene) {
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene (scene);
	}

	public void LoadNextStage() {
		Instantiate(fadeOutObject);
		StartCoroutine (FadeToScene (1.0f, "Lobby"));
	}
}

