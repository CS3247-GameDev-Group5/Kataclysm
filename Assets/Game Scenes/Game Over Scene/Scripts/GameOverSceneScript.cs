using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSceneScript : MonoBehaviour {

	public GameObject gameOverTextObject;
	public Vector3 textPosition;
	public Quaternion textRotation;
	public Canvas canvas;
	public float delay;
	public GameObject fadeOutObject;
	
	string lastGameScene;
	GameObject levelManager;


	// Use this for initialization
	void Start () {

		levelManager = GameObject.Find("LevelManager");
		if(levelManager != null) {
			levelManager.SetActive(false);
		}
		StartCoroutine (MoveGameOverTextIn ());
	}

	void resetForLobby() {
		// Destroy persistent objects
		try {
			Destroy(levelManager);
		} catch {
			// Do nothing
		}
	}

	void resetForReplay() {
			lastGameScene = levelManager.GetComponent<LevelManager>().currentScene;
			var scores = levelManager.GetComponent<LevelManager>().getLevelScoreArray();
			var totalScores = levelManager.GetComponent<LevelManager>().getCumulativeScoreArray();
			//reverse totalScore
			for(int i = 0; i < scores.Length; i++) {
				Debug.Log("1: " + scores[i] + " Total: " + totalScores[i]);
			}
			levelManager.SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {		
		foreach (var input in VirtualController.inputDevices) {
			if (input.GetButtonDown(MappedButton.MenuAffirmitive)) {
				print("Pressed A!");
				resetForReplay();
				LoadNextStage (lastGameScene);
				break;
			} else if(input.GetButtonDown(MappedButton.MenuNegative)) {
				print("Pressed B!");
				resetForLobby();
				LoadNextStage ("Lobby");
				break;
			}
		}
	}
		
	void instantiateTextObject() {
		GameObject textObj = Instantiate (gameOverTextObject) as GameObject;
		textObj.transform.SetParent(canvas.transform);
		textObj.transform.localPosition = textPosition;
		textObj.transform.localRotation = textRotation;
	}

	IEnumerator MoveGameOverTextIn() {
		yield return new WaitForSeconds(delay);
		instantiateTextObject ();
	}
		
	IEnumerator FadeToScene(float delay, string scene) {
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene (scene);
	}

	public void LoadNextStage(string scene) {
		Instantiate(fadeOutObject);
		StartCoroutine (FadeToScene (1.0f, scene));
	}
		
}
