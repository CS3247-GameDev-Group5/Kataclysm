using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDeathPlane : MonoBehaviour {

	LevelManager levelManager;
	AudioSource deathSound;

	// Use this for initialization
	void Start () {
		if (levelManager == null) {
			try {
				levelManager = GameObject.Find ("LevelManager").GetComponent<LevelManager> ();
			} catch {
				// Do nothing
			}
		}
		deathSound = GetComponent<AudioSource>();
	}

	void OnTriggerEnter(Collider other) {
		// play some sound / water splash particle effects
		if (other.gameObject.layer == 10 && other.attachedRigidbody.gameObject.activeInHierarchy) {
			// Death Sound
			deathSound.Play ();
			
			other.attachedRigidbody.gameObject.SetActive (false);

			// Callback to reduce life
			if (levelManager == null) {
				try {
					levelManager = GameObject.Find ("LevelManager").GetComponent<LevelManager> ();
					//levelManager.reduceLife ();
				} catch {
					// Do nothing
				}
			} else {
				//levelManager.reduceLife ();
			}	
		}
	}
}
