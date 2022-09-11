using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Zenject;
using Game.UI.CanvasSystem;

namespace Game.UI.Windows
{
	public class WindowInputGenericDialogue : WindowBase
	{
		public UnityAction onOk;
		public UnityAction onReject;

		[field: SerializeField] public TMPro.TMP_InputField InputField { get; private set; }
		[field: SerializeField] public Button Ok { get; private set; }
		[field: SerializeField] public Button Reject { get; private set; }

		private UIGlobalCanvas globalCanvas;

		[Inject]
		private void Construct(UIGlobalCanvas globalCanvas)
		{
			globalCanvas.WindowsRegistrator.Registrate(this);
		}

		private void Start()
		{
			CanvasGroup.Enable(false);

			Ok.onClick.AddListener(OnOk);
			Reject.onClick.AddListener(OnReject);
		}

		private void OnDestroy()
		{
			globalCanvas?.WindowsRegistrator.UnRegistrate(this);

			Ok?.onClick.RemoveAllListeners();
			Reject?.onClick.RemoveAllListeners();
		}

		public override void Show(UnityAction callback = null)
		{
			InputField.text = "";

			transform.SetAsLastSibling();

			base.Show(callback);

			InputField.ActivateInputField();
			InputField.Select();
		}

		public override void Hide(UnityAction callback = null)
		{
			onOk = null;
			onReject = null;

			base.Hide(callback);
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