using Game.Managers.GameManager;

using Sirenix.OdinInspector;

using System.Collections;
using System.IO;
using System.Linq;

using UnityEngine;

namespace Game.Managers.SceneManager
{
	[CreateAssetMenu(fileName = "Scene", menuName = "Scene")]
	public class SceneData : ScriptableObject
	{
		[ValueDropdown("SelectScene", DropdownTitle = "Scene Selection")]
		public string sceneName;
		public GameLocation gameLocation;

		private static IEnumerable SelectScene()
		{
			var filesPath = Directory.GetFiles("Assets/Game/Scenes");
			var fileNameList = filesPath
			  .Select(Path.GetFileName)
			  .Select(file => file.Split('.')[0])
			  .Distinct()
			  .ToList();

			return fileNameList;
		}
	}
}