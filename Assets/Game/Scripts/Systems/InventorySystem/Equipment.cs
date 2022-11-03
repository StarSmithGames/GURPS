//using System;
//using System.Collections.Generic;

//using UnityEngine;
//using UnityEngine.Events;

//namespace Game.Systems.InventorySystem
//{
//	public interface IEquipment : IInventory
//	{
//		event UnityAction OnEquipmentChanged;

//		EquipSlot Head { get; }
//		EquipSlot Sholders { get; }
//		EquipSlot Chest { get; }
//		EquipSlot Forearms { get; }
//		EquipSlot Legs { get; }
//		EquipSlot Feet { get; }

//		EquipWeapon WeaponCurrent { get; }
//		EquipWeapon WeaponMain { get; }
//		EquipWeapon WeaponSpare { get; }

//		EquipSlot Cloak { get; }
//		EquipSlot Jewelry { get; }
//		EquipSlot Ring0 { get; }
//		EquipSlot Ring1 { get; }
//		EquipSlot Trinket { get; }

//		bool AddTo(Item item, EquipSlot equip);

//		bool RemoveFrom(EquipSlot equip);

//		bool Swap(EquipSlot from, EquipSlot to);
//	}

//	/// <summary>
//	/// IInventory implementation
//	/// </summary>
//	public partial class Equipment : IEquipment
//	{
//		public bool IsEmpty { get; }
//		public List<Item> Items { get; }
//		public List<Slot> Slots { get; }

//		public event UnityAction OnInventoryChanged;

//		public bool Add(Item item, bool notify = true)
//		{
//			throw new NotImplementedException();
//		}

//		public bool Remove(Item item, bool notify = true)
//		{
//			throw new NotImplementedException();
//		}

//		public void Clear(bool notify = true)
//		{
//			throw new NotImplementedException();
//		}
//	}


//	public partial class Equipment : IEquipment
//	{
//		public event UnityAction OnEquipmentChanged;

//		public EquipSlot Head { get; private set; }
//		public EquipSlot Sholders { get; private set; }
//		public EquipSlot Chest { get; private set; }
//		public EquipSlot Forearms { get; private set; }
//		public EquipSlot Legs { get; private set; }
//		public EquipSlot Feet { get; private set; }

//		public EquipWeapon WeaponCurrent => isWeaponMain ? WeaponMain : WeaponSpare;
//		public EquipWeapon WeaponMain { get; private set; }
//		public EquipWeapon WeaponSpare { get; private set; }

//		public EquipSlot Cloak { get; private set; }
//		public EquipSlot Jewelry { get; private set; }
//		public EquipSlot Ring0 { get; private set; }
//		public EquipSlot Ring1 { get; private set; }
//		public EquipSlot Trinket { get; private set; }

//		public Dictionary<EquipSlot, Type[]> ArmorsByTypes { get; private set; }

//		private bool isWeaponMain = true;

//		private EquipmentSettings settings;
//		private IInventory inventory;

//		public Equipment(EquipmentSettings settings, IInventory inventory)
//		{
//			this.settings = settings;
//			this.inventory = inventory;

//			Initialization();
//		}

//		private void Initialization()
//		{
//			Head		= new EquipSlot();
//			Sholders	= new EquipSlot();
//			Chest		= new EquipSlot();
//			Forearms	= new EquipSlot();
//			Legs		= new EquipSlot();
//			Feet		= new EquipSlot();

//			WeaponMain = new EquipWeapon(new EquipSlot(), new EquipSlot(), inventory);
//			WeaponSpare = new EquipWeapon(new EquipSlot(), new EquipSlot(), inventory);

//			Cloak		= new EquipSlot();
//			Jewelry		= new EquipSlot();
//			Ring0		= new EquipSlot();
//			Ring1		= new EquipSlot();
//			Trinket		= new EquipSlot();

//			ArmorsByTypes = new Dictionary<EquipSlot, Type[]>()
//			{
//				{ Head,     new Type[]{typeof(HeadItemData)} },
//				{ Sholders, new Type[]{typeof(ShouldersItemData) } },
//				{ Chest,    new Type[]{typeof(ChestItemData) } },
//				{ Forearms, new Type[]{typeof(ForearmItemData) } },
//				{ Legs,     new Type[]{typeof(LegsItemData) } },
//				{ Feet,     new Type[]{typeof(FeetItemData) } },
//				{ Cloak,    new Type[]{typeof(BackItemData) } },
//				{ Jewelry,  new Type[]{typeof(JewelryItemData) } },
//				{ Ring0,    new Type[]{typeof(RingItemData) } },
//				{ Ring1,    new Type[]{typeof(RingItemData) } },
//			};

//			Head.SetItem(settings.head);
//		}


//		public bool Add(Item item)
//		{
//			if(item.IsWeapon)
//			{
//				return WeaponMain.Add(item);
//			}
//			else if(item.IsArmor)
//			{
//				return AddArmor(item);
//			}

//			return false;
//		}
//		public bool AddTo(Item item, UISlotEquipment equip)
//		{
//			if (item.IsWeapon)
//			{
//				if (WeaponMain.Contains(equip))
//				{
//					return WeaponMain.AddTo(equip, item);
//				}
//				else if (WeaponSpare.Contains(equip))
//				{
//					return WeaponSpare.AddTo(equip, item);
//				}
//			}
//			else if (item.IsArmor)
//			{
//				if (AddArmorTo(item, equip))
//				{
//					return true;
//				}
//				return AddArmor(item);
//			}


//			return false;
//		}

//		public bool RemoveFrom(EquipSlot equip)
//		{
//			if (!equip.IsEmpty)
//			{
//				if (equip.CurrentItem.IsWeapon)
//				{
//					if (WeaponMain.Contains(equip))
//					{
//						return WeaponMain.RemoveFrom(equip);
//					}
//					else if (WeaponSpare.Contains(equip))
//					{
//						return WeaponSpare.RemoveFrom(equip);
//					}
//				}
//				else if (equip.CurrentItem.IsArmor)
//				{
//					equip.SetItem(null);
//					OnEquipmentChanged?.Invoke();
//					return true;
//				}
//			}

//			return false;
//		}


//		private bool AddArmor(Item item)
//		{
//			foreach (var armor in ArmorsByTypes)
//			{
//				if(AddArmorTo(item, armor.Key))
//				{
//					return true;
//				}
//			}

//			return false;
//		}
//		private bool AddArmorTo(Item item, EquipSlot equip)
//		{
//			if(IsCanAddTo(item, equip))
//			{
//				if (!equip.IsEmpty)
//				{
//					inventory.Add(equip.CurrentItem);
//				}
//				equip.SetItem(item);
//				OnEquipmentChanged?.Invoke();
//				return true;
//			}

//			return false;
//		}
//		private bool IsCanAddTo(Item item, EquipSlot equip)
//		{
//			if (ArmorsByTypes.TryGetValue(equip, out Type[] types))
//			{
//				for (int i = 0; i < types.Length; i++)
//				{
//					if (types[i].IsAssignableFrom(item.ItemData.GetType()))
//					{
//						return true;
//					}
//				}
//			}

//			return false;
//		}

//		public bool Swap(EquipSlot from, EquipSlot to)
//		{
//			var weaponFrom = GetWeapon(from);
//			var weaponTo = GetWeapon(to);

//			if (weaponFrom == null && weaponTo == null)//armor swap
//			{
//				if (IsCanAddTo(from.CurrentItem, to))
//				{
//					from.Swap(to);
//					OnEquipmentChanged?.Invoke();
//					return true;
//				}
//			}
//			else if(weaponFrom != null && weaponTo != null)//weapon swap
//			{
//				return SwapWeapons(from, to);
//			}

//			return false;
//		}


//		private bool SwapWeapons(EquipSlot from, EquipSlot to)
//		{
//			var weaponFrom = GetWeapon(from);
//			var weaponTo = GetWeapon(to);

//			if (weaponFrom == weaponTo)//one handeds
//			{
//				from.Swap(to);
//			}
//			else
//			{
//				if (weaponTo.IsEmpty)
//				{
//					if (from.CurrentItem.IsTwoHandedWeapon)
//					{
//						weaponFrom.Swap(weaponTo);
//					}
//					else
//					{
//						to.SetItem(from.CurrentItem);
//						from.SetItem(null);
//					}
//				}
//				else
//				{
//					if (to.IsEmpty)
//					{
//						if (from.CurrentItem.IsTwoHandedWeapon)
//						{
//							Item item = from.CurrentItem;
//							weaponFrom.SetItemBoth(null);
//							if (!weaponTo.Main.IsEmpty)
//							{
//								weaponFrom.Main.SetItem(weaponTo.Main.CurrentItem);
//							}
//							if (!weaponTo.Spare.IsEmpty)
//							{
//								weaponFrom.Spare.SetItem(weaponTo.Spare.CurrentItem);
//							}

//							weaponTo.SetItemBoth(item);
//						}
//						else
//						{
//							to.SetItem(from.CurrentItem);
//							from.SetItem(null);
//						}
//					}
//					else
//					{
//						if (from.CurrentItem.IsTwoHandedWeapon)
//						{
//							weaponFrom.Swap(weaponTo);
//						}
//						else
//						{
//							if (to.CurrentItem.IsTwoHandedWeapon)//OneHanded x TwoHanded
//							{
//								Item item = to.CurrentItem;
//								weaponTo.SetItemBoth(null);

//								to.SetItem(from.CurrentItem);
//								from.SetItem(null);

//								if (!weaponFrom.Main.IsEmpty)
//								{
//									weaponTo.Add(weaponFrom.Main.CurrentItem);
//								}
//								if (!weaponFrom.Spare.IsEmpty)
//								{
//									weaponTo.Add(weaponFrom.Spare.CurrentItem);
//								}

//								weaponFrom.SetItemBoth(item);
//							}
//							else//OneHanded x OneHanded
//							{
//								from.Swap(to);
//							}
//						}
//					}
//				}
//			}

//			weaponTo?.onEquipWeaponChanged();
//			weaponFrom?.onEquipWeaponChanged();

//			return false;
//		}


//		private EquipWeapon GetWeapon(EquipSlot equip)
//		{
//			if (WeaponMain.Contains(equip)) return WeaponMain;
//			return WeaponSpare;
//		}

//		public Data GetData()
//		{
//			return new Data
//			{
//				head = Head.CurrentItem,
//				sholders = Sholders.CurrentItem,
//				chest = Chest.CurrentItem,
//				forearms = Forearms.CurrentItem,
//				legs = Legs.CurrentItem,
//				feet = Feet.CurrentItem,

//				//weapon00 = Weapon00.Item,
//				//weapon01 = Weapon01.Item,
//				//weapon10 = Weapon10.Item,
//				//weapon11 = Weapon11.Item,

//				cloak = Cloak.CurrentItem,
//				jewelry = Jewelry.CurrentItem,
//				ring0 = Ring0.CurrentItem,
//				ring1 = Ring1.CurrentItem,
//				trinket = Trinket.CurrentItem,
//			};
//		}

//		public class Data 
//		{
//			public Item head;
//			public Item sholders;
//			public Item chest;
//			public Item forearms;
//			public Item legs;
//			public Item feet;

//			public Item weapon00;
//			public Item weapon01;
//			public Item weapon10;
//			public Item weapon11;
			
//			public Item cloak;
//			public Item jewelry;
//			public Item ring0;
//			public Item ring1;
//			public Item trinket;
//		}
//	}

//	public class EquipWeapon
//	{
//		public UnityAction onEquipWeaponChanged;

//		public bool IsEmpty => Main.IsEmpty && Spare.IsEmpty;

//		public EquipSlot Main { get; }
//		public EquipSlot Spare { get; }

//		public Hands Hands { get; private set; }

//		public IInventory Inventory { get; }

//		public EquipWeapon(EquipSlot main, EquipSlot spare, IInventory inventory)
//		{
//			Main = main;
//			Spare = spare;
//			Inventory = inventory;

//			onEquipWeaponChanged += OnEquipWeaponChanged;

//			OnEquipWeaponChanged();
//		}

//		public void Swap(EquipWeapon weapon)
//		{
//			Item main = weapon.Main.CurrentItem;
//			bool mark = weapon.Spare.Mark;
//			Item spare = weapon.Spare.CurrentItem;
			
//			weapon.Main.SetItem(Main.CurrentItem);
//			weapon.Spare.Mark = Spare.Mark;
//			weapon.Spare.SetItem(Spare.CurrentItem);

//			Main.SetItem(main);
//			Spare.Mark = mark;
//			Spare.SetItem(spare);

//			onEquipWeaponChanged?.Invoke();
//			weapon?.onEquipWeaponChanged();
//		}

//		public void SetItemBoth(Item item)
//		{
//			Main.SetItem(item);
//			Spare.Mark = item != null;
//			Spare.SetItem(item);
//		}
//		public void SetItemUp(Item item)
//		{
//			Main.SetItem(item);
//			Spare.Mark = false;
//			Spare.SetItem(null);
//		}
//		public void SetItemDown(Item item)
//		{
//			Main.SetItem(null);
//			Spare.Mark = false;
//			Spare.SetItem(item);
//		}

//		public bool Add(Item item)
//		{
//			if (item.IsTwoHandedWeapon)
//			{
//				if (Main.IsEmpty)//����������
//				{
//					if (!Spare.IsEmpty)
//					{
//						Inventory.Add(Spare.CurrentItem);
//					}

//					SetItemBoth(item);
//					onEquipWeaponChanged?.Invoke();
//					return true;
//				}
//				else
//				{
//					if (Main.CurrentItem.IsTwoHandedWeapon)//TwoHanded x TwoHanded
//					{
//						Inventory.Add(Main.CurrentItem);
//						SetItemBoth(item);
//						onEquipWeaponChanged?.Invoke();
//						return true;
//					}
//					else//TwoHanded x OneHanded
//					{
//						Inventory.Add(Main.CurrentItem);
//						if (!Spare.IsEmpty)
//						{
//							Inventory.Add(Spare.CurrentItem);
//						}
//						SetItemBoth(item);
//						onEquipWeaponChanged?.Invoke();
//						return true;
//					}
//				}
//			}
//			else
//			{
//				if (Main.IsEmpty)//����������
//				{
//					Main.SetItem(item);
//					onEquipWeaponChanged?.Invoke();
//					return true;
//				}
//				else
//				{
//					if (Main.CurrentItem.IsTwoHandedWeapon)//OneHanded x TwoHanded
//					{
//						Inventory.Add(Main.CurrentItem);
//						SetItemUp(item);
//						onEquipWeaponChanged?.Invoke();
//						return true;
//					}
//					else
//					{
//						if (Spare.IsEmpty)
//						{
//							Spare.Mark = false;
//							Spare.SetItem(item);
//							onEquipWeaponChanged?.Invoke();
//							return true;
//						}
//						else
//						{
//							Inventory.Add(Main.CurrentItem);
//							Main.SetItem(item);
//							onEquipWeaponChanged?.Invoke();
//							return true;
//						}
//					}
//				}
//			}
//		}
//		public bool AddTo(EquipSlot equip, Item item)
//		{
//			if (item.IsTwoHandedWeapon)
//			{
//				return Add(item);
//			}
//			else
//			{
//				if (equip.IsEmpty)
//				{
//					equip.SetItem(item);
//					onEquipWeaponChanged?.Invoke();
//					return true;
//				}
//				else
//				{
//					if (equip.CurrentItem.IsTwoHandedWeapon)
//					{
//						Inventory.Add(Main.CurrentItem);
//						SetItemBoth(null);
//						equip.SetItem(item);
//						onEquipWeaponChanged?.Invoke();
//						return true;
//					}
//					else
//					{
//						Inventory.Add(equip.CurrentItem);
//						equip.SetItem(item);
//						onEquipWeaponChanged?.Invoke();
//						return true;
//					}
//				}
//			}
//		}

//		public bool RemoveFrom(EquipSlot equip)
//		{
//			if (equip.CurrentItem.IsTwoHandedWeapon)
//			{
//				SetItemBoth(null);
//				onEquipWeaponChanged?.Invoke();
//				return true;
//			}
//			else
//			{
//				equip.SetItem(null);
//				onEquipWeaponChanged?.Invoke();
//				return true;
//			}
//		}

//		public bool Contains(EquipSlot equip)
//		{
//			return equip != null && (Main == equip || Spare == equip);
//		}


//		private void OnEquipWeaponChanged()
//		{
//			if (Main.IsEmpty && Spare.IsEmpty)
//			{
//				Hands = Hands.None;
//			}
//			else if (!Main.IsEmpty && Spare.IsEmpty)
//			{
//				Hands = Hands.Main;
//			}
//			else if (Main.IsEmpty && !Spare.IsEmpty)
//			{
//				Hands = Hands.Spare;
//			}
//			else
//			{
//				Hands = Hands.Both;
//			}
//		}
//	}

//	[System.Serializable]
//	public class EquipmentSettings
//	{
//		public Item head;
//		public Item sholders;
//		public Item chest;
//		public Item forearms;
//		public Item legs;
//		public Item feet;
//		[Space]
//		public Item weapon00;
//		public Item weapon01;
//		public Item weapon10;
//		public Item weapon11;
//		[Space]
//		public Item cloak;
//		public Item jewelry;
//		public Item ring0;
//		public Item ring1;
//		public Item trinket;
//	}



//	public enum Hands : int
//	{
//		None = -1,
//		Main = 0,
//		Spare = 1,
//		Both = 2,
//	}
//}