using DG.Tweening;

using Game.Managers.CharacterManager;
using Game.Managers.GameManager;
using Game.Systems.BattleSystem;
using Game.Systems.InventorySystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class UIManager : MonoBehaviour
{
	public UIVirtualSpace CurrentVirtualSpace { get; private set; }
	public UIWindowsManager WindowsManager { get; private set; }

	[field: SerializeField] public UIAvatars Avatars { get; private set; }
	[field: SerializeField] public UICharacterStatusWindow CharacterStatus { get; private set; }
	[field: SerializeField] public UIBattle Battle { get; private set; }
	[Space]
	[SerializeField] private UIVirtualSpace originalVirtualSpace;

	private List<UIVirtualSpace> virtualSpaces = new List<UIVirtualSpace>();

	private SignalBus signalBus;
	private CharacterManager characterManager;

	[Inject]
	private void Construct(SignalBus signalBus, UIWindowsManager windowsManager, CharacterManager characterManager)
	{
		this.signalBus = signalBus;
		this.characterManager = characterManager;

		WindowsManager = windowsManager;


		signalBus?.Subscribe<SignalCharacterChanged>(OnCharacterChanged);
	}

	private void OnDestroy()
	{
		signalBus?.Unsubscribe<SignalCharacterChanged>(OnCharacterChanged);
	}

	private void Awake()
	{
		for (int i = 0; i < 2; i++)
		{
			var space = Instantiate(originalVirtualSpace, originalVirtualSpace.transform.parent);
			virtualSpaces.Add(space);
			space.gameObject.SetActive(false);
		}

		originalVirtualSpace.gameObject.SetActive(false);
	}

	private void SetVirtualSpace(int index)
	{
		virtualSpaces.ForEach((x) => x.gameObject.SetActive(false));

		virtualSpaces[index].gameObject.SetActive(true);
		CurrentVirtualSpace = virtualSpaces[index];
	}

	private void OnCharacterChanged(SignalCharacterChanged signal)
	{
		int index = characterManager.Party.CurrentCharacterIndex;

		SetVirtualSpace(index);
		Avatars.SetAvatarFrame(index);
		CharacterStatus.SetCharacter(signal.character);
	}
}