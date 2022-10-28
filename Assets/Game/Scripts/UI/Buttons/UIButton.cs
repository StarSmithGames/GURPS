using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class UIButton : MonoBehaviour
	{
		[field: SerializeField] public Button Button { get; private set; }
	}
}