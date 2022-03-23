using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    public float FillAmount
    {
        get => Bar.fillAmount;
        set => Bar.fillAmount = value;
    }

    [field: SerializeField] public Image Bar { get; private set; }
    [field: SerializeField] public TMPro.TextMeshProUGUI BarText { get; private set; }
}
