using Game.Entities;

using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

namespace Game.Managers.PartyManager
{
	[CreateAssetMenu(fileName = "PartyData", menuName = "Game/Party")]
	public class PartyData : ScriptableObject
	{
		[TableList(AlwaysExpanded = true, ShowIndexLabels = true)]
		public List<PlayableCharacterData> characters = new List<PlayableCharacterData>();
	}
}