using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class Character : MonoBehaviour
{
	public Transform CameraPivot => cameraPivot;


	private Transform cameraPivot;
	private CharacterThirdPersonController controller;

	[Inject]
	private void Construct(
		[Inject(Id = "CameraPivot")] Transform cameraPivot,
		CharacterThirdPersonController controller)
	{
		this.cameraPivot = cameraPivot;
		this.controller = controller;
	}

	public void SetTarget()
	{

	}

	public void SetTarget(Vector3 destination)
	{
		controller.SetDestination(destination);
	}
}