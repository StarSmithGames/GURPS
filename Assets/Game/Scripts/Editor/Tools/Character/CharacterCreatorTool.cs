using Game.Entities;
using Game.Systems.SheetSystem;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;

namespace Game.Editor
{
    public class CharacterCreatorTool : OdinMenuEditorWindow
    {
        private static CharacterCreatorTool Window;

        private static string CharactersPath = "Assets/Game/Resources/Assets/Sheet/Characters";

        private static string[] EntitiesCreation = new string[]
        {
            "Entity/Character",
            "Entity/Companion",
            "Entity/NPC",
            "Model"
        };

        private Texture2D trash;

        private IEnumerable<OdinMenuItem> treeMenu;

        [MenuItem("Tools/Character Creator", priority = 1)]
        public static void OpenWindow()
        {
            //Window
            Window = GetWindow<CharacterCreatorTool>(title: "Character Creator", focus: true);
            //window.maxSize = new Vector2(250, 120);
            Window.minSize = new Vector2(800, 500);
            Window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
            Window.ShowUtility();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            trash = (EditorGUIUtility.Load("Trash.png") as Texture2D);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            treeMenu = tree.AddAllAssetsAtPath("Edit Data", CharactersPath, typeof(EntityData), true);
            treeMenu.SortMenuItemsByName();

            foreach (var item in treeMenu)
            {
				item.OnRightClick = (menuItem) => EditorGUIUtility.PingObject(menuItem.Value as ScriptableObject);
            }

            tree.MarkDirty();

            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;

            // Draws a toolbar with the name of the currently selected menu item.
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null)
                {
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Refresh")))
				{
                    RefreshButton();
                }

				//if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Character")))
				//{
				//	CreateCharacter();
				//}

                if(GUILayout.Button("Create Entity", EditorStyles.popup))
				{
                    CreateCharacter();
                }


				if (selected?.Value is EntityData data)
                {
                    if (SirenixEditorGUI.ToolbarButton(new GUIContent("Delete", trash)))
                    {
                        var path = AssetDatabase.GetAssetPath(data);
                        AssetDatabase.DeleteAsset(path);
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        private void CreateCharacter()
		{
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new SearchProvider(EntitiesCreation, (x) => Debug.LogError(x)));

            return;

			Sirenix.OdinInspector.Demos.RPGEditor.ScriptableObjectCreator.ShowDialog<CharacterData>(CharactersPath, obj =>
            {
                base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor

                RefreshData(obj);
            });
        }

        private void RefreshButton()
		{
            treeMenu.ForEach((x) =>
            {
                if (x.Value is EntityData data)
                {
                    RefreshData(data);
                }
            });

            AssetDatabase.SaveAssets();
        }

        private void RefreshData(EntityData data)
		{
            var baseAbilityies = data.sheet.abilities.baseAbilities;
            //if(data.sheet.abilities.baseAbilities.Count == 0)
            {
                baseAbilityies.Clear();
                baseAbilityies.AddRange(GetAbilities().Where((x) => x.name.StartsWith("Base")).OrderBy((x) => x.name));
            }
        }

        public static AbilityData[] GetAbilities()
        {
            return AssetDatabase.FindAssets("t: AbilityData")
                .Select((x) => AssetDatabase.GUIDToAssetPath(x))
                .Select((x) => AssetDatabase.LoadAssetAtPath<AbilityData>(x)).ToArray();
        }
    }

	public class SearchProvider : ScriptableObject, ISearchWindowProvider
	{
        private string[] items;
        private Action<string> callback;

        public SearchProvider(string[] list, Action<string> callback)
		{
            this.items = list;
            this.callback = callback;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
		{
            var searchList = new List<SearchTreeEntry>();

            searchList.Add(new SearchTreeGroupEntry(new GUIContent("List"), 0));

            var list = items.ToList();

            list.Sort((a, b) =>
            {
                var splitsA = a.Split('/');
                var splitsB = b.Split('/');

				for (int i = 0; i < splitsA.Length; i++)
				{
                    if(i >= splitsB.Length)
					{
                        return 1;
					}

                    var result = splitsA[i].CompareTo(splitsB[i]);
                    if(result != 0)
					{
                        if(splitsA.Length != splitsB.Length && (i == splitsA.Length - 1 || i == splitsB.Length - 1))
						{
                            return splitsA.Length < splitsB.Length ? 1 : -1;
                        }

                        return result;
					}
				}

                return 0;
            });

            var groups = new List<string>();

			foreach (var item in items)
			{
                var entryTitles = item.Split('/');
                var groupName = "";

				for (int i = 0; i < entryTitles.Length - 1; i++)
				{
                    groupName += entryTitles[i];

					if (!groups.Contains(groupName))
					{
                        searchList.Add(new SearchTreeGroupEntry(new GUIContent(entryTitles[i]), i + 1));
                        groups.Add(groupName);
                    }

                    groupName += "/";
                }

                var entry = new SearchTreeEntry(new GUIContent(entryTitles.Last()));
                entry.level = entryTitles.Length;
                entry.userData = entryTitles.Last();
                searchList.Add(entry);
            }

            return searchList;
		}

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
            callback?.Invoke((string)SearchTreeEntry.userData);

            return true;
		}
	}
}