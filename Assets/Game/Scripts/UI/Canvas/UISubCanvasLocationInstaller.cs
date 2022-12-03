using Game.HUD;
using Game.Systems.InventorySystem;
using UnityEngine;
using Zenject;

namespace Game.UI.CanvasSystem
{
	[CreateAssetMenu(fileName = "UISubCanvasLocationInstaller", menuName = "Installers/UISubCanvasLocationInstaller")]
	public class UISubCanvasLocationInstaller : ScriptableObjectInstaller<UISubCanvasLocationInstaller>
	{
		[Header("Character")]
		public UIAvatar avatarPrefab;
		[Header("Inventory")]
		public UIDragItem itemCursorPrefab;
		public UIContainerWindow chestPopupWindowPrefab;
		[Header("Battle")]
		public UITurn turnPrefab;
		public GameObject turnSeparatePrefab;

		[Header("Effects")]
		public UIEffect effectPrefab;

		[Header("Actions")]
		public UISlotAction actionSlotPrefab;
		public UIActionPoint actionPointPrefab;

		[Header("Skills")]
		public UISlotSkill skillSlotPrefab;
		public UISkillPack skillPackPrefab;

		public override void InstallBindings()
		{
			BindInventory();

			BindBattleSystem();

			BindAvatars();
			BindActionPoints();
			BindEffects();
			BindActions();
			BindSkills();
		}


		private void BindInventory()
		{
			Container.BindFactory<UIContainerWindow, UIContainerWindow.Factory>()
			   .FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
			   .FromComponentInNewPrefab(chestPopupWindowPrefab)
			   .UnderTransform((x) => x.Container.Resolve<UISubCanvas>().Windows));

			Container.BindInstance(Container.InstantiatePrefabForComponent<Systems.InventorySystem.UIDragItem>(itemCursorPrefab));

			Container.BindInterfacesAndSelfTo<ContainerSlotHandler>().AsSingle();
		}

		private void BindBattleSystem()
		{
			Container.BindFactory<UITurn, UITurn.Factory>()
					.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(6)
					.FromComponentInNewPrefab(turnPrefab)
					.UnderTransform((x) => x.Container.Resolve<UISubCanvas>().transform));

			Container.BindInstance(Container.InstantiatePrefab(turnSeparatePrefab)).WithId("TurnSeparate");
		}

		private void BindAvatars()
		{
			Container.BindFactory<UIAvatar, UIAvatar.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(2)
				.FromComponentInNewPrefab(avatarPrefab)
				.UnderTransform((x) => x.Container.Resolve<UISubCanvas>().transform));
		}

		private void BindActionPoints()
		{
			Container.BindFactory<UIActionPoint, UIActionPoint.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(7)
				.FromComponentInNewPrefab(actionPointPrefab)
				.UnderTransform((x) => x.Container.Resolve<UISubCanvas>().transform));
		}

		private void BindEffects()
		{
			Container.BindFactory<UIEffect, UIEffect.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(3)
				.FromComponentInNewPrefab(effectPrefab)
				.UnderTransform((x) => x.Container.Resolve<UISubCanvas>().transform));
		}

		private void BindActions()
		{
			Container.BindFactory<UISlotAction, UISlotAction.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(15)
				.FromComponentInNewPrefab(actionSlotPrefab)
				.UnderTransform((x) => x.Container.Resolve<UISubCanvas>().transform));
		}

		private void BindSkills()
		{
			Container.BindFactory<UISlotSkill, UISlotSkill.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(3)
				.FromComponentInNewPrefab(skillSlotPrefab)
				.UnderTransform((x) => x.Container.Resolve<UISubCanvas>().transform));

			Container.BindFactory<UISkillPack, UISkillPack.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(1)
				.FromComponentInNewPrefab(skillPackPrefab)
				.UnderTransform((x) => x.Container.Resolve<UISubCanvas>().transform));
		}
	}
}