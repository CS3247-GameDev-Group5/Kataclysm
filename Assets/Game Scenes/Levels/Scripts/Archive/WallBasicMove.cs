using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBasicMove : MonoBehaviour {
	public float moveSpeed = 5f;
	public float direction = -1f;
	public float killZ = -10f;

	private bool isPaused = false;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}

	public void changeSpeed(float newSpeed) {
		moveSpeed = newSpeed;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isPaused) {
			return;
		}
		rb.MovePosition(transform.position + transform.forward * Time.deltaTime * direction * moveSpeed);
	}
		
	void Update() {
		// KillZ Plane
		if (transform.position.z < killZ) {
			Destroy (transform.gameObject);
		}
	}

	public void togglePause() {
		isPaused = !isPaused;
	}
}
