using Game.UI;

using NodeCanvas.DialogueTrees;

using UnityEngine;

using Zenject;

namespace Game.Systems.DialogueSystem
{
	[CreateAssetMenu(fileName = "DialogueSystemInstaller", menuName = "Installers/DialogueSystemInstaller")]
	public class DialogueSystemInstaller : ScriptableObjectInstaller<DialogueSystemInstaller>
	{
		public DialogueSystemWindow.Settings settings;
		public Barker.Settings barkSettings;
		[Space]
		public DialogueSystemWindow dialogueSystemWindowPrefab;
		public UISubtitle subtitlePrefab;
		public UINotification notificationPrefab;
		public UIChoice choicePrefab;

		public override void InstallBindings()
		{
			Container.DeclareSignal<SignalStartDialogue>();
			Container.DeclareSignal<SignalEndDialogue>();

			Container.BindInstance(settings).WhenInjectedInto<DialogueSystemWindow>();
			Container.BindInstance(barkSettings);

			Container.Bind<DialogueSystemWindow>()
				.FromComponentInNewPrefab(dialogueSystemWindowPrefab)
				.UnderTransform(x => x.Container.Resolve<UISubCanvas>().Windows)
				.AsSingle()
				.NonLazy();

			Container.BindInterfacesAndSelfTo<DialogueSystem>().AsSingle();
			Container.Bind<DialogueTreeController>().FromNewComponentOnNewGameObject().WhenInjectedInto<DialogueSystem>();//>:/

			//Factories
			Container
				.BindFactory<UISubtitle, UISubtitle.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(10)
				.FromComponentInNewPrefab(subtitlePrefab));

			Container
				.BindFactory<UINotification, UINotification.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(2)
				.FromComponentInNewPrefab(notificationPrefab));

			Container
				.BindFactory<UIChoice, UIChoice.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(3)
				.FromComponentInNewPrefab(choicePrefab));
		}
	}
}