using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;

namespace Game.UI.Windows
{
	public class WindowInputGenericDialogue : MonoBehaviour, IWindow
	{
		public UnityAction onOk;
		public UnityAction onReject;

		public bool IsShowing { get; private set; }

		[field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
		[field: SerializeField] public TMPro.TMP_InputField InputField { get; private set; }
		[field: SerializeField] public Button Ok { get; private set; }
		[field: SerializeField] public Button Reject { get; private set; }

		private UIGlobalCanvas globalCanvas;

		[Inject]
		private void Construct(UIGlobalCanvas globalCanvas)
		{
			globalCanvas.WindowsManager.Register(this);
		}

		private void Start()
		{
			CanvasGroup.Enable(false);

			Ok.onClick.AddListener(OnOk);
			Reject.onClick.AddListener(OnReject);
		}

		private void OnDestroy()
		{
			globalCanvas?.WindowsManager.UnRegister(this);

			Ok?.onClick.RemoveAllListeners();
			Reject?.onClick.RemoveAllListeners();
		}

		public void Enable(bool trigger)
		{
			CanvasGroup.Enable(trigger);
			IsShowing = trigger;
		}

		public void Show(UnityAction callback = null)
		{
			InputField.text = "";


			transform.SetAsLastSibling();

			CanvasGroup.Enable(true);
			
			InputField.ActivateInputField();
			InputField.Select();

			IsShowing = true;
		}

		public void Hide(UnityAction callback = null)
		{
			onOk = null;
			onReject = null;

			CanvasGroup.Enable(false);
			IsShowing = false;
		}

		private void OnOk()
		{
			onOk?.Invoke();
			Hide();
		}

		private void OnReject()
		{
			onReject?.Invoke();
			Hide();
		}
	}
}