using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
	public bool IsShowing { get; protected set; }

	public void Enable(bool trigger)
	{
		gameObject.SetActive(trigger);
		IsShowing = trigger;
	}
}