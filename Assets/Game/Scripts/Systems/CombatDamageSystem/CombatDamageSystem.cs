using Game.Entities;
using Game.Entities.Models;
using Game.Managers.StorageManager;
using Game.Systems.AnimatorController;
using Game.Systems.CombatDamageSystem;
using Game.Systems.FloatingTextSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;
using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Systems.CombatDamageSystem
{
	public class CombatDamageSystem
	{
		private FloatingSystem floatingSystem;

		public CombatDamageSystem(FloatingSystem floatingSystem)
		{
			this.floatingSystem = floatingSystem;
		}

		public void DealDamage(Damage damage, IDamageable damageable)
		{
			float dmg = Mathf.Max(damage.DMG, 0);

			var pos = (damageable as MonoBehaviour).transform.TransformPoint(damageable.DamagePosition);

			Debug.LogError(dmg);

			if (dmg < 1)
			{
				floatingSystem.CreateText(pos, "Miss!", type: AnimationType.BasicDamageType);
			}
			else
			{
				floatingSystem.CreateText(pos, damage.damageType.ToString(), type: AnimationType.BasicDamageType);

				if (damage.IsPhysicalDamage)
				{
					floatingSystem.CreateText(pos, ((int)dmg).ToString(), type: AnimationType.AdvanceDamage);

					if (damageable is ISheetable sheetable)
					{
						if (!sheetable.Sheet.Settings.isImmortal)
						{
							sheetable.Sheet.Stats.HitPoints.CurrentValue -= dmg;
						}

						CheckDeath(sheetable);
					}
				}
				else if (damage.IsMagicalDamage)
				{

				}
			}
		}

		private void CheckDeath(ISheetable sheetable)
		{
			if (sheetable.Sheet.Stats.HitPoints.CurrentValue == 0)
			{
				if (sheetable is IDieable dieable)
				{
					if (!sheetable.Sheet.Conditions.IsContains<Death>())
					{
						if (sheetable.Sheet.Conditions.Add(new Death()))
						{
							dieable.Die();
						}
					}
				}
			}
		}
	}

	[System.Serializable]
	public class WeaponDamage
	{
		[InfoBox("MainDamage damageType", InfoMessageType.Error, VisibleIf = "IsDamageTypeError")]
		public Damage mainDamage;
		public bool isHasSideDamages = false;
		[ShowIf("isHasSideDamages")]
		public List<Damage> sideDamages = new List<Damage>();
		[Space]
		[SuffixLabel("%", true)]
		public float criticalDamage = 150;
		[SuffixLabel("%", true)]
		public float criticalChance = 0;

		[ReadOnly][ShowInInspector] public bool IsHasPhysicalDamage => mainDamage.IsPhysicalDamage || sideDamages.Any((x) => x.IsPhysicalDamage);
		[ReadOnly][ShowInInspector] public bool IsHasMagicalDamage => mainDamage.IsMagicalDamage || sideDamages.Any((x) => x.IsMagicalDamage);

		private bool IsDamageTypeError
		{
			get
			{
				return isHasSideDamages ? sideDamages.Where((x) => x.damageType == mainDamage.damageType).ToList().Count > 0 : false;
			}
		}
	}

	public class Damage
	{
		public IDamageable owner;

		public Vector3 direction;

		[MinMaxSlider(0, 99, true)]
		public Vector2 amount;
		public DamageType damageType;

		public float DMG => amount.GetRandomBtw();

		public bool IsPhysicalDamage =>
			damageType == DamageType.Slashing ||
			damageType == DamageType.Crushing ||
			damageType == DamageType.Piercing ||
			damageType == DamageType.Missile;

		public bool IsMagicalDamage =>
			damageType == DamageType.Magic ||
			damageType == DamageType.Fire ||
			damageType == DamageType.Air ||
			damageType == DamageType.Water ||
			damageType == DamageType.Cold ||
			damageType == DamageType.Electricity ||
			damageType == DamageType.Poison;
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

	public enum DamageType : int
	{
		Slashing = 1,
		Crushing = 2,
		Piercing = 3,
		Missile = 4,

		Magic = 10,

		Fire = 15,
		Air = 16,
		Water = 17,
		Cold = 18,
		Electricity = 19,
		Poison = 20,
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