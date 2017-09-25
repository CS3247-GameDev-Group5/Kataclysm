using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager: MonoBehaviour {

	enum GameState {
		start, prepare, warning, collection, end, pause, inactive
	}

	enum GameType {cooperative, competitive}; 

	// Public variables
	public GameObject levelDataObject;
	public GameObject playerPrefab;

	public int numOfLives = 9;
	public int readyTime = 10; // Seconds
	public int collectionTime = 5; // Seconds
	public float dropTime = 3.0f; // Seconds
	public string gameOverLevel;

	// Displays - SceneDataManager will have to init these
	[HideInInspector]
	public int currentLevel;
	[HideInInspector]
	public float killZPlane;
	[HideInInspector]
	public GameObject wallManagerObject;
	[HideInInspector]
	public GameObject modelManagerObject;
	[HideInInspector]
	public TextController livesText;
	[HideInInspector]
	public TextController centerText;
	[HideInInspector]
	public TextController targetScoreText;
	[HideInInspector]
	public TextController timerText;
	[HideInInspector]
	public TextController wavesLeftText;
	[HideInInspector]
	public TextController tutorialText;
	[HideInInspector]
	public TextController[] katzScoreTexts; 
	[HideInInspector]
	public GameObject GameUI;
	[HideInInspector]
	public GameObject PauseUI;
	[HideInInspector]
	public GameObject platformManagerObject;
	[HideInInspector]
	public GameObject[] spawnPoints;
	[HideInInspector]
	public Transform[] wallSpawnPoints; // LCR format
	[HideInInspector]
	public string nextLevel;
	public AudioClip cheer1;

	// Initialised at runtime data
	LobbyManager lobbyManager;
	WallManager wallManager;
	Level currentLevelData;
	ModelManager modelManager;
	PlatformManager platformManager;

	int numberOfPlayers = 0;
	int[] playerModels;
	InputDevice[] playerControls;

	// Member Variables
	GameObject[] playerKats;
	GameState currentGameState = GameState.start;
	int gameTime = 0;
	int currentSequence = 0;
	int totalSequences = 0;
	int currentScore = 0;
	int targetScore = 0;
	int[] playersScore; 
	int[] cumulativePlayersScore; 
	int nextGameTime = 0;
	bool isGameOver = false;
	bool isTutorial = false;
	GameState previousGameState;
	Color[] colorCodes = {Color.cyan, Color.red, Color.green, Color.magenta};

	// Drop Time
	int collectionPhase = 1;
	bool startedWait = false;

	// End Phase
	bool canSwitchScene = false;
	int endPhase = 1;
	[HideInInspector]
	public string currentScene;

	AudioSource cheer;

	[HideInInspector]
	public static LevelManager instance {
		get; private set;
	}
	// Persistent
	void Awake() {
		if(instance == null) {
			instance = this;
			cheer = gameObject.AddComponent<AudioSource>();
			cheer.playOnAwake = false;
			DontDestroyOnLoad(gameObject);
		}
		else Destroy(gameObject);
	}

	// Called by scene data manager to Start level manager operations
	public void SpinUpLevelManager () {
		if (numberOfPlayers == 0) {
			initialiseFromLobby ();
		}
		if (tutorialText != null) {
			isTutorial = true;
		}
		prepareLevel (currentLevel);
		//updateLivesDisplay ();
		updateScoreDisplay();
		updateTargetScoreDisplay ();
		updateWavesLeftDisplay ();
		startLevelTimer ();
		currentScene = SceneManager.GetActiveScene().name;
	}

	// Main Update Loop
	void Update () {

		// Perform state-related activities
		switch (currentGameState) {

		case GameState.start:
			// Wait for 10 seconds? 
			if (gameTime >= readyTime) {
				
				// Spawn Walls
				wallManager.spawnWall(0, currentLevelData.wallSeqeunceL[currentSequence]);
				wallManager.spawnWall(1, currentLevelData.wallSeqeunceC[currentSequence]);
				wallManager.spawnWall(2, currentLevelData.wallSeqeunceR[currentSequence]);

				// Platform start splitting (for the first sequence)
				if (currentLevelData.splittingPlatform && platformManager != null) {
					int circularIndex = getIndexCircular(currentSequence, currentLevelData.splittingToggle.Length);
					if (currentLevelData.splittingToggle [circularIndex]) {
						// Toggle splitting
						updateCenterTextDisplay ("CAREFUL!", 1.5f); // Include scale out + fade out anim
						platformManager.toggleSplitting ();
					}
				} else {
					updateCenterTextDisplay ("Prepare!", 1.5f); // Include scale out + fade out anim
				}

				// Transit to next state
				nextGameTime = gameTime + currentLevelData.prepareTime [currentSequence];
				currentGameState = GameState.prepare;

				// Show Tutorial Text During the 10secs of preparation
				if (isTutorial) {
					tutorialText.toggleAnimation();
					updateTutorialTextDisplay (currentLevelData.texts[currentSequence], 10.0f);
				}
			}
			break;

		case GameState.prepare:
			if (gameTime >= nextGameTime) {
				updateCenterTextDisplay ("Warning!", 1.5f); // Include screen red tint + fade out anim
				int circularIndex = getIndexCircular(currentSequence, currentLevelData.wallSpeed.Length);

				// Move Walls
				wallManager.moveWall (0, currentLevelData.wallSpeed [circularIndex], killZPlane);
				wallManager.moveWall (1, currentLevelData.wallSpeed [circularIndex], killZPlane);
				wallManager.moveWall (2, currentLevelData.wallSpeed [circularIndex], killZPlane);

				// stop particleEffects from looping
				wallManager.stopAllParticleEffects();

				// Modify Platform
				if (currentLevelData.rotatingPlatform && platformManager != null) {
					circularIndex = getIndexCircular(currentSequence, currentLevelData.rotateToggle.Length);
					if (currentLevelData.rotateToggle [circularIndex]) {
						// Toggle Rotating
						platformManager.toggleRotation();
					}
				}

				// Transit to next state
				currentGameState = GameState.warning;
			}
			break;

		case GameState.warning:
			
			if (wallManager.allWallsPassed () && collectionPhase == 1) {
				

				if (!startedWait) {
					startedWait = true;

					// Remove particleEffects
					wallManager.destroyAllParticleEffects ();
					// Start timer for wait
					StartCoroutine ("waitForDropTime");
				}
			}
			else if (wallManager.allWallsPassed() && collectionPhase == 2) {

				// Collate survivors
				int survivors = 0;
				for (int i = 0; i < numberOfPlayers; i++) {
					if (playerKats[i].activeInHierarchy) {
						survivors += 1;
						currentScore += 10;
						playersScore[i] += 10; 
						updateKatzScoreDisplay (i, "Katz\n" + playersScore [i]);
						// keep the controls of the control, model
					} 
				}
				updateScoreDisplay ();
				updateTargetScoreDisplay ();
				/*
				foreach (GameObject kat in playerKats) {
					// Depending on final implementation, counting survivors can be modified
					if (kat.activeInHierarchy) {
						survivors += 1;
						// keep the controls of the control, model
					} 
				}
				*/

				// Display aftermath
				if (survivors == numberOfPlayers) {
					cheer.clip = cheer1;
					cheer.Play();
					updateCenterTextDisplay ("All Kats survived!", 2.0f);
				} else if (survivors != 0) {
					updateCenterTextDisplay (survivors + " Kats survived!", 2.0f);
				} else {
					updateCenterTextDisplay ("No Kats \nsurvived...", 2.0f);
				}

				// Transit to next state
				nextGameTime = gameTime + collectionTime;
				currentGameState = GameState.collection;
			}


			break;

		case GameState.collection:
			//print ("next Game Time: " + nextGameTime);
			//print ("current game time: " + gameTime);
			if (gameTime >= nextGameTime) {
				// Else update to next sequence
				currentSequence += 1;
				updateWavesLeftDisplay ();
				print (currentSequence + ", " + totalSequences);

				// Reset Collection Phase
				startedWait = false;
				collectionPhase = 1;

				/*
				if (numOfLives <= 0) {
					// go game over screen
					isGameOver = true;
					updateCenterTextDisplay ("Game Over", 2.0f);
					currentGameState = GameState.end;
					break;
				} else if (currentSequence > totalSequences - 1) {
					Debug.Log ("All sequences complete");
					updateCenterTextDisplay ("Level Passed! \nYou win!", 2.0f);
					currentGameState = GameState.end;
					break;
				}
				*/

				if (currentSequence > totalSequences - 1) {
					if (currentScore >= targetScore) {
						updateCenterTextDisplay ("Level Passed! \nYou win!", 3.0f);
						currentGameState = GameState.end;
						break;	
					} else {
						isGameOver = true;
						updateCenterTextDisplay ("Game Over", 3.0f);
						currentGameState = GameState.end;
						break;	
					}
				}

				// Else prepare next sequence
				int circularIndex = getIndexCircular (currentSequence, currentLevelData.prepareTime.Length);
				nextGameTime = gameTime + currentLevelData.prepareTime [circularIndex];
				//updateCenterTextDisplay ("Prepare!", 1.0f); // Include scale out + fade out anim

				// Platform start splitting
				if (currentLevelData.splittingPlatform && platformManager != null) {
					circularIndex = getIndexCircular(currentSequence, currentLevelData.splittingToggle.Length);
					if (currentLevelData.splittingToggle [circularIndex]) {
						// Toggle splitting
						updateCenterTextDisplay ("CAREFUL!", 1.5f); // Include scale out + fade out anim
						platformManager.toggleSplitting ();
					}
				} else {
					updateCenterTextDisplay ("Prepare!", 1.5f); // Include scale out + fade out anim
				}

				// loop through playerKats, check for active, 
				// for any inactive kats, make active and move to a random spawnpoint
				var list = getRandomizeSpawnList();
				for (int i = 0; i < numberOfPlayers; i++) {
					if (!playerKats [i].activeInHierarchy) {
						Transform spawnPoint = list[i].transform;
						playerKats [i].SetActive (true);

						Rigidbody rb = playerKats [i].GetComponent<Rigidbody> ();
						rb.ResetInertiaTensor ();
						rb.velocity = Vector3.zero;
						rb.angularVelocity = Vector3.zero;

						playerKats [i].transform.position = spawnPoint.position;
						playerKats [i].transform.rotation = spawnPoint.rotation;

						// Moved to NewDeathPlane as callback
						// reduceLife ();
					}
				}

				// Respawn Kats: TODO: Depending on the final respawning method this can be delegated. 
//				for (int i = 0; i < numberOfPlayers; i++) {
//					if (playerKats [i] == null) {
//						// ---- Respawn Kat ----
//						// Get Spawn Point
//						Transform spawnPoint = getRandomSpawnPoint ();
//
//						GameObject playerKat = Instantiate (playerPrefab, spawnPoint.position, spawnPoint.rotation);
//						ModelEntity modelEntity = playerKat.GetComponent<ModelEntity> ();
//						modelEntity.setModel (modelManager.getModelWithCollider ((ModelManager.Shapes)playerModels [i]));
//
//						// Configure player controller
//						PlayerController controller = playerKat.GetComponent<PlayerController> ();
//						controller.storeDevice (playerControls [i]);
//						controller.playerNumber = i;
//
//						// Optional: Modify player controller
//						//			controller.movementSpeed = 15;
//						//			controller.jumpForce = 5;
//
//						// Register Kat
//						playerKats [i] = playerKat;
//					}
//				}

				// Spawn Walls
				wallManager.spawnWall (0, currentLevelData.wallSeqeunceL [currentSequence]);
				wallManager.spawnWall (1, currentLevelData.wallSeqeunceC [currentSequence]);
				wallManager.spawnWall (2, currentLevelData.wallSeqeunceR [currentSequence]);

				// Check for specific walls to change tutorial text
				if (isTutorial) {
					if (!currentLevelData.texts [currentSequence].Equals("empty")) {
						updateTutorialTextDisplay (currentLevelData.texts [currentSequence], 10.0f);
					}
				}

				// Transit
				// Check if end of all sequence
				currentGameState = GameState.prepare;

			}
			break;

		case GameState.end:
			// TODO: Listen for any ready button
			// store each player score to the cumulative array 
			updateCumulativeScore();
			if (endPhase == 1) {
				// Bad practice here, hopefully will improve this soon
				try {
					GameObject fader = GameObject.Find ("Fader");
					Animator faderAnim = fader.GetComponent<Animator> ();
					faderAnim.Play ("FadeOut");
				} catch {
					// Do nothing
				}
				StartCoroutine ("waitForFadeOut");
				endPhase = 2;
			} else if (endPhase == 2 && canSwitchScene) {
				// Unload resources
				tearDown ();
				if (isGameOver) {
					// Load GameOver level
					SceneManager.LoadScene (gameOverLevel);
				} else {
					// Load next Level
					SceneManager.LoadScene (nextLevel);
				}
				currentGameState = GameState.inactive;
			}
			break;

		case GameState.pause:
			break;

		default:
			break;
		}

		// Others
		// Listen for pause game button
		if (playerControls != null) {
			foreach (InputDevice input in playerControls) {
				if (input.GetButtonDown (MappedButton.Start)) {
					togglePauseGame ();
					wallManager.togglePauseAllWalls ();
					break;
				}
			}
		}
	}

	// Base method to prepare level essentials
	void prepareLevel(int currentLevel) {
		modelManager = modelManagerObject.GetComponent<ModelManager> ();
		wallManager = wallManagerObject.GetComponent<WallManager> ();
		if (platformManagerObject != null) {
			platformManager = platformManagerObject.GetComponent<PlatformManager> ();
		}
		var list = getRandomizeSpawnList();
		// Spawn Models
		for (int i = 0; i < numberOfPlayers; i++) {
			// Get Spawn Point
			Transform spawnPoint = list[i].transform;

			/// Spawn Model
			GameObject playerKat = Instantiate (playerPrefab, spawnPoint.position, spawnPoint.rotation);
			ModelEntity modelEntity = playerKat.GetComponent<ModelEntity> ();
			modelEntity.setModel (modelManager.getModelWithCollider ((ModelManager.Shapes)playerModels [i]));
			ChangeColor (playerKat, i);

			// Configure player controller
			PlayerController controller = playerKat.GetComponent<PlayerController> ();
			controller.storeDevice (playerControls [i]);
			controller.playerNumber = i;
			
			// Optional: Modify player controller
			//			controller.movementSpeed = 15;
			//			controller.jumpForce = 5;

			// Register Kat
			playerKats [i] = playerKat;

			// Attach to platform if any
			if (platformManager != null) {
				platformManager.setAsParent (playerKat);
			}
		}

		//set up the displayText 
		for (int i = 0; i < 4 ; i++){
			if (i > numberOfPlayers - 1) {
				updateKatzScoreDisplay (i, "Katz\n AFK");
				katzScoreTexts [i].changeTextAlpha (0.5f);
			} else {
				updateKatzScoreDisplay (i, "Katz\n 0");
			}
		}


		// Setup Wall Spawns
		wallManager.setupSpawnPoints(wallSpawnPoints);

		// Load level
		var levelData = levelDataObject.GetComponent<LevelData>();
		currentLevelData = levelData.getLevelData (currentLevel);

		//set up targetScore 
		targetScore = calculateTargetScore( currentLevelData.passScorePerPlayer, numberOfPlayers); 

		// set up the score array 
		playersScore = new int[numberOfPlayers];
		if (currentLevel == 0 || cumulativePlayersScore == null) {
			cumulativePlayersScore = new int[numberOfPlayers];
		}
		print ("cumulative player score player 1 is: " + cumulativePlayersScore [0]);
		// Unpack level
		if (currentLevelData.lowGravity) {
			Physics.gravity = new Vector3 (0f, -4.0f, 0f); // Moon Gravity?
		}

		totalSequences = currentLevelData.numSequence;

		// Play BGM
		AudioSource bgm = GetComponent<AudioSource>();
		if (!bgm.isPlaying) {
			bgm.Play ();
		}

		// Display Level Name
		centerText.toggleAnimation ();
		if (currentLevel == 0) {
			updateCenterTextDisplay ("T U T O R I A L", 5.0f, 50);
		} else {
			updateCenterTextDisplay ("L E V E L : " + currentLevel, 5.0f, 50);
		}
	}


	// ---- Listeners ----
	// Fetch player info from lobby
	public void initialiseFromLobby() {
		try {
			lobbyManager = GameObject.Find ("LobbyManager").GetComponent<LobbyManager> ();
		} catch (System.Exception e) {
			print ("Debug: " + e.ToString());
			lobbyManager = GameObject.Find ("TestLobbyManager").GetComponent<LobbyManager> ();
			var input = VirtualController.inputDevices[1];
			lobbyManager.playerVirtualInput.Add (input);
			this.gameObject.SetActive(true);
		}

		numberOfPlayers = lobbyManager.numberOfPlayers ();
		playerModels = lobbyManager.getPlayerModels ();
		playerControls = lobbyManager.getPlayerControls ();
		playerKats = new GameObject[numberOfPlayers];

		// Dispose Lobby after use
		Destroy(lobbyManager.gameObject);
	}

	// Reset variables for new level
	public void reset() {
		numOfLives = 9;
		playerKats = new GameObject[numberOfPlayers];
		currentGameState = GameState.start;
		gameTime = 0;
		currentSequence = 0;
		totalSequences = 0;
		currentScore = 0; 
		targetScore = 0;
		nextGameTime = 0;
		isGameOver = false;
		isTutorial = false;

		currentLevel = 0;
		killZPlane = 0;
		wallManagerObject = null;
		modelManagerObject = null;
		livesText = null;
		centerText = null;
		timerText = null;
		wavesLeftText = null;
		tutorialText = null;
		GameUI = null;
		PauseUI = null;
		platformManagerObject = null;
		platformManager = null;
		spawnPoints = null;
		wallSpawnPoints = null;
		nextLevel = "";
	}
	/*
	public void reduceLife() {
		if (numOfLives > 0) {
			numOfLives -= 1;
			updateLivesDisplay();
		}
	}
	*/

	// Toggle Game Pause
	public void togglePauseGame() {
		if (currentGameState != GameState.pause) {
			previousGameState = currentGameState;
			Time.timeScale = 0.0f;
			PauseUI.SetActive (true);
			currentGameState = GameState.pause;
		} else {
			Time.timeScale = 1.0f; 
			currentGameState = previousGameState;
			PauseUI.SetActive (false);
		}
	}


	public int[] getCumulativeScoreArray() {
		return cumulativePlayersScore;
	}
	public int[] getLevelScoreArray() {
		return playersScore;
	}

	void updateCumulativeScore(){
		for (int i = 0; i < playersScore.Length; i++) {
			cumulativePlayersScore [i] += playersScore[i];
		}
	}

	public void resetCumulativeScore() {
		for (int i = 0; i < playersScore.Length; i++) {
			cumulativePlayersScore [i] -= playersScore[i];
			playersScore[i] = 0;
		}
	}

	// ---- Helper Methods ----
	// Displays
	/*
	void updateLivesDisplay()	{
		livesText.changeText ("Lives Left: " + numOfLives.ToString ());
	}
	*/

	void updateScoreDisplay() {
		livesText.changeText ("Score: " + currentScore.ToString());
	}

	void updateTargetScoreDisplay() {
		if (currentScore >= targetScore) {
			targetScoreText.changeText ("Target Score completed!");
			return;
		}
		targetScoreText.changeText ("Target Score: " + targetScore.ToString());
	}

	void updateKatzScoreDisplay(int katzNumber, string newText){
		TextController specifiedController = katzScoreTexts [katzNumber];
		specifiedController.changeText (newText);
	}

	void updateTimerDisplay() {
		int minutes = gameTime / 60;
		string minutesText = minutes.ToString ();
		if (minutes < 10) {
			minutesText = "0" + minutesText;
		}
		int seconds = gameTime % 60;
		string secondsText = seconds.ToString ();
		if (seconds < 10) {
			secondsText = "0" + secondsText;
		}
		timerText.changeText("Time\n" + minutesText + " : " + secondsText);
	}

	void updateCenterTextDisplay(string newText, float time) {
		centerText.displayText (time);
		centerText.changeText(newText);
	}

	void updateCenterTextDisplay(string newText, float time, int fontSize) {
		centerText.displayText (time);
		centerText.changeText(newText, fontSize);
	}

	void updateTutorialTextDisplay(string newText, float time) {
		tutorialText.displayText (time);
		tutorialText.changeText(newText);
	}

	void updateWavesLeftDisplay() {
		int wavesLeft = totalSequences - currentSequence;
		if (wavesLeft < 0) {
			wavesLeftText.changeText("Waves Left: Complete!");
		} else {
			wavesLeftText.changeText("Waves Left: " + wavesLeft);
		}
	}
	List<GameObject> getRandomizeSpawnList() {
		List<GameObject> list = new List<GameObject>();
		for(int i = 0; i < spawnPoints.Length; i++) {
			list.Add(spawnPoints[i]);
		}
		return Fisher_Yates_CardDeck_Shuffle(list);
	}
	// Positions & Renderers
	Transform getRandomSpawnPoint() {
		if (spawnPoints.Length == 0) {
			return this.transform; // Default, self transform
		}
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);
		return spawnPoints[spawnPointIndex].transform;
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

	// Game Timer
	void startLevelTimer() {
		StartCoroutine ("GameTimer");
	}

	IEnumerator GameTimer() {
		while (currentGameState != GameState.end) {
			yield return new WaitForSeconds (1.0f);
			if (currentGameState != GameState.pause) {
				gameTime += 1;
				updateTimerDisplay ();
			}
		}
	}

	IEnumerator waitForDropTime() {
		yield return new WaitForSeconds (dropTime);
		collectionPhase = 2;
	}

	IEnumerator waitForFadeOut() {
		yield return new WaitForSeconds (3.0f);
		canSwitchScene = true;
	}

	// Others
	int getIndexCircular(int index, int length) {
		if (length == 0) {
			return 0;
		}
		return index % length;
	}

	int calculateTargetScore( int targetPerPerson, int numOfPlayer) {
		switch (numOfPlayer) {
		case 1:
			return targetPerPerson; 
		case 2:
			return targetPerPerson * 2;
		case 3:
			double result = targetPerPerson * 2.5;
			return (int)result; 
		case 4:
			return targetPerPerson * 3;
		}

		return targetPerPerson; 

	}

	// Unload resources before transiting to next scene
	void tearDown() {
		AudioSource bgm = GetComponent<AudioSource>();
		bgm.Stop ();
		endPhase = 1;
		canSwitchScene = false;
		foreach (GameObject kat in playerKats) {
			Destroy (kat);
		}

	}

  //================================================================//
  //===================Fisher_Yates_CardDeck_Shuffle====================//
  //================================================================//
 
  /// With the Fisher-Yates shuffle, first implemented on computers by Durstenfeld in 1964, 
  ///   we randomly sort elements. This is an accurate, effective shuffling method for all array types.
 
	public static List<GameObject> Fisher_Yates_CardDeck_Shuffle (List<GameObject>aList) {

		System.Random _random = new System.Random ();

		GameObject myGO;

		int n = aList.Count;
		for (int i = 0; i < n; i++)
		{	
			// NextDouble returns a random number between 0 and 1.
			// ... It is equivalent to Math.random() in Java.
			int r = i + (int)(_random.NextDouble() * (n - i));
			myGO = aList[r];
			aList[r] = aList[i];
			aList[i] = myGO;
		}

		return aList;
	}

}
