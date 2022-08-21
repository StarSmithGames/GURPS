using Game.Managers.FactionManager;

using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Zenject;

namespace Game.Systems.BattleSystem
{
	//required tag -> "Ignore Raycast"
	public class BattleZone : SerializedMonoBehaviour
	{
		[SerializeField] private List<IBattlable> battlables = new List<IBattlable>();

		private Battle battle;

		private BattleSystem battleSystem;
		private FactionManager factionManager;

		[Inject]
		private void Construct(BattleSystem battleSystem, FactionManager factionManager)
		{
			this.battleSystem = battleSystem;
			this.factionManager = factionManager;
		}

		private void Start()
		{
			Assert.IsTrue(battlables.Count > 0, "Battle Zone is Empty, Add Enemies.");
		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			IBattlable battlable = other.GetComponent<IBattlable>();
			if (battlable != null)
			{
				if (IsEnemyInZone(battlable))
				{
					var entities = new List<IBattlable>(battlables);
					entities.Add(battlable);

					var battle = new Battle(entities);
					battle.Initialization();

					Disable();
				}
			}
		}

		protected virtual void OnTriggerExit(Collider other)
		{
		}

		private bool IsEnemyInZone(IBattlable battlable)
		{
			if (battlable is IFactionable factionable)
			{
				for (int i = 0; i < battlables.Count; i++)
				{
					if (factionManager.CheckRelationShip(battlables[i] as IFactionable, RelationType.Enemy, factionable))
					{
						return true;
					}
				}
			}

			return false;
		}

		private void Disable()
		{
			enabled = false;
		}
	}
}