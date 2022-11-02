using UnityEditor;
using UnityEngine;

namespace Game.Systems.SheetSystem.Editor
{
    public static class PrefabSheetCreator
    {
        [MenuItem("Assets/Create/Game/Sheet/Skill")]
        public static void CreateSkillPrefab()
        {
            if(!PrefabCreatorTool.CreatePrefab("Skill", PrefabCreatorTool.GetSelectedPathOrFallback()))
			{
                Debug.LogError("Can't create prefab");
			}
        }
    }
}