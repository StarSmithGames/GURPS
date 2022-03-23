using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.BattleSystem
{
    public class UITurn : PoolableObject
    {
        public UnityAction onClicked;

        [field: SerializeField] public Button BackgroundButton { get; private set; }
        [field: SerializeField] public Image Back { get; private set; }
        [field: SerializeField] public Image Avatar { get; private set; }
        [field: SerializeField] public Image Frame { get; private set; }

        [Space]
        [SerializeField] private Vector2 defaultSize = new Vector2(80, 80);
        [SerializeField] private Vector2 selectedSize = new Vector2(100, 100);

        public IEntity CurrentEntity { get; private set; }

        [Inject]
        private void Construct()
		{
            BackgroundButton.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
            BackgroundButton?.onClick.RemoveAllListeners();
        }

        public void SetEntity(IEntity entity)
		{
            CurrentEntity = entity;

            UpdateUI();
        }

        public void Select()
		{
            (transform as RectTransform).sizeDelta = selectedSize;
		}

        public void Diselect()
		{
            (transform as RectTransform).sizeDelta = defaultSize;
        }



        private void UpdateUI()
		{
            Avatar.sprite = CurrentEntity.EntityData.characterSprite;
        }

        private void OnClick()
		{
            onClicked?.Invoke();
        }

        public class Factory : PlaceholderFactory<UITurn> { }
    }
}