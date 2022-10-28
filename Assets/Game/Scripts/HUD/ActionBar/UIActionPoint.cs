using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.HUD
{
	public class UIActionPoint : PoolableObject
	{
		[field: SerializeField] public Image Active { get; private set; }
		[field: SerializeField] public Image Inactive { get; private set; }

		public void Enable(bool trigger)
		{
			Active.gameObject.SetActive(trigger);
			Inactive.gameObject.SetActive(!trigger);
		}


		public class Factory : PlaceholderFactory<UIActionPoint> { }
	}
}