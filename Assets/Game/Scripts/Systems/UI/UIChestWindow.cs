using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class UIChestWindow : WindowBasePoolable<UIChestWindow>
{
	public Transform content;

	public Button close;
	public Button takeAll;


	private UIManager uiManager;

	[Inject]
	private void Construct(UIManager uiManager)
	{
		this.uiManager = uiManager;

		close.onClick.AddListener(OnClose);
		takeAll.onClick.AddListener(OnTakeAll);
	}

	private void OnDestroy()
	{
		close.onClick.RemoveAllListeners();
		takeAll.onClick.RemoveAllListeners();
	}

	private void OnTakeAll()
	{

	}

	private void OnClose()
	{
		DespawnIt();
	}
}