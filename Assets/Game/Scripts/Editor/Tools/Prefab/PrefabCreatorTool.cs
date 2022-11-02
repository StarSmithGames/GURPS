using System.IO;
using UnityEngine;
using UnityEditor;

public static class PrefabCreatorTool
{
    public static bool CreatePrefab(string name, string path)
    {
        bool success = false;

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);


        string generatedName = GenerateRecursiveName(name, path);
        path = Path.Combine(path, $"{generatedName}.prefab");

        GameObject obj = new GameObject(generatedName);

        PrefabUtility.SaveAsPrefabAsset(obj, path, out success);
        Object.DestroyImmediate(obj);

        return success;
    }

    public static bool CreatePrefab(string name, string path, out GameObject gameObject)
    {
        bool success = false;

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string generatedName = GenerateRecursiveName(name, path);
        path = Path.Combine(path, $"{generatedName}.prefab");

        GameObject obj = new GameObject(generatedName);

        gameObject = PrefabUtility.SaveAsPrefabAsset(obj, path, out success);
        Object.DestroyImmediate(obj);

        return success;
    }

    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    private static string GenerateRecursiveName(string name, string path)
    {
        if (File.Exists(Path.Combine(path, $"{name}.prefab")))
        {
            return GenerateRecursiveName(name + "1", path);
        }

        return name;
    }
}