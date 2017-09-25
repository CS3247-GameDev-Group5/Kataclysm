using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GamepadInput : MonoBehaviour
{
	GamepadManager manager;

	public List<GamepadDevice> gamepads { 
		get{
			return manager.gamepads;
		}
	}

	public List<UnGamepadConfig> unGamepadConfigs;

	public event System.Action<GamepadDevice> OnGamepadAdded;
	public event System.Action<GamepadDevice> OnGamepadRemoved;

	void Start()
	{
		manager = new UnGamepadManager (unGamepadConfigs);
	
		manager.OnGamepadAdded += GamepadAdded;
		manager.OnGamepadRemoved += GamepadRemoved;
		
		manager.Init ();
	}

	void OnDestroy()
	{
		manager.OnGamepadAdded -= GamepadAdded;
		manager.OnGamepadRemoved -= GamepadRemoved;
	}

	void Update()
	{
		manager.Update ();
	}
	
	void GamepadAdded(GamepadDevice gamepadDevice)
	{
		if (OnGamepadAdded != null)
			OnGamepadAdded (gamepadDevice);
	}

	void GamepadRemoved(GamepadDevice gamepadDevice)
	{
		if (OnGamepadRemoved != null)
			OnGamepadRemoved (gamepadDevice);
	}
}

