using Cinemachine;

using Game.Entities;
using Game.Entities.AI;
using Game.Entities.Models;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.InteractionSystem;

using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public abstract class Skill : MonoBehaviour, ISkill
	{
		public event UnityAction<SkillStatus> onStatusChanged;

		public SkillData data;

		public bool IsActive { get; protected set; }
		public SkillStatus SkillStatus { get; private set; }

		protected ICharacter character;
		private Vector3 worldPosition;
		private Plane plane = new Plane(Vector3.up, 0);
		private bool isTargeting = false;

		private MarkPoint startPoint;
		private CinemachineBrain brain;
		private CameraVisionLocation cameraVision;

		[Inject]
		private void Construct(ICharacterModel model, MarkPoint markPoint, CinemachineBrain brain, CameraVisionLocation cameraVision)
		{
			this.character = model.Character;
			this.startPoint = markPoint;
			this.brain = brain;
			this.cameraVision = cameraVision;
		}

		protected virtual void Update()
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

				if (Input.GetMouseButtonDown(0))
				{

				}
				else if (Input.GetMouseButtonDown(1))
				{
					character.Skills.CancelPreparation();
				}
			}
		}

		public abstract ISkill Create();

		public virtual void Activate()
		{
			IsActive = true;
		}

		public virtual void Deactivate()
		{
			IsActive = false;
		}

		public string Title => $"{(data.information.name.IsEmpty() ? name : data.information.name)}";
	}

	public enum SkillStatus
	{
		None,

		Prepared,
		Running,
		Canceled,
		Faulted,
		Success,
	}
}