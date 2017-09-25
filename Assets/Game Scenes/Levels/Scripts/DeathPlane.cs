using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour {
	public string tagName = "Player";
	public Transform[] playerRespawnPoints; 

	// Use this for initialization
	void Start () {
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.layer == 10) {
			var player = other.attachedRigidbody.gameObject;
			Destroy (other.gameObject);
		} else if(other.gameObject.layer == 9 && other.gameObject.CompareTag(tagName)) {
			Destroy (other.gameObject);
		}
	}
//	Level Manager will handle respawn
//	void respawnCollidedObject(GameObject player)	{
//		int spawnIndex = Random.Range (0, playerRespawnPoints.Length);
//
//		Rigidbody rb = player.GetComponent<Rigidbody> ();
//		rb.ResetInertiaTensor();
//		rb.velocity = Vector3.zero;
//		rb.angularVelocity = Vector3.zero;
//
//		player.transform.position = playerRespawnPoints[spawnIndex].position;
//		player.transform.rotation = playerRespawnPoints[spawnIndex].rotation;
//
//		// reduce live in game manager
//	}
}
