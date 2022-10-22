using Game.Systems.SheetSystem;
using Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Game.Managers.PartyManager
{
	public class UIEffect : PoolableObject
	{
		[field: SerializeField] public PointerHandlerComponent PointerHandler { get; private set; }
		[field: SerializeField] public Image Filler { get; private set; }
		[field: SerializeField] public Image Icon { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }

		private bool isInitialized = false;

		private IEffect effect;


		private void OnDestroy()
		{
			if (isInitialized)
			{
				if (PointerHandler != null)
				{
					PointerHandler.onPointerEnter -= OnPointerEnter;
					PointerHandler.onPointerExit -= OnPointerExit;
					PointerHandler.onPointerClick -= OnPointerClick;
				}
			}
		}

		public void SetEffect(IEffect effect)
		{

			UpdateUI();
		}

		private void UpdateUI()
		{

		}

		public override void OnSpawned(IMemoryPool pool)
		{
			if (!isInitialized)
			{
				PointerHandler.onPointerEnter += OnPointerEnter;
				PointerHandler.onPointerExit += OnPointerExit;
				PointerHandler.onPointerClick += OnPointerClick;
			}

			isInitialized = true;

			base.OnSpawned(pool);
		}

		private void OnPointerEnter(PointerEventData data)
		{

		}

		private void OnPointerExit(PointerEventData data)
		{

		}

		private void OnPointerClick(PointerEventData data)
		{

		}

		public class Factory : PlaceholderFactory<UIEffect> { }
	}
}