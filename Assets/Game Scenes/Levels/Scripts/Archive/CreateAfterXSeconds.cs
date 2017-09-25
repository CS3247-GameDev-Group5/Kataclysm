using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAfterXSeconds : MonoBehaviour {
	public float timeToWait = 1f;
	public Vector3 position;
	public GameObject objectToCreate;

	private IEnumerator coroutine;

	// Use this for initialization
	void Start () {
		coroutine = waitAndCreate ();
		StartCoroutine(coroutine);
	}

	// Update is called once per frame
	void Update () {
	}

	IEnumerator waitAndCreate()	{
		yield return new WaitForSecondsRealtime(timeToWait);
		Instantiate (objectToCreate, position, Quaternion.identity);
		StopCoroutine (coroutine);
	}
}
