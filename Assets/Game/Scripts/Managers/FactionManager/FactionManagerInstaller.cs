using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;

namespace Game.Managers.FactionManager
{
	[CreateAssetMenu(fileName = "FactionManagerInstaller", menuName = "Installers/FactionManagerInstaller")]
    public class FactionManagerInstaller : ScriptableObjectInstaller<FactionManagerInstaller>
    {
		public List<Relation> relations = new List<Relation>();

		public override void InstallBindings()
		{
			Container.BindInstance(relations).WhenInjectedInto<FactionManager>();
			Container.Bind<FactionManager>().AsSingle().NonLazy();
		}

		private void Validate()
		{

		}
	}

	[System.Serializable]
	public class Faction
	{
		[ValueDropdown("SelectFaction", DropdownTitle = "Faction Selection")]
		[HideLabel]
		public string faction;

		private static IEnumerable SelectFaction()
		{
			return FactionManager.Factions;
		}
	}

	[System.Serializable]
	public class Relation
	{
		[HorizontalGroup]
		[HideLabel]
		public Faction factionA;
		[HorizontalGroup]
		[HideLabel]
		public RelationType relation;
		[HorizontalGroup]
		[HideLabel]
		public Faction factionB;
	}

	public enum RelationType
	{
        Neutral,
        Enemy,
        Friendly,
	}
}