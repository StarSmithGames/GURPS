using Game.Managers.SceneManager;
using Game.Managers.TransitionManager;
using Game.UI.CanvasSystem;
using Game.UI.Windows;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace Game.UI
{
	public class UIButtonSwitchScene : UIButton
	{
		[HideLabel]
		public SceneName sceneName;

		[field: SerializeField] public Transitions In { get; private set; }
		[field: SerializeField] public Transitions Out { get; private set; }

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
			Button?.onClick.RemoveAllListeners();
		}

		private void Click()
		{
			globalCanvas.WindowsRegistrator.GetAs<WindowInfinityLoading>().Show(sceneName.GetScene(), In, Out);
		}
	}
}