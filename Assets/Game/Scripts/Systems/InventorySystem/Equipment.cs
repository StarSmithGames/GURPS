using Game.Systems.CutomizationSystem;
using Game.Systems.DamageSystem;

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

		public EquipWeapon WeaponCurrent => isWeaponMain ? WeaponMain : WeaponSpare;
		public EquipWeapon WeaponMain { get; private set; }
		public EquipWeapon WeaponSpare { get; private set; }

		public Equip Cloak { get; private set; }
		public Equip Jewelry { get; private set; }
		public Equip Ring0 { get; private set; }
		public Equip Ring1 { get; private set; }
		public Equip Trinket { get; private set; }

		public Dictionary<Equip, Type[]> ArmorsByTypes { get; private set; }

		private bool isWeaponMain = true;

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

			WeaponMain = new EquipWeapon(new Equip(), new Equip(), inventory);
			WeaponSpare = new EquipWeapon(new Equip(), new Equip(), inventory);

			Cloak		= new Equip();
			Jewelry		= new Equip();
			Ring0		= new Equip();
			Ring1		= new Equip();
			Trinket		= new Equip();

			ArmorsByTypes = new Dictionary<Equip, Type[]>()
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

			Head.SetItem(settings.head);
		}


		public bool Add(Item item)
		{
			if(item.IsWeapon)
			{
				return WeaponMain.Add(item);
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
				if (WeaponMain.Contains(equip))
				{
					return WeaponMain.AddTo(equip, item);
				}
				else if (WeaponSpare.Contains(equip))
				{
					return WeaponSpare.AddTo(equip, item);
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
					if (WeaponMain.Contains(equip))
					{
						return WeaponMain.RemoveFrom(equip);
					}
					else if (WeaponSpare.Contains(equip))
					{
						return WeaponSpare.RemoveFrom(equip);
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
			foreach (var armor in ArmorsByTypes)
			{
				if(AddArmorTo(item, armor.Key))
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
			if (ArmorsByTypes.TryGetValue(equip, out Type[] types))
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
				if (weaponTo.IsEmpty)
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

			weaponTo?.onEquipWeaponChanged();
			weaponFrom?.onEquipWeaponChanged();

			return false;
		}


		private EquipWeapon GetWeapon(Equip equip)
		{
			if (WeaponMain.Contains(equip)) return WeaponMain;
			return WeaponSpare;
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

				//weapon00 = Weapon00.Item,
				//weapon01 = Weapon01.Item,
				//weapon10 = Weapon10.Item,
				//weapon11 = Weapon11.Item,

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

	public class EquipWeapon
	{
		public UnityAction onEquipWeaponChanged;

		public bool IsEmpty => Main.IsEmpty && Spare.IsEmpty;

		public Equip Main { get; }
		public Equip Spare { get; }

		public Hands Hands { get; private set; }

		public IInventory Inventory { get; }

		public EquipWeapon(Equip main, Equip spare, IInventory inventory)
		{
			Main = main;
			Spare = spare;
			Inventory = inventory;

			onEquipWeaponChanged += OnEquipWeaponChanged;

			OnEquipWeaponChanged();
		}

		public void Swap(EquipWeapon weapon)
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

			onEquipWeaponChanged?.Invoke();
			weapon?.onEquipWeaponChanged();
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
					onEquipWeaponChanged?.Invoke();
					return true;
				}
				else
				{
					if (Main.Item.IsTwoHandedWeapon)//TwoHanded x TwoHanded
					{
						Inventory.Add(Main.Item);
						SetItemBoth(item);
						onEquipWeaponChanged?.Invoke();
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
						onEquipWeaponChanged?.Invoke();
						return true;
					}
				}
			}
			else
			{
				if (Main.IsEmpty)//одноручное
				{
					Main.SetItem(item);
					onEquipWeaponChanged?.Invoke();
					return true;
				}
				else
				{
					if (Main.Item.IsTwoHandedWeapon)//OneHanded x TwoHanded
					{
						Inventory.Add(Main.Item);
						SetItemUp(item);
						onEquipWeaponChanged?.Invoke();
						return true;
					}
					else
					{
						if (Spare.IsEmpty)
						{
							Spare.Mark = false;
							Spare.SetItem(item);
							onEquipWeaponChanged?.Invoke();
							return true;
						}
						else
						{
							Inventory.Add(Main.Item);
							Main.SetItem(item);
							onEquipWeaponChanged?.Invoke();
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
					onEquipWeaponChanged?.Invoke();
					return true;
				}
				else
				{
					if (equip.Item.IsTwoHandedWeapon)
					{
						Inventory.Add(Main.Item);
						SetItemBoth(null);
						equip.SetItem(item);
						onEquipWeaponChanged?.Invoke();
						return true;
					}
					else
					{
						Inventory.Add(equip.Item);
						equip.SetItem(item);
						onEquipWeaponChanged?.Invoke();
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
				onEquipWeaponChanged?.Invoke();
				return true;
			}
			else
			{
				equip.SetItem(null);
				onEquipWeaponChanged?.Invoke();
				return true;
			}
		}

		public bool Contains(Equip equip)
		{
			return equip != null && (Main == equip || Spare == equip);
		}


		private void OnEquipWeaponChanged()
		{
			if (Main.IsEmpty && Spare.IsEmpty)
			{
				Hands = Hands.None;
			}
			else if (!Main.IsEmpty && Spare.IsEmpty)
			{
				Hands = Hands.Main;
			}
			else if (Main.IsEmpty && !Spare.IsEmpty)
			{
				Hands = Hands.Spare;
			}
			else
			{
				Hands = Hands.Both;
			}
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



	public enum Hands : int
	{
		None = -1,
		Main = 0,
		Spare = 1,
		Both = 2,
	}
}