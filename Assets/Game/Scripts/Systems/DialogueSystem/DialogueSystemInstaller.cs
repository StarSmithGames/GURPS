using UnityEngine;

using Zenject;

namespace Game.Systems.DialogueSystem
{
	[CreateAssetMenu(fileName = "DialogueSystemInstaller", menuName = "Installers/DialogueSystemInstaller")]
	public class DialogueSystemInstaller : ScriptableObjectInstaller<DialogueSystemInstaller>
	{
		public UIChoice choicePrefab;
		public DialogueSystemHandler.Settings settings;


		public override void InstallBindings()
		{
			Container.BindInstance(settings);

			Container.BindInterfacesAndSelfTo<DialogueSystemHandler>().AsSingle();

			Container
				.BindFactory<UIChoice, UIChoice.Factory>()
				.FromMonoPoolableMemoryPool((x) => x.WithInitialSize(3)
				.FromComponentInNewPrefab(choicePrefab));
		}
	}
}