using DG.Tweening;

using Game.Managers.CharacterManager;
using Game.Managers.GameManager;
using Game.Systems.InventorySystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class UIManager : MonoBehaviour
{
	public UIVirtualSpace CurrentVirtualSpace { get; private set; }
	public UIWindowsManager WindowsManager { get; private set; }

	[Header("Battle")]
	public UIRound round;
	[SerializeField] private GameObject commenceBattle;
	[Space]
	[SerializeField] private UIAvatars avatars;
	[SerializeField] private UICharacterStatusWindow characterStatus;

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
		signalBus?.Subscribe<SignalGameStateChanged>(OnGameStateChanged);
	}

	private void OnDestroy()
	{
		signalBus?.Unsubscribe<SignalCharacterChanged>(OnCharacterChanged);
		signalBus?.Unsubscribe<SignalGameStateChanged>(OnGameStateChanged);
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

	private void OnGameStateChanged(SignalGameStateChanged signal)
	{
		if(signal.oldGameState == GameState.Gameplay && signal.newGameState == GameState.Battle)
		{
			Sequence sequence = DOTween.Sequence();

			sequence
				.AppendCallback(() => commenceBattle.SetActive(true))
				.AppendInterval(1f)
				.AppendCallback(() => commenceBattle.SetActive(false));
		}
	}

	private void OnCharacterChanged(SignalCharacterChanged signal)
	{
		int index = characterManager.CurrentCharacterIndex;

		SetVirtualSpace(index);
		avatars.SetAvatarFrame(index);
		characterStatus.SetCharacter(signal.character);
	}
}