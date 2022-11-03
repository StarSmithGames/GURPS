using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ISlot : ICopyable<ISlot> { }
public interface ISlotable { }

public class RegistratorSlotItem<SLOT, ITEM>
	where SLOT : ISlot
	where ITEM : ISlotable
{
	public event UnityAction onCollectionChanged;

	public List<SlotItemBind<SLOT, ITEM>> registers;

	public RegistratorSlotItem()
	{
		registers = new List<SlotItemBind<SLOT, ITEM>>();
	}

	public virtual bool RegistrateBind(SlotItemBind<SLOT, ITEM> register)
	{
		if (!registers.Contains(register))
		{
			registers.Add(register);

			onCollectionChanged?.Invoke();

			return true;
		}

		return false;
	}

	public virtual void RegistrateBinds(IEnumerable<SlotItemBind<SLOT, ITEM>> registers)
	{
		foreach (var register in registers)
		{
			RegistrateBind(register);
		}
	}

	public virtual bool UnRegistrateBind(SlotItemBind<SLOT, ITEM> register)
	{
		if (registers.Contains(register))
		{
			registers.Remove(register);

			onCollectionChanged?.Invoke();

			return true;
		}

		return false;
	}
}

public class SlotItemBind<SLOT, ITEM>
	where SLOT : ISlot
	where ITEM : ISlotable
{
	public SLOT slot;
	public ITEM item;

	public bool IsEmpty => item == null;
}