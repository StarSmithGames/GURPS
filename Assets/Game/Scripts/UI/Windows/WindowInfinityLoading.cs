using Game.Managers.SceneManager;
using Game.Managers.TransitionManager;

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using Zenject;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Game.UI.Windows
{
	public class WindowInfinityLoading : MonoBehaviour, IWindow
	{
		public bool IsShowing { get; private set; }

		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
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
		private TransitionManager transitionManager;
		private AsyncManager asyncManager;
		private Settings settings;

		[Inject]
		public void Construct(UIGlobalCanvas globalCanvas, SceneManager sceneManager,
			TransitionManager transitionManager, AsyncManager asyncManager, GlobalSettings settings)
		{
			this.globalCanvas = globalCanvas;
			this.sceneManager = sceneManager;
			this.transitionManager = transitionManager;
			this.asyncManager = asyncManager;
			this.settings = settings.infinityLoadingSettings;
		}

		private void Start()
		{
			CanvasGroup.alpha = 0f;
			CanvasGroup.blocksRaycasts = false;
			CanvasGroup.interactable = false;

			globalCanvas.WindowsManager.Register(this);

			Continue.onClick.AddListener(Click);
		}

		private void OnDestroy()
		{
			globalCanvas.WindowsManager.UnRegister(this);

			Continue?.onClick.RemoveAllListeners();
		}

		public void Enable(bool trigger)
		{
			CanvasGroup.alpha = trigger ? 1f : 0f; 

			CanvasGroup.blocksRaycasts = trigger;
			CanvasGroup.interactable = trigger;
		}

		public void Show(Scenes scene, Transitions transitionsIn, Transitions transitionOut)
		{
			this.transitionsIn = transitionsIn;
			this.transitionOut = transitionOut;

			ShowAfterTransition(SceneStorage.GetSceneName(scene));
		}

		public void Show(string sceneName, Transitions transitionsIn, Transitions transitionOut)
		{
			this.transitionsIn = transitionsIn;
			this.transitionOut = transitionOut;

			ShowAfterTransition(sceneName);
		}

		public void Show(UnityAction callback = null)
		{
			CanvasGroup.blocksRaycasts = true;
			CanvasGroup.interactable = true;

			Progress.enabled = true;

			Continue.gameObject.SetActive(false);
			CanvasGroup.DOFade(1, 0.2f)
				.OnComplete(() => callback?.Invoke());
		}

		public void Hide(UnityAction callback = null)
		{
			CanvasGroup.DOFade(0, 0.1f)
				.OnComplete(() =>
				{
					CanvasGroup.blocksRaycasts = false;
					CanvasGroup.interactable = false;

					callback?.Invoke();
				});
		}

		private void ShowAfterTransition(string scene)
		{
			transitionManager
				.TransitionIn(transitionsIn,
				() => {
					Show();
					//start progress
					isProgressing = true;
					asyncManager.StartCoroutine(ProgressTick());
					sceneManager.SwitchScene(scene, settings.allowScene);
				});
		}


		private IEnumerator ProgressTick()
		{
			float targetValue;
			float currentValue = 0f;

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
						//end progress
						isProgressing = false;

						ContinueCanvasGroup.alpha = 0f;
						Progress.enabled = false;
						Continue.enabled = true;
						Continue.gameObject.SetActive(true);
						ContinueCanvasGroup.DOFade(1, 0.2f);
					}

					yield return null;
				}
			}

			yield return null;
		}

		private void Click()
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