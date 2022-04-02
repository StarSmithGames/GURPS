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
        private IStatBar hitPoints;

		private void OnDestroy()
		{
            if (hitPoints != null)
            {
                hitPoints.onStatChanged -= OnHitPointsChanged;
            }
        }

		public void SetSheet(ISheet sheet)
		{
            this.sheet = sheet;

            if (hitPoints != null)
			{
                hitPoints.onStatChanged -= OnHitPointsChanged;
            }
            hitPoints = sheet?.Stats.HitPoints;
            if(sheet != null)
			{
                hitPoints.onStatChanged += OnHitPointsChanged;
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (sheet == null) return;

            Name.text = sheet.Information.Name;
            Level.text = "Level 999";

            OnHitPointsChanged();
        }

        private void OnHitPointsChanged()
		{
            if (sheet == null) return;

            HealthBar.FillAmount = sheet.Settings.isImmortal ? 1f : hitPoints.PercentValue;
            HealthBar.BarText.text = sheet.Settings.isImmortal ? SymbolCollector.INFINITY.ToString() : hitPoints.ToString();
        }
    }
}