using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {

		string[] names = Input.GetJoystickNames();
		Debug.Log("Connected Joysticks:");
		for(int i = 0; i < names.Length; i++) {
			Debug.Log("Joystick" + (i + 1) + " = " + names[i]);
		}
	}
	
	// Update is called once per frame
	void OnGUI () {
			var input = VirtualController.inputDevices[2];

			GUILayout.BeginHorizontal ();
			string str = "";
			str += "Device #" + input.name + "\n";
			str += "\n";
			int[] buttonValues = (int[])System.Enum.GetValues (typeof(MappedButton));
			for(int i = 0; i < buttonValues.Length; i++) {
				str += (MappedButton)buttonValues[i]  + ": " + input.GetButton ((MappedButton)buttonValues [i]) + "\n";
			}
			int[] axisValues = (int[])System.Enum.GetValues (typeof(MappedAxis));
			for (int i = 0; i < axisValues.Length; i++) {
				str += (MappedAxis)axisValues [i] + ": " + input.GetAxis ((MappedAxis)axisValues [i]) + "\n";
			}
			GUILayout.Label (str);

			GUILayout.EndHorizontal ();
	}
}
