using ModestTree.Util;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Managers.InputManager
{
	public class InputManager
	{
		private Dictionary<KeyAction, KeyCode> bindings;
		private Dictionary<KeyCode, string> keyNames;

		public InputManager(Dictionary<KeyAction, KeyCode> bindings, Dictionary<KeyCode, string> keyNames)
		{
			this.bindings = bindings;
			this.keyNames = keyNames;
		}

		public string GetKeyName(KeyCode code)
		{
			return keyNames.GetValueOrDefault(code);
		}

		public bool IsAnyKeyDown()
		{
			return Input.anyKeyDown;
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

		public bool IsRightMouseButtonDown()
		{
			return Input.GetMouseButtonDown(1);
		}
		public bool IsRightMouseButtonPressed()
		{
			return Input.GetMouseButton(1);
		}


		public bool IsMiddleMouseButtonDown()
		{
			return Input.GetMouseButtonDown(2);
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
}