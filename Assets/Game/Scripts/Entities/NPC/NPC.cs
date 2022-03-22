using Game.Managers.GameManager;
using Game.Systems.BattleSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Zenject;
using Game.Entities;

public class NPC : MonoBehaviour, IEntity
{
	public Transform Transform => transform;
	public CharacterController3D Controller { get; private set; }

	private FieldOfView fov;
	private GameManager gameManager;
	private BattleSystem battleSystem;

	[Inject]
	private void Construct(FieldOfView fov, CharacterController3D controller, GameManager gameManager, BattleSystem battleSystem)
	{
		this.fov = fov;
		Controller = controller;
		this.gameManager = gameManager;
		this.battleSystem = battleSystem;

		//fov.StartView();
	}

	private void Update()
	{
		if(gameManager.CurrentGameState != GameState.Battle)
		{
			//if (fov.visibleTargets.Count > 0)
			//{
			//	List<Transform> targets = new List<Transform>(fov.visibleTargets);
			//	fov.StopView();

			//	List<IEntity> enemies = new List<IEntity>();
			//	for (int i = 0; i < targets.Count; i++)
			//	{
			//		Character c = targets[i].GetComponent<Character>();
					
			//		if(c != null)
			//		{
			//			enemies.Add(c);
			//		}
			//	}

			//	battleSystem.StartBattle(enemies, this);
			//}
		}
	}
}