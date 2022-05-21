using NodeCanvas.DialogueTrees;

using UnityEngine;

using Zenject;

namespace Game.Systems.DialogueSystem
{
	[CreateAssetMenu(fileName = "DialogueSystemInstaller", menuName = "Installers/DialogueSystemInstaller")]
	public class DialogueSystemInstaller : ScriptableObjectInstaller<DialogueSystemInstaller>
	{
		public DialogueSystemHandler.Settings settings;
		public Barker.Settings barkSettings;
		[Space]
		public UISubtitle subtitlePrefab;
		public UINotification notificationPrefab;
		public UIChoice choicePrefab;

		public override void InstallBindings()
		{
			Container.DeclareSignal<StartDialogueSignal>();
			Container.DeclareSignal<EndDialogueSignal>();

			Container.BindInstance(settings);
			Container.BindInstance(barkSettings);

			Container.BindInterfacesAndSelfTo<DialogueSystem>().AsSingle();
			Container.BindInterfacesAndSelfTo<DialogueSystemHandler>().AsSingle();
			Container.Bind<DialogueTreeController>().FromNewComponentOnNewGameObject().WhenInjectedInto<DialogueSystem>();//>:/

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