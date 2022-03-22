using DG.Tweening;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

using static Game.Systems.BattleSystem.BattleSystem;

namespace Game.Systems.BattleSystem
{
	public class UIBattle : MonoBehaviour
	{
		[field: SerializeField] public WindowInformer CommenceBattle { get; private set; }
		[field: SerializeField] public UIRoundQueue RoundQueue { get; private set; }

		private Battle currentBattle;
		private List<UITurn> turns = new List<UITurn>();

		private UITurn.Factory turnFactory;

		[Inject]
		private void Construct(UITurn.Factory turnFactory)
		{
			this.turnFactory = turnFactory;
		}

		public void SetBattle(Battle battle)
		{
			currentBattle = battle;

			UpdateUI();
		}

		public void ShowCommenceBattle()
		{
			Sequence sequence = DOTween.Sequence();

			CommenceBattle.CanvasGroup.alpha = 0;

			sequence
				.AppendCallback(() => CommenceBattle.gameObject.SetActive(true))
				.Append(CommenceBattle.CanvasGroup.DOFade(1, 0.25f))
				.AppendInterval(1.5f)
				.Append(CommenceBattle.CanvasGroup.DOFade(0, 0.1f))
				.AppendCallback(() => CommenceBattle.gameObject.SetActive(false));
		}
		
		private void UpdateUI()
		{
			if(currentBattle == null)
			{
				RoundQueue.gameObject.SetActive(false);

				return;
			}

			ResizeTurns();

			for (int i = 0; i < turns.Count; i++)
			{
				turns[i].SetEntity(currentBattle.rounds.First().turns[i].initiator);
			}

			RoundQueue.gameObject.SetActive(true);
		}

		private void ResizeTurns()
		{
			int diff = currentBattle.rounds.First().turns.Count - turns.Count;
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