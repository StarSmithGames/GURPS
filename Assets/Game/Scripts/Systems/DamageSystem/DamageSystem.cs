using Game.Entities;
using Game.Systems.SheetSystem;

using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.DamageSystem
{
	public class DamageSystem : MonoBehaviour
	{
		//public void Damage()
		//{
		//	IEntity entity = null;

		//	CharacterSheet sheet1 = ((entity as Character).Sheet as CharacterSheet);
		//	ISheet sheet2;
		//absorbed
		//}
	}

	[System.Serializable]
	public struct DamageComposite
	{
		public Damage mainDamage;//only one type dmg!
		public Damage sideDamage;
	}


	[System.Serializable]
	public struct Damage
	{
		public DamageType damageType;

		[ShowIf("@damageType.HasFlag(DamageType.Physical)")]
		public PhysicalDamage physicalDamage;
		[ShowIf("@damageType.HasFlag(DamageType.Magical)")]
		public List<MagicalDamage> magicalDamages;
	}

	[System.Serializable]
	public struct PhysicalDamage
	{
		public float amount;
		public PhysicalDamageType physicalDamage;
	}
	[System.Serializable]
	public struct MagicalDamage
	{
		public float amount;
		public MagicalDamageType magicalDamage;
	}

	[System.Serializable]
	public struct Resistances
	{
		[SuffixLabel("%", overlay: true)]
		[Range(-200f, 200f)]
		public float fire;
		[SuffixLabel("%", overlay: true)]
		[Range(-200f, 200f)]
		public float air;
		[SuffixLabel("%", overlay: true)]
		[Range(-200f, 200f)]
		public float water;
		[SuffixLabel("%", overlay: true)]
		[Range(-200f, 200f)]
		public float earth;
	}



	[System.Flags]
	public enum DamageType
	{
		None,
		Physical = 1,
		Magical = 2,
	}

	public enum PhysicalDamageType
	{
		Slashing,
		Crushing,
		Piercing,
		Missile,
	}

	public enum MagicalDamageType
	{
		Magic,

		Fire,
		Air,
		Water,
		Cold,
		Electricity,
		Poison,
	}

	public enum SpellType
	{
		Regular,
		Melee,
		Ranged,
		Area,
		Resisted,
	}
}