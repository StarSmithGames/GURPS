using System.Collections.Generic;
using UnityEngine;

public static class DontDestroyOnLoadManager
{
	private static List<GameObject> objects = new List<GameObject>();

	public static void DontDestroyOnLoad(this GameObject go)
	{
		if (!objects.Contains(go))
		{
			Object.DontDestroyOnLoad(go);
			objects.Add(go);
		}
	}

	public static void DestroyAll()
	{
		foreach (var go in objects)
		{
			if (go != null)
			{
				Object.Destroy(go);
			}
		}

		objects.Clear();
	}
}