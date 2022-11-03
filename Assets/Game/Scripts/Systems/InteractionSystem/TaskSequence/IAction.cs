using Game.Entities.Models;
using UnityEngine.Events;

namespace Game.Systems.InteractionSystem
{
	public interface IAction : IExecutable { }

	public class GoToPointAction : IAction
	{
		private InteractionPoint point;
		private UnityAction action;

		public GoToPointAction(InteractionPoint point, UnityAction action)
		{
			this.point = point;
			this.action = action;
		}

		public void Execute(IEntityModel model)
		{
			if (point.IsInRange(model.Transform.position))
			{
				action?.Invoke();
			}
			else
			{
				model.TaskSequence
					.Append(new GoToTaskAction(model, point.GetIteractionPosition(model)))
					.Append(() => action?.Invoke());
				model.TaskSequence.Execute();
			}
		}

		public void Execute()
		{
			action?.Invoke();
		}
	}
}