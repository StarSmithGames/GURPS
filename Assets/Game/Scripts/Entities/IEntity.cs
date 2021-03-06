using EPOOutline;

using Game.Systems.CameraSystem;
using Game.Systems.DamageSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Entities
{
	public interface IEntity :
		ISheetable, IPathfinderable,
		IInteractable, IObservable,
		IDamegeable, IKillable
	{
		MonoBehaviour MonoBehaviour { get; }

		CameraPivot CameraPivot { get; }

		AnimatorControl AnimatorControl { get; }

		Markers Markers { get; }
		Outlinable Outlines { get; }

		TaskSequence TaskSequence { get; }

		void Freeze(bool trigger);
	}
}