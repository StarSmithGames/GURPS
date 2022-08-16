using Game.Managers.StorageManager;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Zenject;

namespace Game.UI
{
	public class UICommit : PoolableObject
	{
		public UnityAction<UICommit> onClick;

		public Commit Commit { get; private set; }

		[field: SerializeField] public Image Background { get; private set; }
		[field: SerializeField] public Button Button { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Title { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI DateTime { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Date { get; private set; }

		[field: SerializeField] public GameObject TypeQuickSave { get; private set; }
		[field: SerializeField] public GameObject TypeAutoSave { get; private set; }
		[field: SerializeField] public GameObject TypeManualSave { get; private set; }

		[Space]
		[SerializeField] private Color select;

		private Color baseColor;

		private void Start()
		{
			baseColor = Background.color;
			Button.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
			onClick = null;
			Button?.onClick.RemoveAllListeners();
		}

		public void SetCommit(Commit commit)
		{
			Commit = commit;

			Title.text = Commit?.title;

			DateTime.text = Commit?.date.ToString("HH:mm:ss");
			Date.text = Commit?.date.ToString("MM/dd/yyyy");

			if (commit != null)
			{
				TypeQuickSave.gameObject.SetActive(commit.type == CommitType.QuickSave);
				TypeAutoSave.gameObject.SetActive(commit.type == CommitType.AutoSave);
				TypeManualSave.gameObject.SetActive(commit.type == CommitType.ManualSave);
			}
		}

		public void Select()
		{
			Background.color = select;
		}

		public void Diseclect()
		{
			Background.color = baseColor;
		}

		private void OnClick()
		{
			onClick?.Invoke(this);
		}

		public class Factory : PlaceholderFactory<UICommit> { }
	}
}