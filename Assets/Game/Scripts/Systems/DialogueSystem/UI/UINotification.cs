using UnityEngine;

using Zenject;

namespace Game.Systems.DialogueSystem
{
	public class UINotification : PoolableObject
	{
		[field: SerializeField] public TMPro.TextMeshProUGUI Text;
	
		public class Factory : PlaceholderFactory<UINotification> { }
	}
}