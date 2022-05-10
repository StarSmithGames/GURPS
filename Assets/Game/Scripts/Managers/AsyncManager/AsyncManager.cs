using UnityEngine;

public class AsyncManager : MonoBehaviour
{
	public static AsyncManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<AsyncManager>();
			}

			return instance;
		}
	}
	private static AsyncManager instance;
}