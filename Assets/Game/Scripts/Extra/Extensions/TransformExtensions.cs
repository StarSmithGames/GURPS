using UnityEngine;

public static class TransformExtensions
{
    public static Transform DestroyChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        return transform;
    }

    public static float GetRandomBtw(this Vector2 vector2)
	{
        return Random.Range(vector2.x, vector2.y);
	}
}