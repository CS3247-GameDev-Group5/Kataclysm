using UnityEngine;
using System.Collections.Generic;

public class WallManager: MonoBehaviour {
	public GameObject[] walls;

	// Initialised by SceneDataManager
	Transform[] spawnPoints; // Follow L, C, R order
	GameObject[] wallAtSpawnPoints;

	public PhysicMaterial wallMat;
	public GameObject spawnParticleEffect;

	// For Dictionary of Walls
	public NamedWall[] namedWallArray; // This is what you should edit in the editor
	Dictionary<string, GameObject> dictionaryOfWalls {get; set;}	

	// ParticleEffects
	List<GameObject> particleEffectsList;

	// To Display a "Key-Value" lookalike in the editor for Wall
	[System.Serializable]
	public struct NamedWall {
		public string name;
		public GameObject wall;
	}

	// Use this for initialization
	void Start () {

		dictionaryOfWalls = new Dictionary<string, GameObject> ();

		// Manually port over the walls from the Array to the Dictionary
		foreach (NamedWall namedWall in namedWallArray) {
			dictionaryOfWalls [namedWall.name] = namedWall.wall;
		}

		particleEffectsList = new List<GameObject> ();
	}

	public void setupSpawnPoints(Transform[] newSpawnPoints) {
		spawnPoints = newSpawnPoints;
		wallAtSpawnPoints = new GameObject[spawnPoints.Length];
	}

	// Spawns a given wall (specified by name) at the given spawnPoint
	public void spawnWall(int spawnPointIndex, string wallKey) {

		string wallToSpawn;

		// If empty string, return
		if (wallKey.Trim ().Length < 1) {
			return;
		}

		if (wallKey == "empty") {
			wallAtSpawnPoints [spawnPointIndex] = null;
			return;
		}

		// Special keywords
		if (wallKey == "random") {
			int randomWall = Random.Range (0, namedWallArray.Length);
			wallToSpawn = namedWallArray [randomWall].name;
		} else {
			// Tokenize wallKey
			string[] walls = wallKey.Split(","[0]);
			int randomWall = Random.Range (0, walls.Length);
			wallToSpawn = walls [randomWall].Trim();	
		}

		// Create wall object given the spawnPoint and wall
		print ("wall " + wallToSpawn);
		var w = Instantiate (dictionaryOfWalls[wallToSpawn], spawnPoints [spawnPointIndex].position, spawnPoints [spawnPointIndex].rotation);
		wallAtSpawnPoints [spawnPointIndex] = w;

		// Create spawn ParticleFx
		if (spawnParticleEffect != null) {
			GameObject pfx = Instantiate (spawnParticleEffect, w.transform.position, Quaternion.identity);
			pfx.GetComponent<Explosion> ().Play ();
			particleEffectsList.Add (pfx);
		}
		// print (particleEffectsList.ToString());

		// Adjust height of wall
		w.transform.Translate (new Vector3 (0, 1.0f, 0));
		w.transform.localScale += new Vector3 (0.5f, 0.5f, 0.5f);

		// Attach physics and movement components
		var m = w.AddComponent<MeshCollider> ();
		m.material = wallMat;
		w.AddComponent<Rigidbody> ().isKinematic = true;
	}

	// Move a given wall at the given spawnPoint
	public void moveWall(int spawnPointIndex, float wallSpeed, float killZPlane) {
		if (wallAtSpawnPoints [spawnPointIndex] != null) {
			GameObject w = wallAtSpawnPoints [spawnPointIndex];
			w.AddComponent<WallBasicMove> ();
			WallBasicMove wMove = w.GetComponent<WallBasicMove> ();
			wMove.changeSpeed (wallSpeed);
			wMove.killZ = killZPlane;
		}
	}

	public void stopAllParticleEffects()	{
		foreach (GameObject pFx in particleEffectsList) {
			Explosion fx = pFx.GetComponentInChildren<Explosion> ();
			fx.Stop ();
		}
	}

	public void destroyAllParticleEffects() {
		foreach (GameObject pFx in particleEffectsList) {
			Destroy (pFx);
		}

		particleEffectsList = new List<GameObject> ();
	}

	public bool allWallsPassed() {
		foreach (GameObject w in wallAtSpawnPoints) {
			if (w != null) {
				return false;
			}
		}
		return true;
	}

	public void togglePauseAllWalls() {
		foreach (GameObject w in wallAtSpawnPoints) {
			if (w != null) {
				try {
					w.GetComponent<WallBasicMove> ().togglePause ();
				} finally {

				}
			}
		}
	}
}