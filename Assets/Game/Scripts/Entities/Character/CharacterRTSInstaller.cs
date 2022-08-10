using UnityEngine;

using Zenject;

public class CharacterRTSInstaller : MonoInstaller
{
	[SerializeField] private CharacterController characterController;

	public override void InstallBindings()
	{
		Container.BindInstance(characterController);
	}
}