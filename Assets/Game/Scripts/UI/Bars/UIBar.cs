using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIBar : MonoBehaviour
    {
        public float FillAmount
        {
            get => Bar.fillAmount;
            set => Bar.fillAmount = value;
        }

        public bool isHasText = false;
        [field: ShowIf("isHasText")]
        public bool isTextShowOnHover = false;

        [field: SerializeField] public PointerHandlerComponent PointerHandler { get; private set; }
        [field: SerializeField] public Image Bar { get; private set; }
        [field: ShowIf("isHasText")]
        [field: SerializeField] public TMPro.TextMeshProUGUI BarText { get; private set; }

		private void Start()
		{
            if(isHasText && isTextShowOnHover)
			{
                PointerHandler.onPointerEnter += OnPointerEnter;
                PointerHandler.onPointerExit += onPointerExit;

                BarText.enabled = false;
            }
        }

		private void OnDestroy()
		{
            if (isHasText && isTextShowOnHover)
            {
                PointerHandler.onPointerEnter -= OnPointerEnter;
                PointerHandler.onPointerExit -= onPointerExit;
            }
        }

		protected virtual void OnPointerEnter(PointerEventData data)
		{
            BarText.enabled = true;
        }

        protected virtual void onPointerExit(PointerEventData data)
        {
            BarText.enabled = false;
        }
    }
}