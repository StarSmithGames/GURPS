using Game.UI.Windows;

using UnityEngine;

namespace Game.Systems.BattleSystem {
    public class WindowInformer : WindowBase
    {
        [field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }

        public void SetText(string text)
        {
            Text.text = text;
        }
    }
}