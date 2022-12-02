using Game.Systems.SheetSystem;
using Game.UI;
using Game.UI.CanvasSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Game.HUD
{
    public class UITurn : PoolableObject
    {
        public UnityAction<UITurn> onDoubleCick;

        [field: SerializeField] public PointerHandlerComponent PointerHandler { get; private set; }
        [field:Space]
        [field: SerializeField] public Image Back { get; private set; }
        [field: SerializeField] public Image Avatar { get; private set; }
        [field: SerializeField] public Image Frame { get; private set; }
        [field: SerializeField] public TMPro.TextMeshProUGUI Name { get; private set; }
        [field: Space]
        [field: SerializeField] public UIStatBar HPBar { get; private set; }

        [Space]
        [SerializeField] private Vector2 defaultSize = new Vector2(80, 80);
        [SerializeField] private Vector2 selectedSize = new Vector2(100, 100);
        [SerializeField] private Vector2 diselectedSize = new Vector2(60, 60);

        private WindowEntityInformation EntityInformation
        {
            get
            {
                if (entityInformation == null)
                {
                    entityInformation = subCanvas.WindowsRegistrator.GetAs<WindowEntityInformation>();
                }

                return entityInformation;
            }
        }
        private WindowEntityInformation entityInformation;

        public ISheetable CurrentSheetable { get; private set; }

        private UISubCanvas subCanvas;

        [Inject]
        private void Construct(UISubCanvas subCanvas)
		{
            this.subCanvas = subCanvas;
        }

		private void Start()
		{
            PointerHandler.onPointerClick += OnClickChanged;
            PointerHandler.onPointerEnter += OnPointerEnter;
            PointerHandler.onPointerExit += OnPointerExit;
        }

        private void OnDestroy()
        {
            if (PointerHandler != null)
            {
                PointerHandler.onPointerClick -= OnClickChanged;
                PointerHandler.onPointerEnter -= OnPointerEnter;
                PointerHandler.onPointerExit -= OnPointerExit;
            }
        }

        public void SetEntity(ISheetable sheetable)
		{
            CurrentSheetable = sheetable;
			HPBar.SetStat(CurrentSheetable?.Sheet.Stats.HitPoints, CurrentSheetable?.Sheet.Settings.isImmortal ?? false);

			UpdateUI();
        }

        public void Select()
		{
			Name.text = CurrentSheetable.Sheet.Information.Name;
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
			Avatar.sprite = CurrentSheetable.Sheet.Information.portrait;
		}

        private void OnClickChanged(PointerEventData eventData)
		{
            if(eventData.clickCount == 2)
			{
                onDoubleCick?.Invoke(this);
            }
        }

        private void OnPointerEnter(PointerEventData eventData)
        {
            if (CurrentSheetable != null)
            {
                EntityInformation.SetSheet(CurrentSheetable.Sheet);

                if (!EntityInformation.IsShowing)
                {
                    EntityInformation.Enable(true);
                }
            }
        }

        private void OnPointerExit(PointerEventData eventData)
        {
            if (EntityInformation.IsShowing)
            {
                EntityInformation.Enable(true);
            }
        }

        public class Factory : PlaceholderFactory<UITurn> { }
    }
}