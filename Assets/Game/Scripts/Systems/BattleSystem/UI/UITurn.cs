using Game.Entities;
using Game.Systems.SheetSystem;
using Game.UI;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.BattleSystem
{
    public class UITurn : PoolableObject
    {
        public UnityAction<UITurn> onDoubleCick;

        [field: SerializeField] public UIButtonPointer BackgroundButton { get; private set; }
        [field: SerializeField] public PointerHoverComponent PointerHover { get; private set; }
        [field:Space]
        [field: SerializeField] public Image Back { get; private set; }
        [field: SerializeField] public Image Avatar { get; private set; }
        [field: SerializeField] public Image Frame { get; private set; }
        [field: SerializeField] public TMPro.TextMeshProUGUI Name { get; private set; }
        [field: Space]
        [field: SerializeField] public UIBar HPBar { get; private set; }

        [Space]
        [SerializeField] private Vector2 defaultSize = new Vector2(80, 80);
        [SerializeField] private Vector2 selectedSize = new Vector2(100, 100);
        [SerializeField] private Vector2 diselectedSize = new Vector2(60, 60);

        public IEntityModel CurrentEntity { get; private set; }

        private UIManager uiManager;

        [Inject]
        private void Construct()
		{
        }

		private void OnDestroy()
		{
			if(BackgroundButton != null)
			{
                BackgroundButton.onClickChanged -= OnClickChanged;
            }

			if (PointerHover != null)
			{
                PointerHover.onPointerEnter -= OnPointerEnter;
                PointerHover.onPointerExit -= OnPointerExit;
            }
		}

		private void Start()
		{
            BackgroundButton.onClickChanged += OnClickChanged;
            PointerHover.onPointerEnter += OnPointerEnter;
            PointerHover.onPointerExit += OnPointerExit;
        }

        public void SetEntity(IEntityModel entity)
		{
            CurrentEntity = entity;
            //HPBar.SetStat(CurrentEntity?.Sheet.Stats.HitPoints, CurrentEntity?.Sheet.Settings.isImmortal ?? false);

            UpdateUI();
        }

        public void Select()
		{
            //Name.text = CurrentEntity.Sheet.Information.Name;
            Name.enabled = true;
            (transform as RectTransform).sizeDelta = selectedSize;
		}

        public void Diselect()
		{
            Name.text = "";
            Name.enabled = false;
            (transform as RectTransform).sizeDelta = defaultSize;
        }


        private void UpdateUI()
		{
            //Avatar.sprite = (CurrentEntity.Sheet.Information as HumanoidEntityInformation).portrait;
        }

        private void OnClickChanged(int count)
		{
            if(count == 2)
			{
                onDoubleCick?.Invoke(this);
            }
        }

        private void OnPointerEnter(PointerEventData eventData)
        {
            //uiManager.Battle.SetSheet(CurrentEntity.Sheet);
        }
        private void OnPointerExit(PointerEventData eventData)
        {
            uiManager.Battle.SetSheet(null);
        }

        public class Factory : PlaceholderFactory<UITurn> { }
    }
}