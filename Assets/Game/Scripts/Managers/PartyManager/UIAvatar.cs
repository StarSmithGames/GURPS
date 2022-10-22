using Game.Entities;
using Game.Entities.Models;
using Game.Systems.BattleSystem;
using Game.Systems.SheetSystem;
using Game.UI;
using Game.UI.CanvasSystem;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

using Zenject;

namespace Game.Managers.PartyManager
{
	public class UIAvatar : PoolableObject
	{
		public UnityAction<UIAvatar> onClicked;
		public UnityAction<UIAvatar> onDoubleClicked;

		[field: SerializeField] public PointerHandlerComponent PointerHandler { get; private set; }
		[field: Space]
		[field: SerializeField] public Image Avatar { get; private set; }
		[field: Space]
		[field: SerializeField] public Image FrameLeader { get; private set; }
		[field: SerializeField] public Image FrameSpare { get; private set; }
		[field: SerializeField] public Image IconInBattle { get; private set; }
		[field: Space]
		[field: SerializeField] public UIBar HPBar { get; private set; }
		[field: Space]
		[field: SerializeField] public Transform EffectsContent { get; private set; }

		public ICharacter CurrentCharacter { get; private set; }

		private bool isInitialized = false;
		private IStatBar stat;
		private List<UIEffect> effects = new List<UIEffect>();

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
		private UIEffect.Factory effectFactory;

		[Inject]
		private void Construct(UISubCanvas subCanvas, UIEffect.Factory effectFactory)
		{
			this.subCanvas = subCanvas;
			this.effectFactory = effectFactory;
		}

		private void OnDestroy()
		{
			if (isInitialized)
			{
				if (PointerHandler != null)
				{
					PointerHandler.onPointerClick -= OnPointerClick;
					PointerHandler.onPointerEnter -= OnPointerEnter;
					PointerHandler.onPointerExit -= OnPointerExit;
				}
			}
			//if (CurrentModel != null)
			//{
			//	CurrentModel.onBattleChanged -= UpdateBattleUI;
			//}
		}

		public void SetCharacter(ICharacter character)
		{
			if (CurrentCharacter != null)
			{
				CurrentCharacter.Effects.onRegistratorChanged -= UpdateEffectsUI;
				//character.Model.JoinBattle -= UpdateBattleUI;
			}

			CurrentCharacter = character;
			HPBar.SetStat(CurrentCharacter.Sheet.Stats.HitPoints, CurrentCharacter.Sheet.Settings.isImmortal);

			UpdateUI();

			if (CurrentCharacter != null)
			{
				CurrentCharacter.Effects.onRegistratorChanged += UpdateEffectsUI;
				//CurrentModel.onBattleChanged += UpdateBattleUI;
			}
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
			var isInBattle = CurrentCharacter.Model.InBattle;

			IconInBattle.enabled = isInBattle;

			FrameLeader.color = isInBattle ? Color.grey : Color.white;
			FrameSpare.color = isInBattle ? Color.grey : Color.white;
		}

		private void UpdateEffectsUI()
		{
			var characterEffects = CurrentCharacter.Effects.CurrentEffects;

			CollectionExtensions.Resize(characterEffects, effects,
			() =>
			{
				var effect = effectFactory.Create();

				effect.transform.SetParent(EffectsContent);
				effect.transform.localScale = Vector3.one;
				return effect;
			},
			() =>
			{
				var effect = effects.Last();

				effect.DespawnIt();
				return effect;
			});

			for (int i = 0; i < effects.Count; i++)
			{
				effects[i].SetEffect(characterEffects[i]);
			}
		}


		public override void OnSpawned(IMemoryPool pool)
		{
			if (!isInitialized)
			{
				PointerHandler.onPointerEnter += OnPointerEnter;
				PointerHandler.onPointerExit += OnPointerExit;
				PointerHandler.onPointerClick += OnPointerClick;

				EffectsContent.DestroyChildren();
			}

			isInitialized = true;

			base.OnSpawned(pool);
		}

		private void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.clickCount == 1)
			{
				onClicked?.Invoke(this);
			}
			else if (eventData.clickCount > 1)
			{
				onDoubleClicked?.Invoke(this);
			}
		}

		private void OnPointerEnter(PointerEventData eventData)
		{
			if (CurrentCharacter != null)
			{
				EntityInformation.SetSheet(CurrentCharacter.Sheet);

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