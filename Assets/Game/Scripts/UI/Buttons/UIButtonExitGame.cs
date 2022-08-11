using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class UIButtonExitGame : MonoBehaviour
	{
		[field: SerializeField] public Button Button { get; private set; }

		private void Start()
		{
			Button.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
			Button?.onClick.RemoveAllListeners();
		}

		private void OnClick()
		{
			Application.Quit();
		}
	}
}