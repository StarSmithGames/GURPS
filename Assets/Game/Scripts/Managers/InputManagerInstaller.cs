using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;

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

		Container.BindInstance(pairs).WhenInjectedInto<InputManager>();

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
	Interact			= 10,
	ContextMenu			= 15,

	CancelAction		= 20,

	CameraCenter		= 25,
	TacticalCamera		= 26,

	CameraZoomIn		= 27,
	CameraZoomOut		= 28,

	CameraRotate		= 29,
	CameraRotateLeft	= 30,
	CameraRotateRight	= 31,

	CameraForward		= 32,
	CameraBackward		= 33,
	CameraLeft			= 34,
	CameraRight			= 35,


	WorldTooltips		= 45,

	Inventory			= 50,
	InGameMenu			= 55,

	QuickSave			= 65,
	QuickLoad			= 66,
}