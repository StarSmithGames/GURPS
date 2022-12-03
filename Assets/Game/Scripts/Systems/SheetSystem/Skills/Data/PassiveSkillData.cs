using System.Collections.Generic;

using UnityEngine;

namespace Game.Systems.SheetSystem.Skills
{
	[CreateAssetMenu(fileName = "PassiveSkill", menuName = "Game/Sheet/Skills/Passive")]
	public class PassiveSkillData : SkillData
	{
		[SerializeReference] public List<EnchantmentType> enchantments = new List<EnchantmentType>();
	}
}