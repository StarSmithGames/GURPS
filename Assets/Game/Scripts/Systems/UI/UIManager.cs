using CMF;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class UIManager : MonoBehaviour
{
	public Transform WindowsRoot;


	public UIWindowsManager WindowsManager { get; private set; }

	public List<UIAvatar> avatars = new List<UIAvatar>();

	[Inject]
	private void Construct(UIWindowsManager windowsManager)
	{
		WindowsManager = windowsManager;
	}
}