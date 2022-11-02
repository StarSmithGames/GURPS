using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using Game.UI.CanvasSystem;
using Game.UI.Windows;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.HUD
{
	public class WindowSkillDeck : WindowBase
	{
		[field: SerializeField] public Button Close { get; private set; }

		private List<UISlotSkill> skills = new List<UISlotSkill>();
		private Skills currentSkills;

		private UISubCanvas subCanvas;
		private UISlotSkill.Factory skillSlotFactory;

		[Inject]
		private void Construct(UISubCanvas subCanvas, UISlotSkill.Factory skillSlotFactory)
		{
			this.subCanvas = subCanvas;
			this.skillSlotFactory = skillSlotFactory;
		}

		private void Start()
		{
			Enable(false);

			subCanvas.WindowsRegistrator.Registrate(this);

			Close.onClick.AddListener(OnClose);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsRegistrator.UnRegistrate(this);
			Close?.onClick.RemoveAllListeners();
		}

		public void SetSkills(Skills skills)
		{
			currentSkills = skills;

			if (IsShowing)
			{
				UpdateUI();
			}
		}

		private void UpdateUI()
		{
		}

		public override void Show(UnityAction callback = null)
		{
			UpdateUI();
			base.Show(callback);
		}

		private void OnClose()
		{
			Hide();
		}
	}
}