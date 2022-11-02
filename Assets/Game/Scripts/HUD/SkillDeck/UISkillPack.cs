using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace Game.HUD
{
    public class UISkillPack : PoolableObject
    {
        [field: SerializeField] public TMPro.TextMeshProUGUI Level { get; private set; }
        [field: SerializeField] public Transform Content { get; private set; }

		private bool isInitialized = false;
		private List<UISlotSkill> slots = new List<UISlotSkill>();
		private ISkill[] skills;

		private UISlotSkill.Factory slotFactory;

		[Inject]
		private void Construct(UISlotSkill.Factory slotFactory)
		{
			this.slotFactory = slotFactory;
		}

		public void SetSkills(ISkill[] skills)
		{
			this.skills = skills;
		}

		public override void OnSpawned(IMemoryPool pool)
		{
			if (!isInitialized)
			{
				Content.DestroyChildren();
			}

			isInitialized = true;

			CollectionExtensions.Resize(skills, slots,
			() =>
			{
				var slot = slotFactory.Create();
				slot.transform.SetParent(Content);
				slot.transform.localScale = Vector3.one;

				return slot;
			},
			() =>
			{
				var slot = slots.Last();
				slot.DespawnIt();

				return slot;
			});

			for (int i = 0; i < slots.Count; i++)
			{
				//slots[i].SetItem();
			}

			base.OnSpawned(pool);
		}

		public class Factory: PlaceholderFactory<UISkillPack> { }
    }
}