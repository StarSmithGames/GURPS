using NodeCanvas.Framework;

using UnityEngine;

namespace Game.Systems.DialogueSystem.Nodes
{
	public abstract class RequirementConditionTask : ConditionTask
	{
		public IRequirement Requirement { get; protected set; }
	}
}