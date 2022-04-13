using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Systems.InventorySystem
{
    public class ItemWeaponModel : ItemModel
    {
        [ButtonGroup("Transform")]
        private void LeftSheath()
		{
            var data = (item.ItemData as WeaponItemData);
            data.sheathForLeftHandTransfrom.position = transform.localPosition;
            data.sheathForLeftHandTransfrom.rotation = transform.localRotation;

#if UNITY_EDITOR
            EditorUtility.SetDirty(data);
#endif
        }


        [ButtonGroup("Transform")]
        private void RightSheath()
        {
            var data = (item.ItemData as WeaponItemData);
            data.sheathForRightHandTransfrom.position = transform.localPosition;
            data.sheathForRightHandTransfrom.rotation = transform.localRotation;

#if UNITY_EDITOR
            EditorUtility.SetDirty(data);
#endif
        }
        [ButtonGroup("Transform")]
        private void RightHand()
        {
            var data = (item.ItemData as WeaponItemData);
            data.rightHandTransfrom.position = transform.localPosition;
            data.rightHandTransfrom.rotation = transform.localRotation;

#if UNITY_EDITOR
            EditorUtility.SetDirty(data);
#endif
        }
        [ButtonGroup("Transform")]
        private void LeftHand()
        {
            var data = (item.ItemData as WeaponItemData);
            data.leftHandTransfrom.position = transform.localPosition;
            data.leftHandTransfrom.rotation = transform.localRotation;

#if UNITY_EDITOR
            EditorUtility.SetDirty(data);
#endif
        }
    }
}