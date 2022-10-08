using Game.Entities;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem.Abilities;
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
	public class CreatorTool : OdinMenuEditorWindow
    {
        private static CreatorTool Window;

        private static string CharactersPath = "Assets/Game/Resources/Assets/Sheet/Characters";
        private static string ModelsPath = "Assets/Game/Resources/Assets/Sheet/Models";
        private static string ContainerPath = "Assets/Game/Resources/Assets/Sheet/Models/Containers";

        private static string MenuAbilitiesPath = "Assets/Game/Resources/Assets/Sheet/Abilities";
        private static string AbilitiesPath = "Assets/Sheet/Abilities";


        private Texture2D trash;

        private IEnumerable<OdinMenuItem> treeMenu;

        [MenuItem("Tools/Creator", priority = 1)]
        public static void OpenWindow()
        {
            //Window
            Window = GetWindow<CreatorTool>(title: "Creator", focus: true);
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

            GlobalDatabase.Instance.UpdateCharacterOverview();
            GlobalDatabase.Instance.UpdateModelOverview();
            GlobalDatabase.Instance.UpdateContainerOverview();
            tree.Add("Characters",          new CharacterDataTable(GlobalDatabase.Instance.allCharacters));
			tree.Add("Models",              new ModelDataTable(GlobalDatabase.Instance.allModels));
			tree.Add("Models/Containers",   new ContainerDataTable(GlobalDatabase.Instance.allContainers));

            tree.AddAllAssetsAtPath("Characters", CharactersPath, typeof(CharacterData), true);
            tree.AddAllAssetsAtPath("Models", ModelsPath, typeof(ModelData), true);
            tree.AddAllAssetsAtPath("Abilities", MenuAbilitiesPath, typeof(BaseAbility), true);

            treeMenu = tree.EnumerateTree(true);
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

                if(GUILayout.Button("Create Entity", EditorStyles.popup))
				{
                    CreateCharacterButton();
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

        private void CreateCharacterButton()
		{
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new SearchProvider(Search.EntitiesCreation, (x) =>
			{
                if(x is Type t)
				{
                    if (t == typeof(PlayableCharacterData))
                    {
                        ShowDialogue<PlayableCharacterData>(CharactersPath);
                    }
                    else if (t == typeof(NonPlayableCharacterData))
                    {
                        ShowDialogue<NonPlayableCharacterData>(CharactersPath);
                    }
                    else if (t == typeof(ModelData))
                    {
                        ShowDialogue<ModelData>(ModelsPath);
					}
                    else if(t == typeof(ContainerData))
					{
                        ShowDialogue<ContainerData>(ContainerPath);
                    }
                    else
					{
                        Debug.LogError($"Can't create type: {t}");
					}
				}
            }));
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
            var abilities = data.sheet.abilities.abilities;
            //remove null's
            
            for (int i = abilities.Count - 1; i >= 0; i--)
            {
                if (abilities[i] == null)
                {
                    abilities.RemoveAt(i);
                }
            }

            //cheack all base abilities
            var baseAbilities = GetAbilities().Where((x) => x.name.StartsWith("Base")).OrderBy((x) => x.name);
            data.sheet.abilities.abilities = abilities.Union(baseAbilities).OrderBy((x) => x.name).ToList();
        }

        private void ShowDialogue<T>(string path) where T : EntityData
        {
            Sirenix.OdinInspector.Demos.RPGEditor.ScriptableObjectCreator.ShowDialog<T>(path, obj =>
            {
                base.TrySelectMenuItemWithObject(obj); // Selects the newly created item in the editor

                RefreshData(obj);
            });
        }

        public static BaseAbility[] GetAbilities()
        {
            return Resources.LoadAll<BaseAbility>(AbilitiesPath);
        }
    }
}