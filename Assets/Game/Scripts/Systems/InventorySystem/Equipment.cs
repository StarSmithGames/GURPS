using Game.Systems.SheetSystem;

using Sirenix.OdinInspector;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Game.Systems.InventorySystem
{
	public partial class Equipment
	{
		public event UnityAction OnEquipmentChanged;

		public SlotEquipment Head { get; private set; }
		public SlotEquipment Sholders { get; private set; }
		public SlotEquipment Chest { get; private set; }
		public SlotEquipment Forearms { get; private set; }
		public SlotEquipment Legs { get; private set; }
		public SlotEquipment Feet { get; private set; }
		public SlotEquipment Cloak { get; private set; }
		public SlotEquipment Jewelry { get; private set; }
		public SlotEquipment Ring0 { get; private set; }
		public SlotEquipment Ring1 { get; private set; }
		public SlotEquipment Trinket { get; private set; }

		public SlotWeaponEquipment CurrentWeapon => isWeaponMain ? WeaponMain : WeaponSpare;
		public SlotWeaponEquipment WeaponMain { get; private set; }
		public SlotWeaponEquipment WeaponSpare { get; private set; }

		public Dictionary<SlotEquipment, Type[]> ArmorsByTypes { get; private set; }

		private bool isWeaponMain = true;

		public Equipment(EquipmentSettings settings, ISheet sheet)
		{
			Head = settings.head.Copy();
			Sholders = settings.sholders.Copy();
			Chest = settings.chest.Copy();
			Forearms = settings.forearms.Copy();
			Legs = settings.legs.Copy();
			Feet = settings.feet.Copy();
			Cloak = settings.cloak.Copy();
			Jewelry = settings.jewelry.Copy();
			Ring0 = settings.ring0.Copy();
			Ring1 = settings.ring1.Copy();
			Trinket = settings.trinket.Copy();

			WeaponMain = settings.main.Copy();
			WeaponSpare = settings.spare.Copy();

			ArmorsByTypes = new Dictionary<SlotEquipment, Type[]>()
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
		}

		public bool Add(Item item)
		{
			if (item.IsWeapon)
			{
				return false; //WeaponMain.Add(item);
			}
			else if (item.IsArmor)
			{
				return AddArmor(item);
			}

			return false;
		}
		public bool AddTo(Item item, UISlotEquipment equip)
		{
			if (item.IsWeapon)
			{
				//if (WeaponMain.Contains(equip))
				//{
				//	return WeaponMain.AddTo(equip, item);
				//}
				//else if (WeaponSpare.Contains(equip))
				//{
				//	return WeaponSpare.AddTo(equip, item);
				//}
			}
			else if (item.IsArmor)
			{
				//if (AddArmorTo(item, equip))
				//{
				//	return true;
				//}
				return AddArmor(item);
			}


			return false;
		}

		public bool RemoveFrom(SlotEquipment equip)
		{
			if (!equip.IsEmpty)
			{
				if (equip.item.IsWeapon)
				{
					//if (WeaponMain.Contains(equip))
					//{
					//	return WeaponMain.RemoveFrom(equip);
					//}
					//else if (WeaponSpare.Contains(equip))
					//{
					//	return WeaponSpare.RemoveFrom(equip);
					//}
				}
				else if (equip.item.IsArmor)
				{
					equip.SetItem(null);
					OnEquipmentChanged?.Invoke();
					return true;
				}
			}

			return false;
		}


		private bool AddArmor(Item item)
		{
			foreach (var armor in ArmorsByTypes)
			{
				if (AddArmorTo(item, armor.Key))
				{
					return true;
				}
			}

			return false;
		}
		private bool AddArmorTo(Item item, SlotEquipment equip)
		{
			if (IsCanAddTo(item, equip))
			{
				if (!equip.IsEmpty)
				{
					//inventory.Add(equip.item);
				}
				equip.SetItem(item);
				OnEquipmentChanged?.Invoke();
				return true;
			}

			return false;
		}
		private bool IsCanAddTo(Item item, SlotEquipment equip)
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

		public bool Swap(SlotEquipment from, SlotEquipment to)
		{
			var weaponFrom = GetWeapon(from);
			var weaponTo = GetWeapon(to);

			if (weaponFrom == null && weaponTo == null)//armor swap
			{
				if (IsCanAddTo(from.item, to))
				{
					from.Swap(to);
					OnEquipmentChanged?.Invoke();
					return true;
				}
			}
			else if (weaponFrom != null && weaponTo != null)//weapon swap
			{
				return SwapWeapons(from, to);
			}

			return false;
		}


		private bool SwapWeapons(SlotEquipment from, SlotEquipment to)
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
					if (from.item.IsTwoHandedWeapon)
					{
						weaponFrom.Swap(weaponTo);
					}
					else
					{
						to.SetItem(from.item);
						from.SetItem(null);
					}
				}
				else
				{
					if (to.IsEmpty)
					{
						if (from.item.IsTwoHandedWeapon)
						{
							Item item = from.item;
							weaponFrom.SetItemBoth(null);
							if (!weaponTo.Main.IsEmpty)
							{
								weaponFrom.Main.SetItem(weaponTo.Main.item);
							}
							if (!weaponTo.Spare.IsEmpty)
							{
								weaponFrom.Spare.SetItem(weaponTo.Spare.item);
							}

							weaponTo.SetItemBoth(item);
						}
						else
						{
							to.SetItem(from.item);
							from.SetItem(null);
						}
					}
					else
					{
						if (from.item.IsTwoHandedWeapon)
						{
							weaponFrom.Swap(weaponTo);
						}
						else
						{
							if (to.item.IsTwoHandedWeapon)//OneHanded x TwoHanded
							{
								Item item = to.item;
								weaponTo.SetItemBoth(null);

								to.SetItem(from.item);
								from.SetItem(null);

								if (!weaponFrom.Main.IsEmpty)
								{
									weaponTo.Add(weaponFrom.Main.item);
								}
								if (!weaponFrom.Spare.IsEmpty)
								{
									weaponTo.Add(weaponFrom.Spare.item);
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


		private EquipWeapon GetWeapon(SlotEquipment equip)
		{
			//if (WeaponMain.Contains(equip)) return WeaponMain;
			return null;//WeaponSpare;
		}

		public Data GetData()
		{
			return new Data
			{
				head = Head.item,
				sholders = Sholders.item,
				chest = Chest.item,
				forearms = Forearms.item,
				legs = Legs.item,
				feet = Feet.item,

				//weapon00 = Weapon00.Item,
				//weapon01 = Weapon01.Item,
				//weapon10 = Weapon10.Item,
				//weapon11 = Weapon11.Item,

				cloak = Cloak.item,
				jewelry = Jewelry.item,
				ring0 = Ring0.item,
				ring1 = Ring1.item,
				trinket = Trinket.item,
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

	public class EquipWeapon
	{
		public UnityAction onEquipWeaponChanged;

		public bool IsEmpty => Main.IsEmpty && Spare.IsEmpty;

		public SlotEquipment Main { get; }
		public SlotEquipment Spare { get; }

		public Hands Hands { get; private set; }

		public Inventory Inventory { get; }

		public EquipWeapon(SlotEquipment main, SlotEquipment spare, Inventory inventory)
		{
			Main = main;
			Spare = spare;
			Inventory = inventory;

			onEquipWeaponChanged += OnEquipWeaponChanged;

			OnEquipWeaponChanged();
		}

		public void Swap(EquipWeapon weapon)
		{
			Item main = weapon.Main.item;
			bool mark = weapon.Spare.Mark;
			Item spare = weapon.Spare.item;

			weapon.Main.SetItem(Main.item);
			weapon.Spare.Mark = Spare.Mark;
			weapon.Spare.SetItem(Spare.item);

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
				if (Main.IsEmpty)//����������
				{
					if (!Spare.IsEmpty)
					{
						Inventory.Add(Spare.item);
					}

					SetItemBoth(item);
					onEquipWeaponChanged?.Invoke();
					return true;
				}
				else
				{
					if (Main.item.IsTwoHandedWeapon)//TwoHanded x TwoHanded
					{
						Inventory.Add(Main.item);
						SetItemBoth(item);
						onEquipWeaponChanged?.Invoke();
						return true;
					}
					else//TwoHanded x OneHanded
					{
						Inventory.Add(Main.item);
						if (!Spare.IsEmpty)
						{
							Inventory.Add(Spare.item);
						}
						SetItemBoth(item);
						onEquipWeaponChanged?.Invoke();
						return true;
					}
				}
			}
			else
			{
				if (Main.IsEmpty)//����������
				{
					Main.SetItem(item);
					onEquipWeaponChanged?.Invoke();
					return true;
				}
				else
				{
					if (Main.item.IsTwoHandedWeapon)//OneHanded x TwoHanded
					{
						Inventory.Add(Main.item);
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
							Inventory.Add(Main.item);
							Main.SetItem(item);
							onEquipWeaponChanged?.Invoke();
							return true;
						}
					}
				}
			}
		}
		public bool AddTo(SlotEquipment equip, Item item)
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
					if (equip.item.IsTwoHandedWeapon)
					{
						Inventory.Add(Main.item);
						SetItemBoth(null);
						equip.SetItem(item);
						onEquipWeaponChanged?.Invoke();
						return true;
					}
					else
					{
						Inventory.Add(equip.item);
						equip.SetItem(item);
						onEquipWeaponChanged?.Invoke();
						return true;
					}
				}
			}
		}

		public bool RemoveFrom(SlotEquipment equip)
		{
			if (equip.item.IsTwoHandedWeapon)
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

		public bool Contains(SlotEquipment equip)
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
		public SlotEquipment head;
		public SlotEquipment sholders;
		public SlotEquipment chest;
		public SlotEquipment forearms;
		public SlotEquipment legs;
		public SlotEquipment feet;
		public SlotEquipment cloak;
		public SlotEquipment jewelry;
		public SlotEquipment ring0;
		public SlotEquipment ring1;
		public SlotEquipment trinket;
		[Space]
		public SlotWeaponEquipment main;
		public SlotWeaponEquipment spare;
	}

	public enum Hands : int
	{
		None = -1,
		Main = 0,
		Spare = 1,
		Both = 2,
	}
}