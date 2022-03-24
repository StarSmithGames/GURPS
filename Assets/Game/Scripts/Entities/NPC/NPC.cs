using Game.Managers.GameManager;
using Game.Systems.BattleSystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Zenject;
using Game.Entities;
using Game.Managers.CharacterManager;
using Sirenix.Utilities;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IEntity, IObservable
{
	public EntityData EntityData => entityData;
	[SerializeField] private EntityData entityData;

	public Transform Transform => transform;
	public NavigationController Navigation { get; private set; }
	public CharacterController3D Controller { get; private set; }

	private FieldOfView fov;
	private UIManager uiManager;
	private GameManager gameManager;
	private CharacterManager characterManager;
	private BattleSystem battleSystem;

	[Inject]
	private void Construct(
		FieldOfView fov,
		NavigationController navigationController,
		CharacterController3D controller,
		UIManager uiManager,
		GameManager gameManager,
		CharacterManager characterManager,
		BattleSystem battleSystem)
	{
		this.fov = fov;
		Navigation = navigationController;
		Controller = controller;
		this.uiManager = uiManager;
		this.gameManager = gameManager;
		this.characterManager = characterManager;
		this.battleSystem = battleSystem;

		fov.StartView();
	}

	private void Update()
	{
		if(gameManager.CurrentGameState != GameState.PreBattle &&
			gameManager.CurrentGameState != GameState.Battle)
		{
			if (fov.visibleTargets.Count > 0)
			{
				fov.StopView();
				List<IEntity> entities = new List<IEntity>();

				characterManager.Party.Characters.ForEach((x) =>
				{
					entities.Add(x);
				});
				entities.Add(this);

				battleSystem.StartBattle(entities);
			}
		}
	}
	
	public void Freeze()
	{
		Controller.Freeze();
	}

	public void UnFreeze()
	{
		Controller.UnFreeze();
	}

	public void StartObserve()
	{
		uiManager.Battle.ShowEntityInformation(this);
	}

	public void Observe()
	{
	}

	public void EndObserve()
	{
		uiManager.Battle.HideEntityInformation();
	}
}