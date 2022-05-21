using UnityEngine;

using Zenject;

namespace Game.Systems.DialogueSystem
{
    public class UISubtitle : PoolableObject
    {
        [field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }
        
        public class Factory : PlaceholderFactory<UISubtitle> { }
    }
}