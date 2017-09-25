using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour {

	public GameObject rotatingPlatformObject;
	public GameObject leftPlatformObject;
	public GameObject rightPlatformObject;
	public float rotationSpeed = 1f;
	public float movementSpeed = 1f;
	public float moveLeftBy = 5f;
	public float moveRightBy = 5;

	Vector3 eulerAngleVelocity;
	bool isRotating = false;
	bool isSplit = false;
	bool isMoving = false;
	// Move on x-axis
	float leftOriginalPos;
	float rightOriginalPos;
	// Shaking Mechanic
	bool isShaking = false;
	float shakeSpeed = 10.0f;
	float shakeMagnitude = 0.1f;
	float shakeDuration = 2.0f;

	List<GameObject> children = new List<GameObject>();

	// Use this for initialization
	void Start () {
		eulerAngleVelocity = new Vector3 (0f, rotationSpeed, 0f);
		if (leftPlatformObject != null) {
			leftOriginalPos = leftPlatformObject.transform.position.x;
		}
		if (rightPlatformObject != null) {
			rightOriginalPos = rightPlatformObject.transform.position.x;
		}
	}

	void Update() {
		if (isShaking) {
			if (leftPlatformObject == null || rightPlatformObject == null) {
				return;
			}
			if (isSplit) {
				leftPlatformObject.transform.position = new Vector3(leftOriginalPos - moveLeftBy +
					(shakeMagnitude * Mathf.Sin(Time.time * shakeSpeed)),
					leftPlatformObject.transform.position.y,
					leftPlatformObject.transform.position.z);
				rightPlatformObject.transform.position = new Vector3(rightOriginalPos + moveRightBy + 
					(shakeMagnitude * -Mathf.Sin (Time.time * shakeSpeed)),
					rightPlatformObject.transform.position.y,
					rightPlatformObject.transform.position.z);
			} else {
				leftPlatformObject.transform.position = new Vector3(leftOriginalPos + 
					(shakeMagnitude * Mathf.Sin(Time.time * shakeSpeed)),
					leftPlatformObject.transform.position.y,
					leftPlatformObject.transform.position.z);
				rightPlatformObject.transform.position = new Vector3(rightOriginalPos + 
					(shakeMagnitude * -Mathf.Sin (Time.time * shakeSpeed)),
					rightPlatformObject.transform.position.y,
					rightPlatformObject.transform.position.z);
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if ((isRotating) && (rotatingPlatformObject != null)) {
			rotatingPlatformObject.transform.Rotate (eulerAngleVelocity);
		} else if (isMoving && notInPosition ()) {
			if (isSplit) {
				leftPlatformObject.transform.position += (new Vector3 (-movementSpeed, 0f, 0f));
				rightPlatformObject.transform.position += (new Vector3 (movementSpeed, 0f, 0f));
			} else {
				leftPlatformObject.transform.position += (new Vector3 (movementSpeed, 0f, 0f));
				rightPlatformObject.transform.position += (new Vector3 (-movementSpeed, 0f, 0f));
			}
		}
	}

	public void toggleRotation() {
		isRotating = !isRotating;
		Debug.Log ("Rotating: " + isRotating);
	}

	public void toggleSplitting() {
		isShaking = true;
		StartCoroutine ("shakePlatforms");
	}

	private void startSplitting() {
		isSplit = !isSplit;
		isMoving = true;
		Debug.Log ("Splitting");
	}

	public void setAsParent(GameObject obj) {
		if (rotatingPlatformObject != null) {
			obj.transform.SetParent (rotatingPlatformObject.transform);
			children.Add (obj);
		}

	}

	public void detachAllChildren() {
		foreach (GameObject child in children) {
			child.transform.parent = null;
		}
	}

	public void attachAllChildren() {
		foreach (GameObject child in children) {
			child.transform.SetParent (rotatingPlatformObject.transform);
		}
	}

	private bool notInPosition() {
		if (isSplit) {
			if ((leftPlatformObject.transform.position.x <= leftOriginalPos - moveLeftBy)
				|| (rightPlatformObject.transform.position.x >= rightOriginalPos + moveRightBy)) {
				isMoving = false;
				leftPlatformObject.transform.position = new Vector3 (leftOriginalPos - moveLeftBy,
					leftPlatformObject.transform.position.y, leftPlatformObject.transform.position.z);
				rightPlatformObject.transform.position = new Vector3 (rightOriginalPos + moveRightBy,
					rightPlatformObject.transform.position.y, rightPlatformObject.transform.position.z);
				return false;
			} else {
				return true;
			}
		} else {
			if ((leftPlatformObject.transform.position.x >= leftOriginalPos)
				|| (rightPlatformObject.transform.position.x <= rightOriginalPos)) {
				isMoving = false;
				leftPlatformObject.transform.position = new Vector3(leftOriginalPos, 
					leftPlatformObject.transform.position.y, leftPlatformObject.transform.position.z);
				rightPlatformObject.transform.position = new Vector3(rightOriginalPos,
					rightPlatformObject.transform.position.y, rightPlatformObject.transform.position.z);
				return false;
			} else {
				return true;
			}
		}
	}

	IEnumerator shakePlatforms() {
		yield return new WaitForSeconds (shakeDuration);
		isShaking = false;
		startSplitting ();
	}
}
