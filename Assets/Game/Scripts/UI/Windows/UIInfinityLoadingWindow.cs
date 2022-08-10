using Game.Managers.SceneManager;
using Game.Managers.TransitionManager;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using Zenject;
using UnityEngine.Events;
using Game.UI.GlobalCanvas;

namespace Game.UI.Windows
{
	public class UIInfinityLoadingWindow : MonoBehaviour, IWindow
	{
		public bool IsShowing { get; private set; }

		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
		[field: SerializeField] public Image Ilustratration { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Progress { get; private set; }
		[field: SerializeField] public CanvasGroup ContinueCanvasGroup { get; private set; }
		[field: SerializeField] public Button Continue { get; private set; }

		private Scenes scene;
		private Transitions transitionsIn;
		private Transitions transitionOut;
		private bool isProgressing = false;

		private UIGlobalCanvas globalCanvas;
		private SceneManager sceneManager;
		private TransitionManager transitionManager;
		private AsyncManager asyncManager;

		[Inject]
		public void Construct(UIGlobalCanvas globalCanvas, SceneManager sceneManager, TransitionManager transitionManager, AsyncManager asyncManager)
		{
			this.globalCanvas = globalCanvas;
			this.sceneManager = sceneManager;
			this.transitionManager = transitionManager;
			this.asyncManager = asyncManager;
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

		public void Show(Scenes scene, Transitions transitionsIn, Transitions transitionOut)
		{
			this.scene = scene;
			this.transitionsIn = transitionsIn;
			this.transitionOut = transitionOut;

			ShowAfterTransation();
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

		private void ShowAfterTransation()
		{
			transitionManager
				.TransitionIn(transitionsIn,
				() => {
					Show();
					//start progress

					isProgressing = true;
					asyncManager.StartCoroutine(ProgressTick());

					sceneManager.SwitchScene(scene,
					() =>
					{
						isProgressing = false;
						
						ContinueCanvasGroup.alpha = 0f;
						Progress.enabled = false;
						Continue.enabled = true;
						Continue.gameObject.SetActive(true);
						ContinueCanvasGroup.DOFade(1, 0.2f);

						//end progress
					});


				});
		}

		private IEnumerator ProgressTick()
		{
			while (isProgressing)
			{
				if (sceneManager.ProgressHandle != null)
				{
					Progress.text = $"{sceneManager.ProgressHandle.GetProgressPercent()}%";
				}

				yield return null;
			}

			Progress.text = "100%";
		}

		private void Click()
		{
			Continue.enabled = false;

			ContinueCanvasGroup.DOKill(true);

			Hide(() => transitionManager.TransitionOut(transitionOut));
		}
	}
}