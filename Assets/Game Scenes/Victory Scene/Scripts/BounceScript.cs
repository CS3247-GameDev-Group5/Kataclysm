using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceScript : MonoBehaviour {

	Rigidbody rBody;
	void Start() {
		rBody = GetComponent<Rigidbody> ();
	}

	void OnTriggerEnter(Collider other) {
		if (other.name == "TrampolineRim") {
			transform.position += new Vector3 (0, 0.2f, 0);
			rBody.velocity = new Vector3 (rBody.velocity.x, Mathf.Max (-10f, rBody.velocity.y), rBody.velocity.z);
			rBody.AddForce (new Vector3(0, 800f, 0));
			print ("explode!");
		}
	}
}
