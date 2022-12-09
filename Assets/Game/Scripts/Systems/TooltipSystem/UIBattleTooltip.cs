using UnityEngine;

namespace Game.Systems.TooltipSystem
{
	public class UIBattleTooltip : MonoBehaviour
	{
		[field: SerializeField] public RectTransform Tooltip { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Ruler { get; private set; }
		[field: SerializeField] public TMPro.TextMeshProUGUI Message { get; private set; }
	}
}