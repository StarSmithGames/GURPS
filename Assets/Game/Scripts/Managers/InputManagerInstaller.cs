using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "InputManagerInstaller", menuName = "Installers/InputManagerInstaller")]
public class InputManagerInstaller : ScriptableObjectInstaller<InputManagerInstaller>
{
	[TableList]
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
public enum KeyAction
{
	Interact,
	ContextMenu,

	CancelAction,

	TacticalCamera,

	CameraZoomIn,
	CameraZoomOut,

	CameraRotate,
	CameraRotateLeft,
	CameraRotateRight,

	CameraCenter,
	CameraForward,
	CameraBackward,
	CameraLeft,
	CameraRight,


	WorldTooltips,

	Inventory,
	InGameMenu,

	QuickSave,
	QuickLoad,
}