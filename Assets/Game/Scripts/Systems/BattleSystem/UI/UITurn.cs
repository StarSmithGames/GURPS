using Game.Entities;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.BattleSystem
{
    public class UITurn : PoolableObject
    {
        public UnityAction<UITurn> onDoubleCick;

        [field: SerializeField] public UIButtonPointer BackgroundButton { get; private set; }
        [field: SerializeField] public Image Back { get; private set; }
        [field: SerializeField] public Image Avatar { get; private set; }
        [field: SerializeField] public Image Frame { get; private set; }
        [field: SerializeField] public TMPro.TextMeshProUGUI Name { get; private set; }

        [Space]
        [SerializeField] private Vector2 defaultSize = new Vector2(80, 80);
        [SerializeField] private Vector2 selectedSize = new Vector2(100, 100);

        [Inject]
        private void Construct()
		{
            BackgroundButton.onClickChanged += OnClickChanged;

        }

		private void OnDestroy()
		{
			if(BackgroundButton != null)
			{
                BackgroundButton.onClickChanged -= OnClickChanged;
            }
		}

		public IEntity CurrentEntity { get; private set; }

        public void SetEntity(IEntity entity)
		{
            CurrentEntity = entity;

            UpdateUI();
        }

        public void Select()
		{
            Name.text = CurrentEntity.EntityData.CharacterName;
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
            Avatar.sprite = CurrentEntity.EntityData.characterSprite;
        }

        private void OnClickChanged(int count)
		{
            if(count == 2)
			{
                onDoubleCick?.Invoke(this);
            }
        }

        public class Factory : PlaceholderFactory<UITurn> { }
    }
}