using DG.Tweening;

using Game.Managers.CharacterManager;
using Game.Managers.GameManager;
using Game.Systems.BattleSystem;
using Game.Systems.InteractionSystem;
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
	[field: SerializeField] public UICharacterSheetWindow CharacterStatus { get; private set; }
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
	}

	private void OnDestroy()
	{
		signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
	}

	private void Start()
	{
		CreateVirtualSpaces();

		SetVirtualSpace(characterManager.CurrentParty.LeaderPartyIndex);
		CharacterStatus.SetCharacter(characterManager.CurrentParty.LeaderParty);

		signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
	}

	public void SetVirtualSpace(int index)
	{
		virtualSpaces.ForEach((x) => x.gameObject.SetActive(false));

		virtualSpaces[index].gameObject.SetActive(true);
		CurrentVirtualSpace = virtualSpaces[index];
	}

	private void CreateVirtualSpaces()
	{
		for (int i = 0; i < characterManager.CurrentParty.Characters.Count; i++)
		{
			var space = Instantiate(originalVirtualSpace, originalVirtualSpace.transform.parent);
			virtualSpaces.Add(space);
			space.gameObject.SetActive(false);
		}

		originalVirtualSpace.gameObject.SetActive(false);
	}

	private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
	{
		int index = characterManager.CurrentParty.LeaderPartyIndex;

		SetVirtualSpace(index);
		CharacterStatus.SetCharacter(signal.leader);
	}
}