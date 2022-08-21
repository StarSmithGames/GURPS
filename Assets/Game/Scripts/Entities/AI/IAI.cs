using Game.Entities.Models;

using System.Collections;

using UnityEngine;

namespace Game.Entities.AI
{
	public interface IAI
	{
		Brain Brain { get; }
	}

	public abstract class Brain
	{
		public bool IsAlive { get; protected set; }
		public bool IsBrainProcess => brainCoroutine != null;
		private Coroutine brainCoroutine = null;

		public virtual IBehavior CurrentBehavior { get; protected set; }

		private MonoBehaviour owner;

		public Brain(MonoBehaviour owner)
		{
			this.owner = owner;
		}

		public virtual void StartBrain()
		{
			if (!IsBrainProcess)
			{
				brainCoroutine = owner.StartCoroutine(BrainWork());
			}
		}
		protected abstract IEnumerator BrainWork();
		public virtual void StopBrain()
		{
			if (IsBrainProcess)
			{
				owner.StopCoroutine(brainCoroutine);
				brainCoroutine = null;
			}
		}
	}

	public class DummyAI : Brain
	{
		private PassiveBehavior PassiveBehavior
		{
			get
			{
				if(passiveBehavior == null)
				{
					passiveBehavior = new PassiveBehavior();
				}

				return passiveBehavior;
			}
		}
		private PassiveBehavior passiveBehavior;

		private DummyModel dummy;

		public DummyAI(DummyModel dummy) : base(dummy)
		{
			this.dummy = dummy;

			IsAlive = true;
		}

		protected override IEnumerator BrainWork()
		{
			while (IsAlive)
			{
				if (dummy.InBattle)
				{
					if (dummy.barksInBattle.TreeData.isFirstTime)
					{
						dummy.BarkInBattle();
					}

					yield return null;
				}
				else
				{
					CurrentBehavior = PassiveBehavior;
					yield return CurrentBehavior.Tick();
				}
			}
		}
	}
}