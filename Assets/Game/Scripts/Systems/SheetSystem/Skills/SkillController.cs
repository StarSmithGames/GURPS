using Cinemachine;

using Game.Entities;
using Game.Entities.AI;
using Game.Entities.Models;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.InteractionSystem;

using System;

using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public class SkillController : IInitializable, IDisposable, ITickable
	{
		private bool isTargeting = false;

		private Vector3 worldPosition;
		private Plane plane = new Plane(Vector3.up, 0);

		private ICharacter character;
		private MarkPoint startPoint;
		private CinemachineBrain brain;
		private CameraVisionLocation cameraVision;

		public SkillController(ICharacterModel model, MarkPoint markPoint, CinemachineBrain brain, CameraVisionLocation cameraVision)
		{
			this.character = model.Character;
			this.startPoint = markPoint;
			this.brain = brain;
			this.cameraVision = cameraVision;
		}

		public void Initialize()
		{
			character.Skills.onPreparedSkillChanged += OnPreparedSkillChanged;
		}

		public void Dispose()
		{
			character.Skills.onPreparedSkillChanged -= OnPreparedSkillChanged;
		}

		public virtual void Tick()
		{
			if (isTargeting)
			{
				float distance;
				Ray ray = brain.OutputCamera.ScreenPointToRay(Input.mousePosition);
				if (plane.Raycast(ray, out distance))
				{
					worldPosition = ray.GetPoint(distance);
				}

				if (cameraVision.CurrentObserve != null)
				{
					if (cameraVision.CurrentObserve is IDamageable damageable)
					{
						worldPosition = damageable.MarkPoint.transform.position;
					}
				}

				character.Model.Markers.LineMarker.DrawLine(new Vector3[] { startPoint.transform.position, worldPosition });

				if (Input.GetMouseButtonDown(1))
				{
					character.Skills.CancelPreparation();
				}
			}
		}

		private void OnPreparedSkillChanged()
		{
			isTargeting = character.Skills.IsHasPreparedSkill;
		}
	}
}