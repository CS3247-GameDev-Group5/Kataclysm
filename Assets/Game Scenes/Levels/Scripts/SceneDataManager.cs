using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneDataManager : MonoBehaviour {

	public int level;

	public float killZPlane = -10f;

	public GameObject livesText;
	public GameObject targetScoreText;
	public GameObject centerText;	
	public GameObject timerText;
	public GameObject wavesLeftText;
	public GameObject tutorialText;

	public GameObject[] katzScoreTexts; 

	public GameObject GameUI;
	public GameObject PauseUI;

	public GameObject platformManager;
	public GameObject[] spawnPoints;

	public GameObject leftWallSpawn;
	public GameObject centerWallSpawn;
	public GameObject rightWallSpawn;

	public GameObject modelManager;
	public GameObject wallManager;

	public string nextLevel;

	LevelManager levelManager;

	// Use this for initialization
	void Start () {

		// Init Level Manager
		levelManager = LevelManager.instance;

		// Reset Level Manager
		levelManager.reset ();

		// KillPlanes
		levelManager.killZPlane = this.killZPlane;

		// Text Controllers
		levelManager.livesText = this.livesText.GetComponent<TextController>();
		levelManager.centerText = this.centerText.GetComponent<TextController>();
		levelManager.targetScoreText = this.targetScoreText.GetComponent<TextController> ();
		levelManager.timerText = this.timerText.GetComponent<TextController>();
		levelManager.wavesLeftText = this.wavesLeftText.GetComponent<TextController>();
		if (this.tutorialText != null) {
			levelManager.tutorialText = this.tutorialText.GetComponent<TextController>();
		}

		levelManager.katzScoreTexts = new TextController[4];
		for (int i = 0; i < katzScoreTexts.Length; i++) {
			levelManager.katzScoreTexts [i] = this.katzScoreTexts [i].GetComponent<TextController> ();
		}

		//UIs
		levelManager.GameUI = this.GameUI;
		levelManager.PauseUI = this.PauseUI;

		// Game Objects
		levelManager.platformManagerObject = this.platformManager;
		levelManager.spawnPoints = this.spawnPoints;
		levelManager.wallSpawnPoints = new Transform[] {
			leftWallSpawn.transform, centerWallSpawn.transform, rightWallSpawn.transform
		};
		levelManager.wallManagerObject = wallManager;
		levelManager.modelManagerObject = modelManager;

		// Levels
		levelManager.currentLevel = this.level;
		levelManager.nextLevel = this.nextLevel;

		// Start Level Manager
		levelManager.SpinUpLevelManager();
	}

	void Update () {
		
	}
}
