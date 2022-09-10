using Sirenix.Utilities;

using UnityEngine;

public class GlobalConfig<T> : ScriptableObject where T : GlobalConfig<T>, new()
{
	public static T Instance => GlobalConfigUtility<T>.GetInstance(ConfigAttribute.AssetPath);

	private static GlobalConfigAttribute ConfigAttribute
	{
		get
		{
			if (configAttribute == null)
			{
				configAttribute = typeof(T).GetCustomAttribute<GlobalConfigAttribute>();
				if (configAttribute == null)
				{
					configAttribute = new GlobalConfigAttribute(typeof(T).GetNiceName());
				}
			}

			return configAttribute;
		}
	}

	private static GlobalConfigAttribute configAttribute;
}