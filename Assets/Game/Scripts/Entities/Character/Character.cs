using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class Character : MonoBehaviour, IEntity
{
	public Transform Transform => transform;
	public CharacterController3D Controller { get; private set; }

	public Transform CameraPivot => cameraPivot;

	private Transform cameraPivot;
	

	[Inject]
	private void Construct(
		[Inject(Id = "CameraPivot")] Transform cameraPivot,
		CharacterController3D controller)
	{
		this.cameraPivot = cameraPivot;
		Controller = controller;
	}

	public void SetTarget()
	{

	}

	public void InteractWith(IObservable observable)
	{
		switch (observable)
		{
			case IInteractable interactable:
			{
				interactable.InteractFrom(this);
				break;
			}
		}
	}
}