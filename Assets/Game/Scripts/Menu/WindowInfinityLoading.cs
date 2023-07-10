using Game.Managers.SceneManager;
using Game.Managers.TransitionManager;

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using Zenject;
using UnityEngine.Events;
using Game.UI.CanvasSystem;
using Game.UI.Windows;
using Game.Managers.GameManager;

namespace Game.Menu
{
	public class WindowInfinityLoading : WindowBase
	{
		[field: SerializeField] public Image Ilustratration { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Progress { get; private set; }
		[field: SerializeField] public CanvasGroup ContinueCanvasGroup { get; private set; }
		[field: SerializeField] public Button Continue { get; private set; }

		private Transitions transitionsIn;
		private Transitions transitionOut;
		private bool isProgressing = false;

		private UIGlobalCanvas globalCanvas;
		private SceneManager sceneManager;
		private GameManager gameManager;
		private TransitionManager transitionManager;
		private AsyncManager asyncManager;
		private Settings settings;

		[Inject]
		public void Construct(UIGlobalCanvas globalCanvas,
			SceneManager sceneManager,
			GameManager gameManager,
			TransitionManager transitionManager, AsyncManager asyncManager, GlobalSettings settings)
		{
			this.globalCanvas = globalCanvas;
			this.sceneManager = sceneManager;
			this.gameManager = gameManager;
			this.transitionManager = transitionManager;
			this.asyncManager = asyncManager;
			this.settings = settings.infinityLoadingSettings;
		}

		private void Start()
		{
			Enable(false);

			globalCanvas.WindowsRegistrator.Registrate(this);

			Continue.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
			globalCanvas.WindowsRegistrator.UnRegistrate(this);

			Continue?.onClick.RemoveAllListeners();
		}

		public void Show(string sceneName, UnityAction callback = null)
		{
			this.transitionsIn = Transitions.Fade;
			this.transitionOut = Transitions.Fade;

			gameManager.ChangeState(GameState.Loading);

			//show after transition
			transitionManager
				.TransitionIn(transitionsIn,
				() =>
				{
					Show();
					//start progress
					isProgressing = true;

					asyncManager.StartCoroutine(LoadScene(sceneName, callback));
				});
		}

		private IEnumerator LoadScene(string scene, UnityAction callback = null)
		{
			float targetValue;
			float currentValue = 0f;

			ContinueCanvasGroup.alpha = 0f;
			Continue.enabled = false;
			Progress.enabled = true;

			sceneManager.SwitchScene(scene, settings.allowScene);

			while (isProgressing)
			{
				if (sceneManager.ProgressHandle == null)
				{
					Progress.text = $"{Mathf.Round(currentValue * 100f)}%";
					yield return null;
				}
				else
				{
					targetValue = sceneManager.ProgressHandle.GetProgressPercent() / 0.9f;

					currentValue = Mathf.MoveTowards(currentValue, targetValue, settings.progressAnimationMultiplier * Time.deltaTime);
					Progress.text = $"{Mathf.Round(currentValue * 100f)}%";

					if (Mathf.Approximately(currentValue, 1))
					{
						callback?.Invoke();
						yield return null;

						//end progress
						isProgressing = false;

						Progress.enabled = false;
						Continue.enabled = true;
						ContinueCanvasGroup.DOFade(1, 0.2f);
					}

					yield return null;
				}
			}

			yield return null;
		}

		private void OnClick()
		{
			Continue.enabled = false;

			ContinueCanvasGroup.DOKill(true);

			//allow scene
			if (!sceneManager.ProgressHandle.IsAllowed)
			{
				sceneManager.ProgressHandle.AllowSceneActivation();
			}

			Hide(() => transitionManager.TransitionOut(transitionOut));
		}
		

		[System.Serializable]
		public class Settings
		{
			public bool allowScene = false;
			[Range(0, 1f)]
			public float progressAnimationMultiplier = 0.25f;
		}
	}
}