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

        private IEntity entity;
        private IStatBar hitPoints;

		private void OnDestroy()
		{
            if (hitPoints != null)
            {
                hitPoints.onStatChanged -= OnHitPointsChanged;
            }
        }

		public void SetEntity(IEntity entity)
		{
            this.entity = entity;

            if (hitPoints != null)
			{
                hitPoints.onStatChanged -= OnHitPointsChanged;
            }
            hitPoints = entity?.Sheet.Stats.HitPoints;
            if(entity != null)
			{
                hitPoints.onStatChanged += OnHitPointsChanged;
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            Name.text = entity?.EntityData.CharacterName ?? "";
            Level.text = "Level 999";

            OnHitPointsChanged();
        }

        private void OnHitPointsChanged()
		{
            HealthBar.FillAmount = hitPoints?.PercentValue ?? 0.5f;
            HealthBar.BarText.text = hitPoints?.ToString();
        }
    }
}