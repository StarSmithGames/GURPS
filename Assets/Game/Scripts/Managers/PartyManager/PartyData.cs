using Game.Entities;

using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

namespace Game.Managers.PartyManager
{
	[CreateAssetMenu(fileName = "PartyData", menuName = "Game/Party")]
	public class PartyData : ScriptableObject
	{
		[TableList()]
		public List<PlayableCharacterData> characters = new List<PlayableCharacterData>();

		[Button]
		private void TryGet()
		{
			Debug.LogError(Database.Instance != null);
		}
	}
}