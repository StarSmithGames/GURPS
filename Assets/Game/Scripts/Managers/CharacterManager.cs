using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

using Zenject;

public class CharacterManager : IInitializable, IDisposable
{
	public Character CurrentCharacter;

	private List<Character> characters = new List<Character>();

	private CameraController cameraController;

	public CharacterManager(CameraController cameraController)
	{
		this.cameraController = cameraController;
	}

	public void Initialize()
	{
		characters = GameObject.FindObjectsOfType<Character>().ToList();//stub

		SetCharacter(characters.FirstOrDefault());
	}

	public void Dispose()
	{
	}

	public void SetCharacter(Character character)
{
		CurrentCharacter = character;
		cameraController.SetFollowTarget(CurrentCharacter.CameraPivot);
	}

	public void SetCharacter(int index)
	{
		CurrentCharacter = characters[index];
		cameraController.SetFollowTarget(CurrentCharacter.CameraPivot);
	}
}