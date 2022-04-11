using UnityEngine;

using Zenject;

namespace Game.Systems.FloatingTextSystem
{
    public class FloatingText : PoolableObject
    {
        public TMPro.TextMeshPro Text => text;
        [SerializeField] private TMPro.TextMeshPro text;

        public class Factory : PlaceholderFactory<FloatingText> { }
    }
}