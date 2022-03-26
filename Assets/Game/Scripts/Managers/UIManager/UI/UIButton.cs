using UnityEngine;

public class UIButton : MonoBehaviour
{
    [field: SerializeField] public UIButtonPointer ButtonPointer { get; private set; }

    public void Enable(bool trigger)
	{
		gameObject.SetActive(trigger);
	}
}