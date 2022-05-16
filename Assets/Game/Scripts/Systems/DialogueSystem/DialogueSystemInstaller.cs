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
		public UIChoice choicePrefab;

		public override void InstallBindings()
		{
			Container.BindInstance(settings);
			Container.BindInstance(barkSettings);

			Container.BindInterfacesAndSelfTo<DialogueSystem>().AsSingle();
			Container.BindInterfacesAndSelfTo<DialogueSystemHandler>().AsSingle();
			Container.Bind<DialogueTreeController>().FromNewComponentOnNewGameObject().WhenInjectedInto<DialogueSystem>();//:/

			Container
				.BindFactory<UIChoice, UIChoice.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(3)
				.FromComponentInNewPrefab(choicePrefab));
		}
	}
}