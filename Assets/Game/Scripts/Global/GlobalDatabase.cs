using Game.Entities;
using Game.Systems.InventorySystem;

using Sirenix.OdinInspector;
using Sirenix.Utilities;

using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Game
{
    [GlobalConfig("Game/Resources/Assets")]
    public class GlobalDatabase : GlobalConfig<GlobalDatabase>
    {
        [ReadOnly] public CharacterData player;
        [ReadOnly] public CharacterData[] allCharacters;
        [ReadOnly] public ModelData[] allModels;
        [Header("Inventory")]
        [ReadOnly] public ContainerData[] allContainers;
        [Header("Debug")]
        public Mesh HumanoidMesh;
        public Mesh Stand;
        public Material Material;

#if UNITY_EDITOR

        [Button(ButtonSizes.Medium, DirtyOnClick = true), PropertyOrder(-99)]
        public void UpdateAll()
        {
            UpdateCharacterOverview();
            UpdateModelOverview();
            UpdateContainerOverview();

            AssetDatabase.SaveAssets();
        }

        [Button(ButtonSizes.Small, DirtyOnClick = true), PropertyOrder(-98)]
        public void UpdateCharacterOverview()
        {
            allCharacters = LoadAssets<CharacterData>();
            player = allCharacters.Where((x) => x.name == "Player").FirstOrDefault() as CharacterData;
        }

        [Button(ButtonSizes.Small, DirtyOnClick = true), PropertyOrder(-97)]
        public void UpdateModelOverview()
        {
            allModels = LoadAssets<ModelData>();
        }

        [Button(ButtonSizes.Small, DirtyOnClick = true), PropertyOrder(-96)]
        public void UpdateContainerOverview()
        {
            allContainers = LoadAssets<ContainerData>();
        }

        public static T[] LoadAssets<T>(bool orderByName = true) where T : ScriptableObject
        {
            var result = AssetDatabase
                .FindAssets($"t:{typeof(T).Name}")
                .Select((guid) => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)));

            return orderByName ? result.OrderBy((x) => x.name).ToArray() : result.ToArray();
        }
#endif
    }
}