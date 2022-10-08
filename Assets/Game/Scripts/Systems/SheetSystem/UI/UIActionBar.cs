using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem
{
	public sealed class UIActionBar : MonoBehaviour
	{
		[field: SerializeField] public Transform ActionsContent { get; private set; }

		private List<UIAction> actions = new List<UIAction>();

		private UIAction.Factory actionFactory;

		[Inject]
		private void Construct(UIAction.Factory actionFactory)
		{
			this.actionFactory = actionFactory;
		}

		private void Start()
		{
			ActionsContent.DestroyChildren();

			actions.Clear();

			for (int i = 0; i < 15; i++)
			{
				AddAction();
			}
		}

		private void AddAction()
		{
			var action = actionFactory.Create();

			action.SetAction(null);

			action.transform.SetParent(ActionsContent);
			action.transform.localScale = Vector3.one;

			actions.Add(action);
		}
	}
}