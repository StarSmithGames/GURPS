using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem.Skills;
using Game.UI.CanvasSystem;
using Game.UI.Windows;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.HUD
{
	public class WindowSkillDeck : WindowBase
	{
		[field: SerializeField] public Button Close { get; private set; }
		[field: SerializeField] public Transform LeftContent { get; private set; }

		private bool isInitialized = false;
		private List<UISkillPack> skills = new List<UISkillPack>();
		private Skills currentSkills;

		private UISubCanvas subCanvas;
		private UISkillPack.Factory skillPackFactory;

		[Inject]
		private void Construct(UISubCanvas subCanvas, UISkillPack.Factory skillPackFactory)
		{
			this.subCanvas = subCanvas;
			this.skillPackFactory = skillPackFactory;
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
			if (!isInitialized)
			{
				LeftContent.DestroyChildren();
			}

			var groups = currentSkills.GetSkillGroupsByLevel();

			CollectionExtensions.Resize(groups, skills,
			() =>
			{
				var skill = skillPackFactory.Create();
				skill.transform.SetParent(LeftContent);

				return skill;
			},
			() =>
			{
				return skills.Last();
			});

			for (int i = 0; i < groups.Count; i++)
			{
				skills[i].SetGroup(groups[i]);
			}
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