using Game.Entities;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

public class UIAvatar : PoolableObject
{
	public UnityAction<UIAvatar> onClicked;
	public UnityAction<UIAvatar> onDoubleClicked;

	[field: SerializeField] public UIButtonPointer BackgroundButton { get; private set; }
	[field: Space]
	[field: SerializeField] public Image Avatar { get; private set; }
	[field: Space]
	[field: SerializeField] public Image FrameLeader { get; private set; }
	[field: SerializeField] public Image FrameSpare { get; private set; }
	[field: SerializeField] public Image IconInBattle { get; private set; }

	public Character CurrentCharacter { get; private set; }

	private void OnDestroy()
	{
		if (BackgroundButton != null)
		{
			BackgroundButton.onClickChanged -= OnClick;
		}
		if (CurrentCharacter != null)
		{
			CurrentCharacter.onCharacterBattleStateChanged -= UpdateBattleUI;
		}
	}

	private void Start()
	{
		BackgroundButton.onClickChanged += OnClick;
	}

	public void SetCharacter(Character character)
	{
		if(CurrentCharacter != null)
		{
			CurrentCharacter.onCharacterBattleStateChanged -= UpdateBattleUI;
		}
		CurrentCharacter = character;
		CurrentCharacter.onCharacterBattleStateChanged += UpdateBattleUI;
		
		UpdateUI();
	}

	public void SetFrame(bool isLeader)
	{
		FrameLeader.enabled = isLeader;
		FrameSpare.enabled = !isLeader;
	}

	private void UpdateUI()
	{
		Avatar.sprite = CurrentCharacter.EntityData.characterSprite;
		UpdateBattleUI();
	}

	private void UpdateBattleUI()
	{
		IconInBattle.enabled = CurrentCharacter.InBattle;
	}

	private void OnClick(int count)
	{
		if(count == 1)
		{
			onClicked?.Invoke(this);
		}
		else if(count > 1)
		{
			onDoubleClicked?.Invoke(this);
		}
	}

	public class Factory : PlaceholderFactory<UIAvatar> { }
}