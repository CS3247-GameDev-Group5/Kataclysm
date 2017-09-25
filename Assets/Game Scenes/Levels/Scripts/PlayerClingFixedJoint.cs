using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Cling script, but making use of the fixedJoint
// The script does the following: 
// - based on player input control, decide if the cat is going to cling to another object. 
// - on Clinging, player has to maintain the clinging button down. Else release them. 
// - on Release, add small force to shift the clinging player from the other player.
// - force applied is done after the release of the clinging to ensure only clinging object receive the force. 

public class PlayerClingFixedJoint : MonoBehaviour {
	public float forceOnRelease =  2f ;
	Rigidbody playerRbody; 
	bool isLookingToCling;
	bool isClinging; 
	bool isReleased; 
	Vector3 contactNormal; 
	Quaternion beforeClingingNormal;
	InputDevice attachedDevice; 
	// Use this for initialization
	void Start () {
		isLookingToCling = false;
		isClinging = false; 
		isReleased = false; 
		playerRbody = GetComponent<Rigidbody> ();

		if (GetComponent<PlayerController> ()) {
			attachedDevice = GetComponent<PlayerController> ().getAttachedDevice ();
		}
	}

	void Update() {
		// Listen for callback from player controller
		//primary button hold down
		if (attachedDevice.GetButtonDown(MappedButton.Cling)) {
			isLookingToCling = true;
		} 

		//primary button released
		if (attachedDevice.GetButtonUp(MappedButton.Cling)) {
			isLookingToCling = false; 
		}
	}

	public void startCling() {
		isLookingToCling = true;
	}

	public void endCling() {
		isLookingToCling = false;
	}

	void FixedUpdate() {
		//action to take upon released from clinging
		if(isReleased){			
			isReleased = false; 
			// add force relative to the normal direction of contact
			playerRbody.AddForce (contactNormal * forceOnRelease, ForceMode.Impulse);
			contactNormal = Vector3.zero;
			// TO-DO:fix the player's normal to its original normal (in case there's a change in rotation) 
		}

		// if no longer looking to cling, but is clinging, 
		if (!isLookingToCling && isClinging) {
			//release 
			Destroy(GetComponent<FixedJoint>());
			isClinging = false;
			isReleased = true; 

		}
	}

	void OnCollisionEnter(Collision col){
		if (isLookingToCling && !isClinging) {
			GameObject collidedGameObject = col.gameObject;
			// contact to player only
			if (collidedGameObject.CompareTag ("Player")) {
				gameObject.AddComponent<FixedJoint> ();
				gameObject.GetComponent<FixedJoint> ().connectedBody = collidedGameObject.GetComponent<Rigidbody>();

				//debugging and trying to obtain average normal(?)
				foreach (ContactPoint contact in col.contacts) {
					//print (contact.thisCollider.name + "hit " + contact.otherCollider.name);
					//print (contact.otherCollider.name + "normal: " + contact.normal);
					//Debug.DrawRay (contact.point, contact.normal, Color.red);
					contactNormal += contact.normal;
				}

				// normalize the accumulation of normal.	
				contactNormal = contactNormal.normalized;
				isClinging = true; 
			}
		}

	}


}
