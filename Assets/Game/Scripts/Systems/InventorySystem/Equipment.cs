using Game.Systems.CharacterCutomization;

using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
	public class Equipment : IEquipment
	{
		public event UnityAction OnEquipmentChanged;

		public Equip Head { get; private set; }
		public Equip Sholders { get; private set; }
		public Equip Chest { get; private set; }
		public Equip Forearms { get; private set; }
		public Equip Legs { get; private set; }
		public Equip Feet { get; private set; }

		public Equip Weapon00 { get; private set; }
		public Equip Weapon01 { get; private set; }
		public Equip Weapon10 { get; private set; }
		public Equip Weapon11 { get; private set; }

		//Jewelry		= 21,
		//Ring0		= 22,
		//Ring1		= 23,
		//Trinket		= 24,

		public List<Item> Items { get; private set; }
		public List<Equip> Equips { get; private set; }
		public List<Equip> Armors { get; private set; }
		public List<Equip> Weapons { get; private set; }


		private Dictionary<Equip, Type[]> dictionarySlotTypes;

		public Equipment()
		{
			Initialization();
		}

		private void Initialization()
		{
			Head = new Equip();
			Sholders = new Equip();
			Chest = new Equip();
			Forearms = new Equip();
			Legs = new Equip();
			Feet = new Equip();

			Weapon00 = new Equip();
			Weapon01 = new Equip();
			Weapon10 = new Equip();
			Weapon11 = new Equip();

			dictionarySlotTypes = new Dictionary<Equip, Type[]>()
			{
				{ Head,     new Type[]{typeof(HeadItemData)} },
				{ Sholders, new Type[]{typeof(ShouldersItemData) } },
				{ Chest,    new Type[]{typeof(ChestItemData) } },
				{ Forearms, new Type[]{typeof(ForearmItemData) } },
				{ Legs,     new Type[]{typeof(LegsItemData) } },
				{ Feet,     new Type[]{typeof(FeetItemData) } },

				{ Weapon00, new Type[]{typeof(WeaponItemData) } },
				{ Weapon01, new Type[]{typeof(WeaponItemData) } },
				{ Weapon10, new Type[]{typeof(WeaponItemData) } },
				{ Weapon11, new Type[]{typeof(WeaponItemData) } },
			};

			Armors = new List<Equip>()
			{
				Head,
				Sholders,
				Chest,
				Forearms,
				Legs,
				Feet,
			};

			Weapons = new List<Equip>()
			{
				Weapon00,
				Weapon01,
				Weapon10,
				Weapon11,
			};

			Equips = new List<Equip>();
			Equips.AddRange(Armors);
			Equips.AddRange(Weapons);

			Items = new List<Item>();
		}


		public bool Add(Item item)
		{
			if(item.ItemData is WeaponItemData weaponItemData)
			{
				return AddWeapon(item);
			}
			else
			{

			}

			//OnEquipmentChanged?.Invoke();
			return false;
		}

		public bool Remove(Item item)
		{
			Items.Remove(item);
			OnEquipmentChanged?.Invoke();
			return true;
		}

		private void AddArmor(Item item)
		{

		}

		private bool AddWeapon(Item item)
		{
			if(item.ItemData is MeleeItemData meleeItemData)
			{
				if(meleeItemData.melleType == MelleType.TwoHanded)
				{
					Weapon00.SetItem(item);
					Weapon01.SetItem(item);
				}
				else
				{
					Weapon00.SetItem(item);
					Weapon01.SetItem(null);
				}
			}
			else if(item.ItemData is RangedItemData rangedItemData)
			{
				Weapon00.SetItem(item);
				Weapon01.SetItem(item);
			}

			return true;
		}
	}

	[System.Serializable]
	public class Equip
	{
		public UnityAction onEquipChanged;

		public bool IsEmpty => Item == null;
		public Item Item { get; private set; }

		public void SetItem(Item item)
		{
			Item = item;

			onEquipChanged?.Invoke();
		}
	}
}