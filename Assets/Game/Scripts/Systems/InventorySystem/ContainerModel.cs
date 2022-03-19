using EPOOutline;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerModel : MonoBehaviour, IInteractable, IObservable
{
	[SerializeField] private Outlinable outline;


	private void Awake()
	{
		outline.enabled = false;
	}

	public void Interact()
	{
	}


	public void StartObserve()
	{
		outline.enabled = true;
	}
	public void Observe()
	{
	}
	public void EndObserve()
	{
		outline.enabled = false;
	}
}