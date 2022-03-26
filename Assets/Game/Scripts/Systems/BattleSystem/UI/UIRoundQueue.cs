using CMF;

using Game.Systems.BattleSystem;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

public class UIRoundQueue : MonoBehaviour
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

	public void UpdateTurns(List<Round> rounds)
	{
		int TurnCount()
		{
			int result = 0;
			rounds.ForEach((x) => result += x.Turns.Count);
			return result;
		}

		ResizeTurnsTo(TurnCount());

		int count = 0;
		for (int i = 0; i < rounds.Count; i++)
		{
			for (int j = 0; j < rounds[i].Turns.Count; j++)
			{
				turns[count].SetEntity(rounds[i].Turns[j].Initiator);

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

	private void ResizeTurnsTo(int size)
	{
		int diff = size - turns.Count;
		if (diff != 0)
		{
			if (diff > 0)//need spawn
			{
				for (int i = 0; i < diff; i++)
				{
					turns.Add(CreateElement());
				}
			}
			else//need despawn
			{
				diff += turns.Count;

				for (int i = turns.Count - 1; i >= diff; i--)
				{
					RemoveElement(i);
				}
			}

			UITurn CreateElement()
			{
				UITurn element = turnFactory.Create();

				element.onDoubleCick += OnTurnClickChanged;

				element.transform.SetParent(transform);
				element.transform.localScale = Vector3.one;

				return element;
			}

			void RemoveElement(int index)
			{
				turns[index].onDoubleCick -= OnTurnClickChanged;

				turns[index].DespawnIt();
				turns.RemoveAt(index);
			}
		}
	}

	private void OnTurnClickChanged(UITurn turn)
{
		cameraController.SetFollowTarget(turn.CurrentEntity.CameraPivot);
	}
}