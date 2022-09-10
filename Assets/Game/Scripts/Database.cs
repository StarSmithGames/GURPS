using Game.Entities;
using Game.Systems.InventorySystem;

using Sirenix.OdinInspector;
using Sirenix.Utilities;

using System.Linq;

using UnityEditor;

using UnityEngine;

[GlobalConfig("Game/Resources/Assets")]
public class Database : GlobalConfig<Database>
{
    [ReadOnly] public CharacterData[] allCharacters;
    [ReadOnly] public ModelData[] allModels;
    [Header("Inventory")]
    [ReadOnly] public ContainerData[] allContainers;

#if UNITY_EDITOR

    [Button(ButtonSizes.Medium), PropertyOrder(-99)]
    public void UpdateAll()
	{
        UpdateCharacterOverview();
        UpdateModelOverview();
        UpdateContainerOverview();
    }

    [Button(ButtonSizes.Small), PropertyOrder(-98)]
    public void UpdateCharacterOverview()
    {
        allCharacters = LoadAssets<CharacterData>();
    }

    [Button(ButtonSizes.Small), PropertyOrder(-97)]
    public void UpdateModelOverview()
    {
        allModels = LoadAssets<ModelData>();
    }

    [Button(ButtonSizes.Small), PropertyOrder(-96)]
    public void UpdateContainerOverview()
    {
        allContainers = LoadAssets<ContainerData>();
    }

    public static T[] LoadAssets<T>(bool orderByName = true) where T : ScriptableObject
    {
        var result = AssetDatabase
            .FindAssets($"t:{typeof(T).Name}")
            .Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)));

        return orderByName ? result.OrderBy((x) => x.name).ToArray() : result.ToArray();
    }
#endif
}