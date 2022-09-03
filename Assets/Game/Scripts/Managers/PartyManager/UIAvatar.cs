using Game.Entities;
using Game.Entities.Models;
using Game.Systems.BattleSystem;
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

		public ICharacterModel CurrentModel { get; private set; }

		private IStatBar stat;

		private WindowEntityInformation EntityInformation
		{
			get
			{
				if (entityInformation == null)
				{
					entityInformation = subCanvas.WindowsRegistrator.GetAs<WindowEntityInformation>();
				}

				return entityInformation;
			}
		}
		private WindowEntityInformation entityInformation;

		private UISubCanvas subCanvas;

		[Inject]
		private void Construct(UISubCanvas subCanvas)
		{
			this.subCanvas = subCanvas;
		}

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

			//if (CurrentModel != null)
			//{
			//	CurrentModel.onBattleChanged -= UpdateBattleUI;
			//}
		}

		public void SetCharacter(ICharacter character)
		{
			//if (CurrentModel != null)
			//{
			//	CurrentModel.onBattleChanged -= UpdateBattleUI;
			//}

			CurrentModel = character.Model as ICharacterModel;
			HPBar.SetStat(CurrentModel.Sheet.Stats.HitPoints, CurrentModel.Sheet.Settings.isImmortal);

			UpdateUI();

			//if (CurrentModel != null)
			//{
			//	CurrentModel.onBattleChanged += UpdateBattleUI;
			//}
		}

		public void SetFrame(bool isLeader)
		{
			FrameLeader.enabled = isLeader;
			FrameSpare.enabled = !isLeader;
		}

		private void UpdateUI()
		{
			Avatar.sprite = CurrentModel.Sheet.Information.portrait;
			UpdateBattleUI();
		}

		private void UpdateBattleUI()
		{
			IconInBattle.enabled = CurrentModel.InBattle;

			FrameLeader.color = CurrentModel.InBattle ? Color.grey : Color.white;
			FrameSpare.color = CurrentModel.InBattle ? Color.grey : Color.white;
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
			if (CurrentModel != null)
			{
				EntityInformation.SetSheet(CurrentModel.Sheet);

				if (!EntityInformation.IsShowing)
				{
					EntityInformation.Enable(true);
				}
			}
		}

		private void OnPointerExit(PointerEventData eventData)
		{
			if (EntityInformation.IsShowing)
			{
				EntityInformation.Enable(true);
			}
		}

		public class Factory : PlaceholderFactory<UIAvatar> { }
	}
}