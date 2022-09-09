using Cinemachine;

using Game.Entities.Models;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.SheetSystem.Abilities;
using Game.UI;

using System.Collections.Generic;

using UnityEngine;

using Zenject;

namespace Game
{
	public class LocationInstaller : MonoInstaller
	{
		public UISubCanvas subCanvas;
		[Header("Cameras")]
		public CinemachineBrain brainCamera;
		public List<CinemachineVirtualCamera> characterCamers = new List<CinemachineVirtualCamera>();

		public override void InstallBindings()
		{
			Container.BindInstance(subCanvas);

			CombatDamageSystemInstaller.Install(Container);

			BindAbilityActivators();

			BindCameras();
		}

		private void BindCameras()
		{
			Container.BindInstance(brainCamera);
			Container.BindInstance(characterCamers).WithId("Camers");

			Container.BindInterfacesTo<CameraVisionLocation>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<CameraController>().AsSingle().NonLazy();
		}

		private void BindAbilityActivators()
		{
			Container.BindFactory<IAbility, InstantActivation, InstantActivation.Factory>().NonLazy();
			Container.BindFactory<IAbility, CastActivation, CastActivation.Factory>().NonLazy();

			Container
				.BindFactory<IAbility, IActivation, ActivationFactory>()
				.FromFactory<CustomActivationFactory>()
				.NonLazy();
		}
	}
}