using Game.Entities.Models;

using System.Collections;

using UnityEngine;

namespace Game.Entities.AI
{
	public interface IBehavior
	{
		IBehaviorState CurrentState { get; }
		IEnumerator Tick();
	}

	public abstract class Behavior : IBehavior
	{
		public IBehaviorState CurrentState { get; protected set; }

		public abstract IEnumerator Tick();
	}

	public class PassiveBehavior : Behavior
	{
		public override IEnumerator Tick()
		{
			yield return null;
		}
	}

	//public class CompanionBehavior : IBehavior
	//{
	//	public IBehaviorState CurrentState { get; }

	//	public IEnumerator Tick()
	//	{
	//		yield return null;
	//	}
	//}

	public interface IBehaviorState
	{
		IEnumerator Tick();
	}

	public class IdleState : IBehaviorState
	{
		public IEnumerator Tick()
		{
			yield return null;
		}
	}


	//https://github.com/Black-Horizon-Studios/Emerald-AI/wiki/Behaviors-and-Confidence-Levels
	public enum ConfidenceLevel
	{
		Coward,
		Brave,
		Foolhardy,
	}
}