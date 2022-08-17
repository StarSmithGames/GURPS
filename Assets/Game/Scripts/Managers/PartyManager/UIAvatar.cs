using Game.Entities;
using Game.Entities.Models;
using Game.Systems.SheetSystem;
using Game.UI;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Zenject;

namespace Game.Managers.PartyManager
{
	public class UIAvatar : PoolableObject
	{
		public UnityAction<UIAvatar> onClicked;
		public UnityAction<UIAvatar> onDoubleClicked;

		[field: SerializeField] public UIButtonPointer BackgroundButton { get; private set; }
		[field: SerializeField] public PointerHoverComponent PointerHover { get; private set; }
		[field: Space]
		[field: SerializeField] public Image Avatar { get; private set; }
		[field: Space]
		[field: SerializeField] public Image FrameLeader { get; private set; }
		[field: SerializeField] public Image FrameSpare { get; private set; }
		[field: SerializeField] public Image IconInBattle { get; private set; }
		[field: Space]
		[field: SerializeField] public UIBar HPBar { get; private set; }

		public ICharacter CurrentCharacter { get; private set; }
		private IStatBar stat;

		private void Start()
		{
			BackgroundButton.onClickChanged += OnClick;

			PointerHover.onPointerEnter += OnPointerEnter;
			PointerHover.onPointerExit += OnPointerExit;
		}

		private void OnDestroy()
		{
			if (BackgroundButton != null)
			{
				BackgroundButton.onClickChanged -= OnClick;
			}

			if (PointerHover != null)
			{
				PointerHover.onPointerEnter -= OnPointerEnter;
				PointerHover.onPointerExit -= OnPointerExit;
			}

			if (CurrentCharacter != null)
			{
				//CurrentCharacter.onBattleChanged -= UpdateBattleUI;
			}
		}
		

		public void SetCharacter(ICharacter character)
		{
			if (CurrentCharacter != null)
			{
				//CurrentCharacter.onBattleChanged -= UpdateBattleUI;
			}
			CurrentCharacter = character;
			HPBar.SetStat(CurrentCharacter?.Sheet.Stats.HitPoints, CurrentCharacter?.Sheet.Settings.isImmortal ?? false);
			if (CurrentCharacter != null)
			{
				//CurrentCharacter.onBattleChanged += UpdateBattleUI;
			}

			UpdateUI();
		}

		public void SetFrame(bool isLeader)
		{
			FrameLeader.enabled = isLeader;
			FrameSpare.enabled = !isLeader;
		}

		private void UpdateUI()
		{
			Avatar.sprite = CurrentCharacter.Sheet.Information.portrait;
			UpdateBattleUI();
		}

		private void UpdateBattleUI()
		{
			IconInBattle.enabled = false; //= CurrentCharacter.InBattle;

			FrameLeader.color = /*CurrentCharacter.InBattle ? Color.grey :*/ Color.white;
			FrameSpare.color = /*CurrentCharacter.InBattle ? Color.grey :*/ Color.white;
		}

		private void OnClick(int count)
		{
			if (count == 1)
			{
				onClicked?.Invoke(this);
			}
			else if (count > 1)
			{
				onDoubleClicked?.Invoke(this);
			}
		}

		private void OnPointerEnter(PointerEventData eventData)
		{
			//uiManager.Battle.SetSheet(CurrentCharacter.Sheet);
		}
		private void OnPointerExit(PointerEventData eventData)
		{
			//uiManager.Battle.SetSheet(null);
		}

		public class Factory : PlaceholderFactory<UIAvatar> { }
	}
}