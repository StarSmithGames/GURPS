using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.BattleSystem
{
    public class UITurn : PoolableObject
    {
        public UnityAction onClicked;

        public Button background;
        [Space]
        public Image back;
        public Image avatar;
        public Image frame;

        [Inject]
        private void Construct()
		{
            background.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
            background?.onClick.RemoveAllListeners();
        }

        private void OnClick()
		{
            onClicked?.Invoke();
        }

        public class Factory : PlaceholderFactory<UITurn> { }
    }
}