using Game.Managers.GameManager;
using Game.Managers.PartyManager;
using Game.Systems.NavigationSystem;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.TooltipSystem
{
	public class UITooltipRuller : MonoBehaviour
	{
		[field: SerializeField] public TMPro.TextMeshProUGUI Ruler { get; private set; }

		public TooltipRulerType Type { get; private set; }

		private NavigationController leaderNavigation;
		private NavigationPath customPath;

		private SignalBus signalBus;
		private PartyManager partyManager;

		[Inject]
		private void Construct(SignalBus signalBus,
			PartyManager partyManager)
		{
			this.signalBus = signalBus;
			this.partyManager = partyManager;
		}

		private void Start()
		{
			signalBus?.Subscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			signalBus?.Subscribe<SignalGameStateChanged>(OnGameStateChanged);
		}

		private void OnDestroy()
		{
			signalBus?.Unsubscribe<SignalLeaderPartyChanged>(OnLeaderPartyChanged);
			signalBus?.Unsubscribe<SignalGameStateChanged>(OnGameStateChanged);
		}

		public void Update()
		{
			if (Type == TooltipRulerType.None) return;

			if (Type == TooltipRulerType.CharacterPath)
			{

				Ruler.text =
					Math.Round(leaderNavigation.CurrentPath.Distance, 2) + SymbolCollector.METRE.ToString() + "-" +
					Math.Round(leaderNavigation.FullPath.Distance, 2) + SymbolCollector.METRE.ToString();
			}
			else if(Type == TooltipRulerType.CustomPath)
			{
				Ruler.text = Math.Round(customPath.Distance, 2) + SymbolCollector.METRE.ToString();
			}
		}

		public void SetType(TooltipRulerType type)
		{
			Type = type;
			gameObject.SetActive(Type != TooltipRulerType.None);
		}

		public void SetCustomPath(NavigationPath path)
		{
			customPath = path;
		}

		private void OnLeaderPartyChanged(SignalLeaderPartyChanged signal)
		{
			leaderNavigation = signal.leader.Model.Navigation;
		}

		private void OnGameStateChanged(SignalGameStateChanged signal)
		{
			if (signal.newGameState == GameState.Gameplay)
			{
				leaderNavigation = partyManager.PlayerParty.LeaderParty.Model.Navigation;
			}
		}
	}
	public enum TooltipRulerType
	{
		None,
		CharacterPath,
		CustomPath,
	}
}