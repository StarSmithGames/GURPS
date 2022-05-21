using NodeCanvas.Framework;

using UnityEngine;

namespace Game.Systems.DialogueSystem.Nodes
{
	public abstract class RequirementConditionTask : ConditionTask
	{
		[HideInInspector] public IRequirement requirement;
	}
}