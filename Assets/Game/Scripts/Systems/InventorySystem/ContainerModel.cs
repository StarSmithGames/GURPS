using DG.Tweening;

using EPOOutline;

using Game.Systems.InventorySystem;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Zenject;

using static UnityEngine.EventSystems.EventTrigger;

namespace Game.Systems.InventorySystem
{
	public class ContainerModel : MonoBehaviour, IInteractable, IObservable
	{
		[SerializeField] private Collider collider;
		[SerializeField] private Outlinable outline;

		public ContainerData ContainerData => containerData;
		[SerializeField] private ContainerData containerData;

		[SerializeField] private Settings settings;

		public IInventory Inventory { get; private set; }
		public bool IsOpened => currentChestWindow?.IsShowing ?? false;
		public bool IsSearched => data.isSearched;
		public bool IsInteractable => currentInteractor == null;

		private UIContainerWindow currentChestWindow = null;
		private IEntity currentInteractor = null;

		private Data data;

		private UIManager uiManager;
		private UIContainerWindow.Factory factory;

		[Inject]
		private void Construct(UIManager uiManager, UIContainerWindow.Factory factory)
		{
			this.uiManager = uiManager;
			this.factory = factory;
		}

		private void Awake()
		{
			outline.enabled = false;
		}

		private void Start()
		{
			if (data == null)
			{
				data = new Data();
			}

			Inventory = new Inventory(ContainerData.inventory);
		}


		public void Interact() { }
		public void InteractFrom(IEntity entity)
		{
			if (currentInteractor != null)
			{
				return;
			}

			currentInteractor = entity;
			StartCoroutine(Interaction());
		}


		public void StartObserve()
		{
			if(currentInteractor == null)
			{
				outline.enabled = true;
			}
		}
		public void Observe() { }
		public void EndObserve()
		{
			if (currentInteractor == null)
			{
				outline.enabled = false;
			}
		}

		private void OpenWindow()
		{
			CloseWindow();

			currentChestWindow = factory.Create();
			currentChestWindow.Hide();
			currentChestWindow.transform.parent = uiManager.CurrentVirtualSpace.WindowsRoot;

			(currentChestWindow.transform as RectTransform).anchoredPosition = Vector3.zero;
			currentChestWindow.transform.localScale = Vector3.one;
			currentChestWindow.transform.rotation = Quaternion.Euler(Vector3.zero);
			currentChestWindow.Show();
		}

		private void CloseWindow()
		{
			if(currentChestWindow != null)
			{
				currentChestWindow.Hide();
				currentChestWindow.DespawnIt();
			}
			currentChestWindow = null;
		}


		private IEnumerator Interaction()
		{
			if(currentInteractor == null) yield break;

			outline.enabled = false;

			if (!IsInteractorInRange())
			{
				currentInteractor.Controller.SetDestination(transform.position, settings.maxRange - 0.1f);

				yield return new WaitWhile(() => !currentInteractor.Controller.IsReachedDestination());
			}

			if (IsInteractorInRange())
			{
				OpenWindow();
			}

			while (IsOpened)
			{
				if(!IsInteractorInRange())
				{
					CloseWindow();
				}
				yield return null;
			}

			currentInteractor = null;
		}

		private bool IsInteractorInRange()
		{
			if (currentInteractor == null) return false;
			return Vector3.Distance(transform.position, currentInteractor.Transform.position) <= settings.maxRange;
		}


		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, settings.maxRange);
		}

		[System.Serializable]
		public class Settings
		{
			public float maxRange = 3f;
		}

		public class Data
		{
			public bool isSearched = false;
		}
	}
}