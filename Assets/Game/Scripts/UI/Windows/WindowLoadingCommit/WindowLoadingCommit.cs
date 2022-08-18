using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;
using UnityEngine.EventSystems;
using Game.Managers.StorageManager;
using Game.Managers.InputManager;
using FlowCanvas.Nodes;

namespace Game.UI.Windows
{
	public class WindowLoadingCommit : WindowBase
	{
		public bool IsLoading { get; set; }

		[field: SerializeField] public Transform Content { get; private set; }
		[field: SerializeField] public Button Back { get; private set; }
		[field: SerializeField] public Button NewCommit { get; private set; }
		[field: SerializeField] public PointerInteractComponent Background { get; private set; }
		[field: Space]
		[field: SerializeField] public GameObject Info { get; private set; }
		[field: SerializeField] public Button Load { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI LoadText { get; private set; }

		[field: SerializeField] public TMPro.TextMeshProUGUI CommitName { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI CommitIndex { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI CommitType { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI CommitDate { get; private set; }

		private List<UICommit> commits = new List<UICommit>();
		private UICommit lastCommit = null;

		private UIGlobalCanvas globalCanvas;
		private UISubCanvas subCanvas;
		private UICommit.Factory commitFactory;
		private ISaveLoad saveLoad;
		private SaveLoadOverseer saveLoadOverseer;
		private InputManager inputManager;

		[Inject]
		private void Construct(UICommit.Factory commitFactory, UIGlobalCanvas globalCanvas, UISubCanvas subCanvas, ISaveLoad saveLoad, SaveLoadOverseer saveLoadOverseer, InputManager inputManager)
		{
			this.globalCanvas = globalCanvas;
			this.subCanvas = subCanvas;
			this.commitFactory = commitFactory;
			this.saveLoad = saveLoad;
			this.saveLoadOverseer = saveLoadOverseer;
			this.inputManager = inputManager;
		}

		private void Start()
		{
			IsLoading = true;

			Content.DestroyChildrenByType<UICommit>();

			Enable(false);
			Info.SetActive(false);

			NewCommit.onClick.AddListener(OnNewCommit);
			Back.onClick.AddListener(OnBack);
			Load.onClick.AddListener(OnSaveLoad);
			Background.onPointerDown += OnBackgroundDown;
		
			subCanvas.WindowsRegistrator.Registrate(this);
		}

		private void OnDestroy()
		{
			subCanvas.WindowsRegistrator.UnRegistrate(this);

			Dispose();
			NewCommit?.onClick.RemoveAllListeners();
			Back?.onClick.RemoveAllListeners();
			Load?.onClick.RemoveAllListeners();
			Background.onPointerDown -= OnBackgroundDown;
		}

		private void Update()
		{
			if (IsShowing)
			{
				if (inputManager.GetKeyDown(KeyAction.InGameMenu))
				{
					OnBack();
				}
			}
		}

		public override void Show(UnityAction callback = null)
		{
			NewCommit.gameObject.SetActive(!IsLoading);
			LoadText.text = IsLoading? "LOAD" : "ReSAVE";

			Fill();

			base.Show(callback);
		}

		public override void Hide(UnityAction callback = null)
		{
			base.Hide(() =>
			{
				Dispose();
				callback?.Invoke();
			});
		}

		private void CommitSelected()
		{
			Info.SetActive(true);
			CommitIndex.text = commits.IndexOf(lastCommit).ToString();
			CommitType.text = lastCommit.Commit.type.ToString();
			CommitDate.text = lastCommit.Commit.date.ToString("MM/dd/yyyy HH:mm:ss");
		}

		private void Diselect()
		{
			lastCommit?.Diseclect();
			lastCommit = null;
			Info.SetActive(false);
		}

		private void Fill()
		{
			var profileCommits = saveLoad.GetStorage().CurrentProfile.GetData().commits;

			for (int i = profileCommits.Count - 1; i >= 0; i--)
			{
				var commit = commitFactory.Create();

				commit.SetCommit(profileCommits[i]);

				commit.onClick += OnCommitClicked;

				commit.transform.SetParent(Content);

				commits.Add(commit);
			}
		}

		private void Dispose()
		{
			foreach (var commit in commits)
			{
				commit.onClick -= OnCommitClicked;
				commit.SetCommit(null);
				commit.Diseclect();
				commit.DespawnIt();
			}
			commits.Clear();

			lastCommit = null;
			Info.SetActive(false);
		}

		private void RefreshCommits()
		{
			Diselect();
			Dispose();
			Fill();
		}


		private void OnCommitClicked(UICommit commit)
		{
			lastCommit?.Diseclect();
			lastCommit = commit;
			lastCommit.Select();
			CommitSelected();
		}

		private void OnNewCommit()
		{
			Diselect();

			var window = globalCanvas.WindowsManager.GetAs<WindowInputGenericDialogue>();
			window.onOk += () =>
			{
				saveLoad.Save(Managers.StorageManager.CommitType.ManualSave, window.InputField.text);
				RefreshCommits();
			};
			window.Show();
		}

		private void OnBack()
		{
			Hide();
		}

		private void OnSaveLoad()
		{
			if (lastCommit == null) return;
			if (lastCommit.Commit == null) return;

			if (IsLoading)
			{
				CanvasGroup.interactable = false;
				Background.IsInteractable = false;

				saveLoadOverseer.LoadGame(lastCommit.Commit);
			}
			else//rewrite
			{
				var window = globalCanvas.WindowsManager.GetAs<WindowInputGenericDialogue>();
				window.onOk += () =>
				{
					var profile = saveLoad.GetStorage().CurrentProfile.GetData();

					profile.commits.Remove(lastCommit.Commit);

					saveLoad.Save(Managers.StorageManager.CommitType.ManualSave, window.InputField.text);
					RefreshCommits();
				};
				window.InputField.text = $"{lastCommit.Commit.title}";
				window.Show();
			}
		}

		private void OnBackgroundDown(PointerEventData data)
		{
			Diselect();
		}
	}
}