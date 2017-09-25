using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Usage: 
 * Create empty game object and add this component.
 * Setup the level data and save as prefab.
 */
public class LevelData : MonoBehaviour {

	public Level[] levelData;

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad (this);
	}
	
	public Level getLevelData(int level) {
		return levelData [level];
	}
}

/// <summary>
/// Level.
/// Contains all the information and variables of a level
/// These data will be ultilised by the level manager
/// </summary>
[System.Serializable]
public class Level {
	// Basic
	public int numSequence; // If number of sequences > number of wall sequences, wall sequence will reset.
	public string[] wallSeqeunceL; // Left Side Spawn
	public string[] wallSeqeunceC; // Center Spawn
	public string[] wallSeqeunceR; // Right Side Spawn
	public float[] wallSpeed; // Circular array;
	public int[] prepareTime; // Circular array;
	public int levelType; 
	// ---- Special Modifiers ----
	// Rotating Platforms
	public bool rotatingPlatform = false;
	public bool[] rotateToggle; // toggles on/off for # sequence

	// Splitting Platforms
	public bool splittingPlatform = false;
	public bool[] splittingToggle;

	// Environment
	public bool lowGravity = false;

	//Target Score - based on one player. multiplied according to the number of player 
	public int passScorePerPlayer; 

	// Text for bottom textbox
	public string[] texts;
}