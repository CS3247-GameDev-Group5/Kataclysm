using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

	private string Player_TAG = "Player";

	/// Check if on player
	void OnTriggerStay(Collider collider) {

		GameObject collidedGameObject = collider.gameObject; 
		if(collidedGameObject.transform.root.gameObject.CompareTag(Player_TAG)){			
			collider.attachedRigidbody.SendMessage("OnPlayerBelowTriggerStay",this.GetComponent<Collider>().attachedRigidbody);
		}
	}
}
