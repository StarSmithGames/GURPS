using System;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Managers.TransitionManager
{
	public class TransitionManager : IInitializable, IDisposable
	{
		private ITransition transition;

		private UIFadeTransition.Factory fadeFactory;

		public TransitionManager(AsyncManager asyncManager, UIFadeTransition.Factory fadeFactory)
		{
			this.fadeFactory = fadeFactory;
		}

		public void Initialize()
		{

		}

		public void Dispose()
		{

		}

		public void TransitionIn(Transitions type, UnityAction callback = null)
		{
			if(transition != null)
			{
				if (transition.IsInProgress)
				{
					transition.Terminate();
				}

				transition = null;
			}

			if(type == Transitions.Fade)
			{
				transition = fadeFactory.Create();
			}
			else
			{
				return;
			}

			transition.In(callback);
		}
		public void TransitionOut(Transitions type, UnityAction callback = null)
		{
			transition.Out(callback);
		}
	}
}