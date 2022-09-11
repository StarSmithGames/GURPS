using Game.Entities;
using Game.Systems.InventorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Game.Editor
{
    public static class Search
	{
        public static List<SearchLeaf> EntitiesCreation = new List<SearchLeaf>
        {
            new SearchLeaf() { path = "Entity/PC",          data = typeof(PlayableCharacterData) },
            new SearchLeaf() { path = "Entity/NPC",         data = typeof(NonPlayableCharacterData) },
            new SearchLeaf() { path = "Entity/Model",       data = typeof(ModelData) },
            new SearchLeaf() { path = "Entity/Container",   data = typeof(ContainerData) },
        };
    }

	public class SearchProvider : ScriptableObject, ISearchWindowProvider
	{
        private List<SearchLeaf> items;
        private Action<object> callback;

        public SearchProvider(List<SearchLeaf> list, Action<object> callback)
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
                var splitsA = a.path.Split('/');
                var splitsB = b.path.Split('/');

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
                var entryTitles = item.path.Split('/');
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
                entry.userData = item.data;
                searchList.Add(entry);
            }

            return searchList;
		}

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
            callback?.Invoke(SearchTreeEntry.userData);

            return true;
		}
    }

    public class SearchLeaf
	{
        public string path;
        public object data;
	}
}