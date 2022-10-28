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

		private List<UISkill> skills = new List<UISkill>();
		private Skills currentSkills;

		private UISubCanvas subCanvas;
		private UISkill.Factory skillFactory;

		[Inject]
		private void Construct(UISubCanvas subCanvas, UISkill.Factory skillFactory)
		{
			this.subCanvas = subCanvas;
			this.skillFactory = skillFactory;
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