using Game.Entities.Models;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem.Skills;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
    public class LocalSheet
    {
        public Skills.Skills Skills { get; private set; }

        public LocalSheet(ICharacterModel model, Skills.Skills skills)
        {
            Skills = skills;
			model.Character.SetLocalSheet(this);

			//fill
			int i = 0;
			Skills.CurrentActiveSkills.ForEach((skill) =>
			{
				model.Character.Sheet.ActionBar.Slots[i].SetAction(skill as ActiveSkill);

				var slot = new SlotSkill();
				slot.SetSkill(skill);
				model.Character.Sheet.SkillDeck.SkillSlots.Add(slot);

				i++;
			});
		}
	}
}