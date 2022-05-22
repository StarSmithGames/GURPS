using Sirenix.OdinInspector;

using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Game.Systems.InventorySystem
{
    public class ItemModel : MonoBehaviour
    {
        public Item Item => item;
        [SerializeField] protected Item item;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ItemModel))]
    public class ItemModelEditor : Editor
	{
		public override void OnInspectorGUI()
		{
            ((ItemModel)target).Item.OnGUI();
		}
	}
#endif
}