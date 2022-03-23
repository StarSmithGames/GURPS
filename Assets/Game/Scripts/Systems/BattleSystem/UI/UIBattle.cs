using DG.Tweening;

using System.Collections.Generic;
using System.Linq;

using UnityEditor.VersionControl;

using UnityEngine;

using Zenject;

using static Game.Systems.BattleSystem.BattleSystem;

namespace Game.Systems.BattleSystem
{
	public class UIBattle : MonoBehaviour
	{
		[field: SerializeField] public UIRoundQueue RoundQueue { get; private set; }
		[field: SerializeField] public UIEntityInformation EntityInformation { get; private set; }
		[field: SerializeField] public UISkipTurn SkipTurn { get; private set; }
		[field: SerializeField] public UIMessages Messages { get; private set; }


		private Battle currentBattle;
		private List<UITurn> turns = new List<UITurn>();

		private UITurn.Factory turnFactory;
		private GameObject turnSeparate;

		[Inject]
		private void Construct(UITurn.Factory turnFactory, [Inject(Id = "TurnSeparate")] GameObject turnSeparate)
		{
			this.turnFactory = turnFactory;
			this.turnSeparate = turnSeparate;

			RoundQueue.content.DestroyChildren();

			turnSeparate.transform.parent = RoundQueue.transform;
			turnSeparate.transform.localScale = Vector3.one;
		}

		public void SetBattle(Battle battle)
		{
			if(currentBattle != null)
			{
				currentBattle.onBattleUpdated -= UpdateUI;
			}

			currentBattle = battle;

			if (currentBattle != null)
			{
				currentBattle.onBattleUpdated += UpdateUI;
			}

			UpdateUI();
		}

		public void ShowEntityInformation(IEntity entity)
		{
			EntityInformation.SetEntity(entity);
			EntityInformation.gameObject.SetActive(true);
		}
		public void HideEntityInformation()
		{
			EntityInformation.SetEntity(null);
			EntityInformation.gameObject.SetActive(false);
		}



		private void UpdateUI()
		{
			if(currentBattle == null)
			{
				RoundQueue.gameObject.SetActive(false);
				SkipTurn.gameObject.SetActive(false);

				return;
			}

			ResizeTurnsTo(currentBattle.TurnCount);

			int count = 0;
			for (int i = 0; i < currentBattle.rounds.Count; i++)
			{
				for (int j = 0; j < currentBattle.rounds[i].turns.Count; j++)
				{
					turns[count].SetEntity(currentBattle.rounds[i].turns[j].initiator);

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

			turnSeparate.transform.SetSiblingIndex(currentBattle.rounds.First().turns.Count);//rounds need to be 2 or need factory

			RoundQueue.gameObject.SetActive(true);
		}

		private void ResizeTurnsTo(int size)
		{
			int diff = size - turns.Count;
			if (diff != 0)
			{
				UITurn CreateElement()
				{
					UITurn element = turnFactory.Create();

					element.transform.SetParent(RoundQueue.content);

					element.transform.localScale = Vector3.one;

					return element;
				}

				if (diff > 0)//need add
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
						turns[i].DespawnIt();
						turns.RemoveAt(i);
					}
				}
			}
		}
	}
}