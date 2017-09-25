using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingScript : MonoBehaviour {

	// Vertical bobbing magnitude
	public float magnitude = 0.0025f;

	private float frequencyMin = 4.0f;
	private float frequencyMax = 6.0f;
	private float waterSurfaceHeightThreshold = 0.1f;
	private float randomInterval;

	private Rigidbody rb;

	private bool startBobbing = false;

	void Start () {
		rb = GetComponent<Rigidbody>();
		randomInterval = Random.Range(frequencyMin, frequencyMax);
	}

	void Update() {
		if (rb.transform.position.y <= waterSurfaceHeightThreshold) {
			rb.useGravity = false;
			rb.velocity = Vector3.zero;
			startBobbing = true;
		}
	}
		
	void FixedUpdate () {
		if (startBobbing) {
			rb.MovePosition (transform.position + transform.up * (Mathf.Cos (Time.time * randomInterval) * magnitude));
		}

		//rb.MovePosition (transform.eulerAngles.x + (Mathf.Cos (Time.time * randomInterval) * 2));
	}
}
