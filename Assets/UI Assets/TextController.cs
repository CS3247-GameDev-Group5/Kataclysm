using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour {

	public Text[] textObjects;
	Animator animator;
	public int defaultFontSize = 35;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		print ("here!");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void toggleAnimation() {
		if (animator == null) {
			animator = GetComponent<Animator> ();
		}
		if (animator.isActiveAndEnabled) {
			animator.enabled = false;
			transform.GetChild (0).transform.position = this.transform.position;
		} else {
			animator.enabled = true;
		}
	}

	public void changeText(string newText, int fontSize) {
		foreach (Text textObj in textObjects) {
			textObj.text = newText;
			textObj.fontSize = fontSize;
		}
	}

	public void changeText(string newText, FontStyle fontStyle) {
		foreach (Text textObj in textObjects) {
			textObj.text = newText;
			textObj.fontStyle = fontStyle;
		}
	}

	public void changeTextAlpha(float newA){
		foreach (Text textObj in textObjects) {
			Color textColor = textObj.color; 
			textColor.a = newA; 
			textObj.color = textColor; 
		}
	}

	public void changeText(string newText) {
		foreach (Text textObj in textObjects) {
			textObj.text = newText;
			textObj.fontSize = defaultFontSize;
		}
	}

	public void displayText(float time)	{
		IEnumerator coroutine = waitToDisplay (time);
		StartCoroutine(coroutine);
	}

	private IEnumerator waitToDisplay(float time)	{
		setAnimatorBoolean ("create", true);
		yield return new WaitForSeconds (time);
		setAnimatorBoolean ("create", false);
	}

	public void setAnimatorBoolean(string parameterString, bool boolean)	{
		animator.SetBool (parameterString, boolean);
	}
}
