using Game.UI;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.SheetSystem
{
	public sealed class UIAction : PoolableObject
	{
		public UnityAction<UIAction> onClicked;

		[field: SerializeField] public PointerHandlerComponent Pointer { get; private set; }
		[field: Space]
		[field: SerializeField] public Image Background { get; private set; }
		[field: SerializeField] public Image Icon { get; private set; }
		[field: SerializeField] public Image Frame { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Count { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Num { get; private set; }

		private void Start()
		{
			Pointer.onPointerClick += OnClicked;
		}

		private void OnDestroy()
		{
			if(Pointer != null)
			{
				Pointer.onPointerClick -= OnClicked;
			}
		}

		public void SetAction(Actions.IAction action)
		{
			Icon.enabled = action != null;
			Icon.sprite = action?.Information.portrait;

			Count.enabled = false;
			Count.text = "0";

			Num.enabled = false;
		}

		private void OnClicked(PointerEventData data)
		{
			onClicked?.Invoke(this);
		}

		public class Factory : PlaceholderFactory<UIAction> { }
	}
}