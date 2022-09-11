using Game;
using Game.Systems.SheetSystem;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    public float FillAmount
    {
        get => Bar.fillAmount;
        set => Bar.fillAmount = value;
    }

    [field: SerializeField] public bool IsHasText = false;

    [field: SerializeField] public Image Bar { get; private set; }
    [field: ShowIf("IsHasText")]
    [field: SerializeField] public TMPro.TextMeshProUGUI BarText { get; private set; }

    public IStatBar CurrentStat { get; private set; }
    public bool IsImmortal { get; private set; }

	private void OnDestroy()
	{
        if (CurrentStat != null)
        {
            CurrentStat.onStatChanged -= UpdateUI;
        }
    }

	public void SetStat(IStatBar stat, bool isImmortal = false)
	{
        if (CurrentStat != null)
        {
            CurrentStat.onStatChanged -= UpdateUI;
        }
        CurrentStat = stat;
        IsImmortal = isImmortal;

        if (CurrentStat != null)
        {
            CurrentStat.onStatChanged += UpdateUI;
        }

        UpdateUI();
    }

    private void UpdateUI()
	{
        FillAmount = CurrentStat?.PercentValue ?? 1f;

        if (IsHasText)
        {
            BarText.text = IsImmortal ? SymbolCollector.INFINITY.ToString() : CurrentStat?.ToString();
        }
    }
}