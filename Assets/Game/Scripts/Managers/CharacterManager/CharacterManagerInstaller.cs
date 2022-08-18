using Game.Entities;
using Game.Entities.Models;

using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

using Zenject;

namespace Game.Managers.CharacterManager
{
	[CreateAssetMenu(fileName = "CharacterManagerInstaller", menuName = "Installers/CharacterManagerInstaller")]
	public class CharacterManagerInstaller : ScriptableObjectInstaller<CharacterManagerInstaller>
	{
		[HideLabel]
		public CharacterDatabase database;

		public override void InstallBindings()
		{
			Container.BindInstance(database);

			Container.BindInterfacesAndSelfTo<CharacterManager>().AsSingle();
		}
	}

	[System.Serializable]
	public class CharacterDatabase
	{
		public CharacterData player;
		public List<CompanionData> companions = new List<CompanionData>();
		public List<NPCData> npc = new List<NPCData>();
	}
}