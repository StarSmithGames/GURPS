using Game.Systems.SheetSystem;

using UnityEngine;

namespace Game.UI
{
    public class UIStatBar : UIBar
    {
        public IStatBar CurrentStat { get; private set; }
        public bool IsImmortal { get; private set; }

        private void OnDestroy()
        {
            if (CurrentStat != null)
            {
                CurrentStat.onModifiersChanged -= UpdateUI;
                CurrentStat.onChanged -= UpdateUI;
            }
        }

		public void SetStat(IStatBar stat, bool isImmortal = false)
        {
            if (CurrentStat != null)
            {
                CurrentStat.onModifiersChanged -= UpdateUI;
                CurrentStat.onChanged -= UpdateUI;
            }
            CurrentStat = stat;
            IsImmortal = isImmortal;

            if (CurrentStat != null)
            {
                CurrentStat.onModifiersChanged += UpdateUI;
                CurrentStat.onChanged += UpdateUI;

				FillAmount = CurrentStat.PercentValue;
				FillAmountWhite = FillAmount;
			}

            UpdateUI();
        }

        private void UpdateUI()
        {
            FillAmount = CurrentStat?.PercentValue ?? 1f;

            if (isHasText)
            {
                BarText.text = IsImmortal ? SymbolCollector.INFINITY.ToString() : CurrentStat?.Output;
            }
        }
    }
}