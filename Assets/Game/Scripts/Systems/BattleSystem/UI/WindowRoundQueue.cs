using Game.Systems.CameraSystem;
using Game.Systems.SheetSystem;
using Game.UI.Windows;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.BattleSystem
{
	public class WindowRoundQueue : WindowBase
	{
		private List<UITurn> turns = new List<UITurn>();

		private GameObject turnSeparate;
		private UITurn.Factory turnFactory;
		private CameraController cameraController;

		[Inject]
		private void Construct([Inject(Id = "TurnSeparate")] GameObject turnSeparate,
			UITurn.Factory turnFactory,
			CameraController cameraController)
		{
			this.turnSeparate = turnSeparate;
			this.turnFactory = turnFactory;
			this.cameraController = cameraController;

			transform.DestroyChildren();

			turnSeparate.transform.parent = transform;
			turnSeparate.transform.localScale = Vector3.one;
		}

		public override void Enable(bool trigger)
		{
			base.Enable(trigger);
			gameObject.SetActive(trigger);
		}

		public void UpdateTurns(List<Round> rounds)
		{
			int TurnCount()
			{
				int result = 0;
				rounds.ForEach((x) => result += x.Turns.Count);
				return result;
			}

			CollectionExtensions.Resize(TurnCount(), turns,
			() =>
			{
				var element = turnFactory.Create();

				element.onDoubleCick += OnTurnClickChanged;
				element.transform.SetParent(transform);
				element.transform.localScale = Vector3.one;
				return element;
			},
			() =>
			{
				var element = turns.Last();

				element.onDoubleCick -= OnTurnClickChanged;
				element.DespawnIt();
				return element;
			});

			int count = 0;
			for (int i = 0; i < rounds.Count; i++)
			{
				for (int j = 0; j < rounds[i].Turns.Count; j++)
				{
					turns[count].SetEntity(rounds[i].Turns[j].Initiator as ISheetable);

					if (count == 0)
					{
						turns[count].Select();
					}
					else
					{
						turns[count].Diselect();
					}

					count++;
				}
			}

			turnSeparate.transform.SetSiblingIndex(rounds.First().Turns.Count);//rounds need to be 2 or need factory for turnSeparate
		}

		private void OnTurnClickChanged(UITurn turn)
		{
			//cameraController.SetFollowTarget(turn.CurrentBattlable.CameraPivot);
			cameraController.CameraToHome();
		}
	}
}