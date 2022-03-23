using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using Zenject;

namespace Game.Systems.BattleSystem
{
    public class UISkipTurn : MonoBehaviour
    {
		public UnityAction onClick;

        [SerializeField] private Button background;

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
			onClick?.Invoke();
		}
	}
}