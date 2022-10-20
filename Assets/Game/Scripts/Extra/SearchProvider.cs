using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
public static class Search
{
    public static void OpenSearch<T>(Action<object> callback)
    {
        SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), new SearchProvider(GetSearchByType<T>(), callback));
    }

    public static List<SearchLeaf> GetSearchByType<T>()
    {
        List<SearchLeaf> list = new List<SearchLeaf>();

        var type = typeof(T);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany((s) => s.GetTypes())
            .Where((p) => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract)
            .Select((x) => (GetClassPath(x), x)).ToList();

        types.ForEach((item) =>
        {
            list.Add(new SearchLeaf() { path = item.Item1, data = item.Item2 });
        });

        return list;
    }

    private static string GetClassPath(Type type)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(type);

        string path = type.Name;

        foreach (var attribute in attributes)
        {
            if (attribute is SearchPathAttribute searchPath)
            {
                path = searchPath.path;
            }
        }

        return path;
    }
}
#endif

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
                if (i >= splitsB.Length)
                {
                    return 1;
                }

                var result = splitsA[i].CompareTo(splitsB[i]);
                if (result != 0)
                {
                    if (splitsA.Length != splitsB.Length && (i == splitsA.Length - 1 || i == splitsB.Length - 1))
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
        callback?.Invoke(Activator.CreateInstance((Type)SearchTreeEntry.userData));

        return true;
    }
}

public class SearchLeaf
{
    public string path;
    public object data;
}