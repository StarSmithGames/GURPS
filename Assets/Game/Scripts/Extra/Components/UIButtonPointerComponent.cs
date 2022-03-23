using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonPointerComponent : Button
{
	public UnityAction<int> onClickChanged;


	public int LastClicks { get; private set; }

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);

		onClickChanged?.Invoke(eventData.clickCount);
	}
}