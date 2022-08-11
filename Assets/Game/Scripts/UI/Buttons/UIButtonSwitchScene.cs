using Game.Managers.SceneManager;
using Game.Managers.TransitionManager;
using Game.UI.Windows;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.UI
{
	public class UIButtonSwitchScene : MonoBehaviour
	{
		[field: SerializeField] public Scenes GoTo { get; private set; }
		[field: SerializeField] public Transitions In { get; private set; }
		[field: SerializeField] public Transitions Out { get; private set; }

		[field: SerializeField] public Button Button { get; private set; }

		private UIGlobalCanvas globalCanvas;

		[Inject]
		private void Construct(UIGlobalCanvas globalCanvas)
		{
			this.globalCanvas = globalCanvas;
		}

		private void Start()
		{
			Button.onClick.AddListener(Click);
		}

		private void OnDestroy()
		{
			Button.onClick.RemoveAllListeners();
		}

		private void Click()
		{
			globalCanvas.WindowsManager.GetAs<WindowInfinityLoading>().Show(GoTo, In, Out);
		}
	}
}