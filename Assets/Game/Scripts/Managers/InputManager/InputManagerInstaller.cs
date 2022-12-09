using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Managers.InputManager
{
	[CreateAssetMenu(fileName = "InputManagerInstaller", menuName = "Installers/InputManagerInstaller")]
	public class InputManagerInstaller : ScriptableObjectInstaller<InputManagerInstaller>
	{
		public List<KeyBinding> keyBindings = new List<KeyBinding>();

		public override void InstallBindings()
		{
			Dictionary<KeyAction, KeyCode> pairs = new Dictionary<KeyAction, KeyCode>();
			for (int i = 0; i < keyBindings.Count; i++)
			{
				pairs.Add(keyBindings[i].keyAction, keyBindings[i].keyCode);
			}

			Dictionary<KeyCode, string> keyNames = new Dictionary<KeyCode, string>();
			foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
			{
				keyNames.TryAdd(k, k.ToString());
			}

			for (int i = 0; i < 10; i++)
			{
				keyNames[(KeyCode)((int)KeyCode.Alpha0 + i)] = i.ToString();
				keyNames[(KeyCode)((int)KeyCode.Keypad0 + i)] = i.ToString();
			}

			keyNames[KeyCode.Minus] = "-";
			keyNames[KeyCode.Equals] = "=";
			keyNames[KeyCode.Comma] = ",";
			keyNames[KeyCode.Escape] = "Esc";

			Container.BindInstance(pairs).WhenInjectedInto<InputManager>();
			Container.BindInstance(keyNames).WhenInjectedInto<InputManager>();

			Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();
		}
	}
	[System.Serializable]
	public class KeyBinding
	{
		public KeyAction keyAction;
		public KeyCode keyCode;
	}
	public enum KeyAction : int
	{
		Interact = 10,
		ContextMenu = 15,

		CancelAction = 20,

		CameraCenter = 25,
		TacticalCamera = 26,

		CameraZoomIn = 27,
		CameraZoomOut = 28,

		CameraRotate = 29,
		CameraRotateLeft = 30,
		CameraRotateRight = 31,

		CameraForward = 32,
		CameraBackward = 33,
		CameraLeft = 34,
		CameraRight = 35,


		WorldTooltips = 45,

		Inventory = 50,
		SkillDeck = 51,
		InGameMenu = 55,

		QuickSave = 65,
		QuickLoad = 66,
	}
}