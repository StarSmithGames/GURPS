using EPOOutline;
using Game.Entities;
using Game.Systems.InventorySystem;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Zenject;
using Sirenix.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game
{
	[GlobalConfig("Game/Resources/Assets")]
	[CreateAssetMenu(fileName = "GlobalDatabase", menuName = "GlobalDatabase")]
    public class GlobalDatabaseInstaller : GlobalConfigInstaller<GlobalDatabaseInstaller>
	{
        [HideLabel]
        public GlobalDatabase data;

		public override void InstallBindings()
		{
            Container.BindInstance(data);
		}

#if UNITY_EDITOR
		[Button(ButtonSizes.Medium, DirtyOnClick = true), PropertyOrder(-99)]
        public void UpdateAll()
        {
            UpdateCharacterOverview();
            UpdateModelOverview();
            UpdateContainerOverview();
            UpdateOutlines();

			AssetDatabase.SaveAssets();
        }

        [Button(ButtonSizes.Small, DirtyOnClick = true), PropertyOrder(-98)]
        public void UpdateCharacterOverview()
        {
            data.allCharacters = LoadAssets<CharacterData>();
			data.player = data.allCharacters.Where((x) => x.name == "Player").FirstOrDefault() as CharacterData;
		}

        [Button(ButtonSizes.Small, DirtyOnClick = true), PropertyOrder(-97)]
        public void UpdateModelOverview()
        {
            //allModels = LoadAssets<ModelData>();
        }

        [Button(ButtonSizes.Small, DirtyOnClick = true), PropertyOrder(-96)]
        public void UpdateContainerOverview()
        {
            //allContainers = LoadAssets<ContainerData>();
        }
		[Button(ButtonSizes.Small, DirtyOnClick = true), PropertyOrder(-95)]
		public void UpdateOutlines()
		{
			data.allOutlines = LoadAssets<OutlineData>().ToList();
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
	[System.Serializable]
	public class GlobalDatabase
	{
		public CharacterData player;
		public CharacterData[] allCharacters;
		//[ReadOnly] public ModelData[] allModels;
		[Header("Inventory")]
		public ContainerData[] allContainers;
		[Header("Visual")]
		public List<OutlineData> allOutlines = new List<OutlineData>();
		[Header("Debug")]
		public Mesh HumanoidMesh;
		public Mesh Stand;
		public Material Material;
	}
}