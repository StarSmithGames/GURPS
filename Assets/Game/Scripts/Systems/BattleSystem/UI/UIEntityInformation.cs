using Game.Entities;
using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.Systems.BattleSystem
{
    public class UIEntityInformation : MonoBehaviour
    {
        [field: SerializeField] public TMPro.TextMeshProUGUI Name { get; private set; }
        [field: SerializeField] public UIBar HealthBar { get; private set; }
        [field: SerializeField] public TMPro.TextMeshProUGUI Level { get; private set; }

        private ISheet sheet;

		public void SetSheet(ISheet sheet)
		{
            this.sheet = sheet;

            HealthBar.SetStat(sheet?.Stats.HitPoints, sheet?.Settings.isImmortal ?? false);

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (sheet == null) return;

            Name.text = sheet.Information.Name;
            Level.text = sheet.Settings.identity != Identity.Lifeless ? $"Level {sheet.Stats.Level}" : "";
        }
    }
}