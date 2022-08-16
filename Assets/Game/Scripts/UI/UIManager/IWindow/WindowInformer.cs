using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowInformer : MonoBehaviour
{
    [field: SerializeField] public CanvasGroup CanvasGroup { get; private set; }
    [field: SerializeField] public TMPro.TextMeshProUGUI Text { get; private set; }

    public void SetText(string text)
	{
        Text.text = text;
    }
}