using Game.Managers.GameManager;

using Sirenix.OdinInspector;

using System.Collections;
using System.IO;
using System.Linq;

using UnityEngine;

namespace Game.Managers.SceneManager
{
	[CreateAssetMenu(fileName = "SceneData", menuName = "SceneData")]
	public class SceneData : ScriptableObject
	{
		[HideLabel]
		public SceneName sceneName;
		public GameLocation gameLocation;
	}

	[System.Serializable]
	public class SceneName
	{
		public bool useCustom = false;

		[ShowIf("useCustom")]
		[ValueDropdown("SelectScene", DropdownTitle = "Scene Selection")]
		public string name;

		[HideIf("useCustom")]
		public Scenes scenes;

		public string GetScene()
		{
			if (useCustom)
			{
				return name;
			}

			return scenes.ToString();
		}

		private static IEnumerable SelectScene()
		{
			var filesPath = Directory.GetFiles("Assets/Game/Scenes", "*.unity", SearchOption.TopDirectoryOnly);
			var fileNameList = filesPath
			  .Select(Path.GetFileName)
			  .Select(file => file.Split('.')[0])
			  .Distinct()
			  .ToList();

			return fileNameList;
		}
	}

	public enum Scenes
	{
		Menu = 0,
		Map = 1,
		Polygon = 2,
	}
}