using Game.UI;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Zenject;

namespace Game.HUD
{
	public sealed class UIAction : PoolableObject
	{


		public class Factory : PlaceholderFactory<UIAction> { }
	}
}