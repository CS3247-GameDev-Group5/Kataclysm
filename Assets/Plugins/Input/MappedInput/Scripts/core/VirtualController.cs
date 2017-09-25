using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(GamepadInput))]
public class VirtualController : MonoBehaviour {
	public static VirtualController instance {
		get; private set;
	}

	GamepadInput _gamepadInput;

	public GamepadInput gamepadInput {
		get {
			if (!_gamepadInput)
				_gamepadInput = GetComponent<GamepadInput> ();
			return _gamepadInput;
		}
	}

	public static System.Action<InputDevice> OnDeviceAdded;
	public static System.Action<InputDevice> OnDeviceRemoved;

	public static List<InputDevice> inputDevices = new List<InputDevice>();

	public GamepadInputMapping gamepadInputMapping;
	public KeyboardInputMapping[] keyboardInputMapping;

	void Awake()
	{
		if(instance == null) {

#if UNITY_STANDALONE || UNITY_EDITOR

			if( keyboardInputMapping != null)
				AddKeyboardDevice ();
#endif
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else Destroy(this); // or gameObject
	}

	void Start()
	{
		for (int i = 0; i < gamepadInput.gamepads.Count; i++)
		{
			OnGamepadAdded (gamepadInput .gamepads[i]);
		}

		gamepadInput.OnGamepadAdded += OnGamepadAdded;
		gamepadInput.OnGamepadRemoved += OnGamepadRemoved;
	}

	void OnDestroy()
	{
		gamepadInput.OnGamepadAdded -= OnGamepadAdded;
		gamepadInput.OnGamepadRemoved -= OnGamepadRemoved;
	}

	void Update()
	{

	}
		
	void OnGamepadAdded(GamepadDevice gamepad)
	{
		GameObject obj = new GameObject ();
		var device = obj.AddComponent<GamepadInputDevice> ();
		device.setMapping(gamepadInputMapping);
		(device as GamepadInputDevice).gamepad = gamepad;
		obj.transform.parent = transform;

		obj.name = "Device: "+ gamepad.displayName;
		inputDevices.Add (device);
		
		if( OnDeviceAdded != null )
			OnDeviceAdded(device);
	}

	void OnGamepadRemoved(GamepadDevice gamepad)
	{
		for(int i = 0; i < inputDevices.Count; i++)
		{
			if( inputDevices[i] is GamepadInputDevice && (inputDevices[i] as GamepadInputDevice).gamepad == gamepad)
			{
				var device = inputDevices[i];
				inputDevices.Remove (device);

				if( OnDeviceRemoved != null )
					OnDeviceRemoved(device);
				Destroy(device.gameObject);
			}
		}
	}

	void AddKeyboardDevice()
	{
		foreach(var kbMapping in keyboardInputMapping) {
			GameObject obj = new GameObject ("Keyboard",typeof(KeyboardMultiInputDevice));
			obj.transform.parent = transform;
			KeyboardMultiInputDevice device = obj.GetComponent<KeyboardMultiInputDevice> ();
			device.setMapping(kbMapping);
			inputDevices.Add(device);
		}
	}
}
