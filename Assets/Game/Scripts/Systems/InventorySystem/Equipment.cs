using Game.Systems.CharacterCutomization;

using System;
using System.Collections.Generic;

using UnityEngine;
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

		public Equip Cloak { get; private set; }
		public Equip Jewelry { get; private set; }
		public Equip Ring0 { get; private set; }
		public Equip Ring1 { get; private set; }
		public Equip Trinket { get; private set; }

		public List<Equip> Armors { get; private set; }

		private Dictionary<Equip, Type[]> dictionarySlotTypes;
		private EquipWeaponConnection Weapon0;
		private EquipWeaponConnection Weapon1;

		private EquipmentSettings settings;
		private IInventory inventory;

		public Equipment(EquipmentSettings settings, IInventory inventory)
		{
			this.settings = settings;
			this.inventory = inventory;
			Initialization();
		}

		private void Initialization()
		{
			Head		= new Equip();
			Sholders	= new Equip();
			Chest		= new Equip();
			Forearms	= new Equip();
			Legs		= new Equip();
			Feet		= new Equip();

			Weapon00	= new Equip();
			Weapon01	= new Equip();
			Weapon10	= new Equip();
			Weapon11	= new Equip();

			Cloak		= new Equip();
			Jewelry		= new Equip();
			Ring0		= new Equip();
			Ring1		= new Equip();
			Trinket		= new Equip();

			dictionarySlotTypes = new Dictionary<Equip, Type[]>()
			{
				{ Head,     new Type[]{typeof(HeadItemData)} },
				{ Sholders, new Type[]{typeof(ShouldersItemData) } },
				{ Chest,    new Type[]{typeof(ChestItemData) } },
				{ Forearms, new Type[]{typeof(ForearmItemData) } },
				{ Legs,     new Type[]{typeof(LegsItemData) } },
				{ Feet,     new Type[]{typeof(FeetItemData) } },
				{ Cloak,    new Type[]{typeof(BackItemData) } },
				{ Jewelry,  new Type[]{typeof(JewelryItemData) } },
				{ Ring0,    new Type[]{typeof(RingItemData) } },
				{ Ring1,    new Type[]{typeof(RingItemData) } },
			};

			Armors = new List<Equip>()
			{
				Head,
				Sholders,
				Chest,
				Forearms,
				Legs,
				Feet,
				Cloak,
				Jewelry,
				Ring0,
				Ring1,
			};

			Weapon0 = new EquipWeaponConnection()
			{
				Main		= Weapon00,
				Spare		= Weapon01,
				Inventory	= inventory,
			};
			Weapon1 = new EquipWeaponConnection()
			{
				Main		= Weapon10,
				Spare		= Weapon11,
				Inventory	= inventory,
			};

			Head.SetItem(settings.head);
		}


		public bool Add(Item item)
		{
			if(item.IsWeapon)
			{
				return Weapon0.Add(item);
			}
			else if(item.IsArmor)
			{
				return AddArmor(item);
			}

			return false;
		}
		public bool AddTo(Item item, Equip equip)
		{
			if (item.IsWeapon)
			{
				if (Weapon0.Contains(equip))
				{
					return Weapon0.AddTo(equip, item);
				}
				else if (Weapon1.Contains(equip))
				{
					return Weapon1.AddTo(equip, item);
				}
			}
			else if (item.IsArmor)
			{
				if (AddArmorTo(item, equip))
				{
					return true;
				}
				return AddArmor(item);
			}


			return false;
		}

		public bool RemoveFrom(Equip equip)
		{
			if (!equip.IsEmpty)
			{
				if (equip.Item.IsWeapon)
				{
					if (Weapon0.Contains(equip))
					{
						return Weapon0.RemoveFrom(equip);
					}
					else if (Weapon1.Contains(equip))
					{
						return Weapon1.RemoveFrom(equip);
					}
				}
				else if (equip.Item.IsArmor)
				{
					equip.SetItem(null);
					return true;
				}
			}

			return false;
		}


		private bool AddArmor(Item item)
		{
			foreach (var armor in Armors)
			{
				if(AddArmorTo(item, armor))
				{
					return true;
				}
			}

			return false;
		}
		private bool AddArmorTo(Item item, Equip equip)
		{
			if(IsCanAddTo(item, equip))
			{
				if (!equip.IsEmpty)
				{
					inventory.Add(equip.Item);
				}
				equip.SetItem(item);

				return true;
			}

			return false;
		}
		private bool IsCanAddTo(Item item, Equip equip)
		{
			if (dictionarySlotTypes.TryGetValue(equip, out Type[] types))
			{
				for (int i = 0; i < types.Length; i++)
				{
					if (types[i].IsAssignableFrom(item.ItemData.GetType()))
					{
						return true;
					}
				}
			}

			return false;
		}

		public bool Swap(Equip from, Equip to)
		{
			var weaponFrom = GetWeapon(from);
			var weaponTo = GetWeapon(to);

			if (weaponFrom == null && weaponTo == null)//armor swap
			{
				if (IsCanAddTo(from.Item, to))
				{
					from.Swap(to);
					return true;
				}
			}
			else if(weaponFrom != null && weaponTo != null)//weapon swap
			{
				return SwapWeapons(from, to);
			}

			return false;
		}


		private bool SwapWeapons(Equip from, Equip to)
		{
			var weaponFrom = GetWeapon(from);
			var weaponTo = GetWeapon(to);

			if (weaponFrom == weaponTo)//one handeds
			{
				from.Swap(to);
			}
			else
			{
				if (weaponTo.IsTwoEmpty)
				{
					if (from.Item.IsTwoHandedWeapon)
					{
						weaponFrom.Swap(weaponTo);
					}
					else
					{
						to.SetItem(from.Item);
						from.SetItem(null);
					}
				}
				else
				{
					if (to.IsEmpty)
					{
						if (from.Item.IsTwoHandedWeapon)
						{
							Item item = from.Item;
							weaponFrom.SetItemBoth(null);
							if (!weaponTo.Main.IsEmpty)
							{
								weaponFrom.Main.SetItem(weaponTo.Main.Item);
							}
							if (!weaponTo.Spare.IsEmpty)
							{
								weaponFrom.Spare.SetItem(weaponTo.Spare.Item);
							}

							weaponTo.SetItemBoth(item);
						}
						else
						{
							to.SetItem(from.Item);
							from.SetItem(null);
						}
					}
					else
					{
						if (from.Item.IsTwoHandedWeapon)
						{
							weaponFrom.Swap(weaponTo);
						}
						else
						{
							if (to.Item.IsTwoHandedWeapon)//OneHanded x TwoHanded
							{
								Item item = to.Item;
								weaponTo.SetItemBoth(null);

								to.SetItem(from.Item);
								from.SetItem(null);

								if (!weaponFrom.Main.IsEmpty)
								{
									weaponTo.Add(weaponFrom.Main.Item);
								}
								if (!weaponFrom.Spare.IsEmpty)
								{
									weaponTo.Add(weaponFrom.Spare.Item);
								}

								weaponFrom.SetItemBoth(item);
							}
							else//OneHanded x OneHanded
							{
								from.Swap(to);
							}
						}
					}
				}
			}
			return false;
		}


		private EquipWeaponConnection GetWeapon(Equip equip)
		{
			if (Weapon0.Contains(equip)) return Weapon0;
			if (Weapon1.Contains(equip)) return Weapon1;

			return null;
		}

		public Data GetData()
		{
			return new Data
			{
				head = Head.Item,
				sholders = Sholders.Item,
				chest = Chest.Item,
				forearms = Forearms.Item,
				legs = Legs.Item,
				feet = Feet.Item,

				weapon00 = Weapon00.Item,
				weapon01 = Weapon01.Item,
				weapon10 = Weapon10.Item,
				weapon11 = Weapon11.Item,

				cloak = Cloak.Item,
				jewelry = Jewelry.Item,
				ring0 = Ring0.Item,
				ring1 = Ring1.Item,
				trinket = Trinket.Item,
			};
		}

		public class Data 
		{
			public Item head;
			public Item sholders;
			public Item chest;
			public Item forearms;
			public Item legs;
			public Item feet;

			public Item weapon00;
			public Item weapon01;
			public Item weapon10;
			public Item weapon11;
			
			public Item cloak;
			public Item jewelry;
			public Item ring0;
			public Item ring1;
			public Item trinket;
		}
	}

	[System.Serializable]
	public class Equip
	{
		public UnityAction onEquipChanged;

		public bool Mark { get; set; }

		public bool IsEmpty => Item == null;
		public Item Item { get; private set; }

		public void SetItem(Item item)
		{
			if(item != null)
			{
				if (item.ItemData == null)
				{
					item = null;
				}
			}
			
			Item = item;

			onEquipChanged?.Invoke();
		}

		public void Swap(Equip equip)
		{
			Item item = equip.Item;
			equip.SetItem(Item);
			SetItem(item);
		}
	}

	public class EquipWeaponConnection
	{
		public bool IsTwoEmpty => Main.IsEmpty && Spare.IsEmpty;
		public bool IsEmpty => Main.IsEmpty || Spare.IsEmpty;

		public Equip Main { get; set; }
		public Equip Spare { get; set; }

		public IInventory Inventory { get; set; }

		public void Swap(EquipWeaponConnection weapon)
		{
			Item main = weapon.Main.Item;
			bool mark = weapon.Spare.Mark;
			Item spare = weapon.Spare.Item;
			
			weapon.Main.SetItem(Main.Item);
			weapon.Spare.Mark = Spare.Mark;
			weapon.Spare.SetItem(Spare.Item);

			Main.SetItem(main);
			Spare.Mark = mark;
			Spare.SetItem(spare);
		}

		public void SetItemBoth(Item item)
		{
			Main.SetItem(item);
			Spare.Mark = item != null;
			Spare.SetItem(item);
		}
		public void SetItemUp(Item item)
		{
			Main.SetItem(item);
			Spare.Mark = false;
			Spare.SetItem(null);
		}
		public void SetItemDown(Item item)
		{
			Main.SetItem(null);
			Spare.Mark = false;
			Spare.SetItem(item);
		}

		public bool Add(Item item)
		{
			if (item.IsTwoHandedWeapon)
			{
				if (Main.IsEmpty)//двухручное
				{
					if (!Spare.IsEmpty)
					{
						Inventory.Add(Spare.Item);
					}

					SetItemBoth(item);
					return true;
				}
				else
				{
					if (Main.Item.IsTwoHandedWeapon)//TwoHanded x TwoHanded
					{
						Inventory.Add(Main.Item);
						SetItemBoth(item);
						return true;
					}
					else//TwoHanded x OneHanded
					{
						Inventory.Add(Main.Item);
						if (!Spare.IsEmpty)
						{
							Inventory.Add(Spare.Item);
						}
						SetItemBoth(item);
						return true;
					}
				}
			}
			else
			{
				if (Main.IsEmpty)//одноручное
				{
					Main.SetItem(item);
					return true;
				}
				else
				{
					if (Main.Item.IsTwoHandedWeapon)//OneHanded x TwoHanded
					{
						Inventory.Add(Main.Item);
						SetItemUp(item);
						return true;
					}
					else
					{
						if (Spare.IsEmpty)
						{
							Spare.Mark = false;
							Spare.SetItem(item);
							return true;
						}
						else
						{
							Inventory.Add(Main.Item);
							Main.SetItem(item);
							return true;
						}
					}
				}
			}
		}
		public bool AddTo(Equip equip, Item item)
		{
			if (item.IsTwoHandedWeapon)
			{
				return Add(item);
			}
			else
			{
				if (equip.IsEmpty)
				{
					equip.SetItem(item);
					return true;
				}
				else
				{
					if (equip.Item.IsTwoHandedWeapon)
					{
						Inventory.Add(Main.Item);
						SetItemBoth(null);
						equip.SetItem(item);
						return true;
					}
					else
					{
						Inventory.Add(equip.Item);
						equip.SetItem(item);
						return true;
					}
				}
			}
		}

		public bool RemoveFrom(Equip equip)
		{
			if (equip.Item.IsTwoHandedWeapon)
			{
				SetItemBoth(null);
				return true;
			}
			else
			{
				equip.SetItem(null);
				return true;
			}
		}

		public bool Contains(Item item)
		{
			return item != null && (Main.Item == item || Spare.Item == item);
		}
		public bool Contains(Equip equip)
		{
			return equip != null && (Main == equip || Spare == equip);
		}
	}

	[System.Serializable]
	public class EquipmentSettings
	{
		public Item head;
		public Item sholders;
		public Item chest;
		public Item forearms;
		public Item legs;
		public Item feet;
		[Space]
		public Item weapon00;
		public Item weapon01;
		public Item weapon10;
		public Item weapon11;
		[Space]
		public Item cloak;
		public Item jewelry;
		public Item ring0;
		public Item ring1;
		public Item trinket;
	}
}