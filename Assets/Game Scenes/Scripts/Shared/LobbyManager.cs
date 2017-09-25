using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {

	public int max_players = 4;
	public GameObject[] playerSpawnPoints;
	public GameObject[] playerTextObjects;
	public GameObject playerSelectorObject;

	// For Fading
	public GameObject fadeOutObject;
	public GameObject levelSeletectorObject;

	// Sounds
	public AudioClip bgm;
	public AudioClip readySound;

	// For Lobby-only use
	bool disableInputs = false;
	GameObject[] playerObjects;
	bool[] playerInGame;
	//string[] usedStartKeys;
	List<bool> playerReady = new List<bool>();
	AudioSource bgmAudio;
//	audioSource.clip = Resources.Load(name) as AudioClip;
	AudioSource readyAudio;

	// Passed to level manager
	public int[] playerModels;
	public List<InputDevice> playerVirtualInput = new List<InputDevice>();
	public int nextPlayer = 0; // -> NumOfPlayers

	public string[] levelStrings;
	int selectedLevelIndex = 0;
	TextController[] levelSelectorText;

	// Persistence through scene changes.
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}

	// Use this for initialization
	void Start () {
		LevelManager.instance.gameObject.SetActive(false);
		playerObjects = new GameObject [max_players];
		playerModels = new int[max_players];
		playerReady = new List<bool> ();
		playerVirtualInput = new List<InputDevice> ();
		playerInGame = new bool[max_players];
		for (int i = 0; i < max_players; i++) {
			playerInGame [i] = false;
		}

		// Audio
		bgmAudio = this.gameObject.AddComponent<AudioSource>();
		readyAudio = this.gameObject.AddComponent<AudioSource>();
		playBGM();
		readyAudio.playOnAwake = false;
		readyAudio.clip = readySound;
		levelSelectorText = levelSeletectorObject.GetComponentsInChildren<TextController>();

		// print("c "+VirtualController.inputDevices.Count);
		// print("c "+VirtualController.inputDevices[1].name);
	}

	// Update is called once per frame
	void Update () {
		// print("U");
		// Listen for player Start keys:
		for(int vId = 0; vId < VirtualController.inputDevices.Count; vId++) {
			var input = VirtualController.inputDevices[vId];
			if(input.GetButtonDown(MappedButton.Start) && !disableInputs) {
				print("pressed start "+input.name);
				var playerId = 	playerVirtualInput.IndexOf(input);		
				if(playerId >= 0) {
					toggleReady (playerId);
				} else if(playerVirtualInput.Count < max_players){
					addPlayer(input);
				}
			}
		}
		if(playerVirtualInput.Count > 0 && !playerReady[0]) {
			if(playerVirtualInput[0].GetButtonDown(MappedButton.MenuL1)) {
				if(selectedLevelIndex > 0) {
					selectedLevelIndex -= 1;
					levelSelectorText[0].changeText(toUpperCaseAndSpacesBetween(levelStrings[selectedLevelIndex]));
				}
			} else if(playerVirtualInput[0].GetButtonDown(MappedButton.MenuR1)) {
				if(selectedLevelIndex < levelStrings.Length - 1) {
					selectedLevelIndex += 1;
					levelSelectorText[0].changeText(toUpperCaseAndSpacesBetween(levelStrings[selectedLevelIndex]));
				}
			}
		}
	}

	string toUpperCaseAndSpacesBetween(string text) {
		string upperText = text.ToUpper();
		string resultText = "";
		foreach(char c in upperText) {
			resultText += c + " ";
		}
		return resultText.TrimEnd();
	}
	// Adds a player object and register the player
	void addPlayer(InputDevice input) {			
		print("adding"+input.name);
		// Register player
		playerInGame [nextPlayer] = true;
		playerReady.Add (false); // Set ready to false
		playerVirtualInput.Add(input);

		// Create Player Selector Object
		GameObject player = Instantiate(playerSelectorObject, 
			playerSpawnPoints[nextPlayer].transform.position, 
			playerSpawnPoints[nextPlayer].transform.rotation) as GameObject;
		playerObjects[nextPlayer] = player;
		player.AddComponent<Rigidbody>();
		KatModelSelector playerModelSelector = player.GetComponent<KatModelSelector> ();
		playerModelSelector.spawnPoint = playerSpawnPoints [nextPlayer];
		playerModelSelector.setVirtualInputController(input); // Use the start key to map to the control set in model selector
		playerModelSelector.playerNumber = nextPlayer;

		// Update the player start text if it exists
		if (nextPlayer < playerTextObjects.Length) {
			TextController playerTextController = playerTextObjects [nextPlayer].GetComponent<TextController>();
			playerTextController.toggleAnimation ();
			playerTextController.changeText("Press 'Left' or 'Right' to \nchange your Kat! \nPress 'Start' again to Ready!", 38);
		}
		// increment next player counter
		nextPlayer += 1;
	}

	void toggleReady(int playerId) {
		// Toggle Ready
		TextController playerTextController = playerTextObjects [playerId].GetComponent<TextController>();
		bool isReady = (bool) playerReady[playerId];
		if (!isReady) {
			// Save selected model type
			KatModelSelector modelSelector = playerObjects [playerId].GetComponent<KatModelSelector> ();
			playerModels [playerId] = modelSelector.getCurrentModel ();
			// -> Ready Up 
			playerTextController.changeText ("READY", 55);
			playerReady [playerId] = true;
			modelSelector.isReady = true;
			playReadyAudio ();
		} else {
			// -> Not ready
			playerTextController.changeText ("Press 'Left' or 'Right' to \nchange your Kat! \nPress 'Start' again to Ready!", 38);
			playerReady [playerId] = false;
			KatModelSelector modelSelector = playerObjects [playerId].GetComponent<KatModelSelector> ();
			modelSelector.isReady = false;
		}

		// Check if all ready
		if (isAllReady ()) {
			Debug.Log ("All Ready");
			GoToGameScreen ();
			bgmAudio.Stop ();
		}
	}

	IEnumerator FadeToScene(float delay, string scene) {
		yield return new WaitForSeconds(delay);
		disableInputs = true; // Prevent listening for inputs in next scene
		LevelManager.instance.gameObject.SetActive(true);
		SceneManager.LoadScene (scene);
	}

	void GoToGameScreen() {
		GameObject.Find ("Fader").GetComponent<Animator>().Play ("FadeOut");
		StartCoroutine (FadeToScene (1.0f, levelStrings[selectedLevelIndex]));
	}


	bool isAllReady() {
		if (playerReady.Count < 1) {
			return false;
		}
		foreach (bool ready in playerReady) {
			if (!ready) {
				return false;
			}
		}
		return true;
	}

	private void playBGM() {
		bgmAudio.playOnAwake = false;
		bgmAudio.clip = bgm;
		bgmAudio.loop = true;
		bgmAudio.Play ();
	}

	private void playReadyAudio() {
		readyAudio.Play ();
	}

	// ---- For Game Manager ----
	// Return number of players
	public int numberOfPlayers() {
		return nextPlayer;
	}

	// Return array of player models (indexes)
	public int[] getPlayerModels() {
		return playerModels;
	}

	// Return the control sets of players (map from playerStartKey name)
	public InputDevice[] getPlayerControls() {
		return playerVirtualInput.ToArray();
	}
}
