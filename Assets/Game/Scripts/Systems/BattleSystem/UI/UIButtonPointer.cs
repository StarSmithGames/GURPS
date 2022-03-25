using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonPointer : Button
{
	public UnityAction<int> onClickChanged;

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);

		onClickChanged?.Invoke(eventData.clickCount);
	}
}