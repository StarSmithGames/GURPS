using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Systems.QuestSystem
{
	[CreateAssetMenu(menuName = "Installers/QuestSystemInstaller", fileName = "QuestSystemInstaller")]
	public class QuestSystemInstaller : ScriptableObjectInstaller<QuestSystemInstaller>
	{
		public List<QuestData> masterBranch = new List<QuestData>();

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<QuestManager>().AsSingle();
		}
	}
}