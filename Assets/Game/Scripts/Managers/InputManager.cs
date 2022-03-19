using ModestTree.Util;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class InputManager : IInitializable, IDisposable
{
	private Dictionary<KeyAction, KeyCode> bindings;

	public InputManager(Dictionary<KeyAction, KeyCode> bindings)
	{
		this.bindings = bindings;
	}

	public void Initialize()
	{

	}
	public void Dispose()
	{

	}


	public bool GetKeyUp(KeyAction action)
	{
		return Input.GetKeyUp(bindings[action]);
	}

	public bool GetKey(KeyAction action)
	{
		return Input.GetKey(bindings[action]);
	}

	public bool GetKeyDown(KeyAction action)
	{
		return Input.GetKeyDown(bindings[action]);
	}

	public Vector2 GetMousePosition()
	{
		return Input.mousePosition;
	}
	public bool IsLeftMouseButtonDown()
	{
		return Input.GetMouseButtonDown(0);
	}
	public bool IsLeftMouseButtonPressed()
	{
		return Input.GetMouseButton(0);
	}

	public bool IsScroolWheelUp()
	{
		return Input.mouseScrollDelta.y > 0;
	}
	public bool IsScroolWheelDown()
	{
		return Input.mouseScrollDelta.y < 0;
	}
}