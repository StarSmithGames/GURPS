using Cinemachine;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.InteractionSystem;
using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public class BlitzBoltSkill : Skill
	{
		private Vector3 worldPosition;
		private Plane plane = new Plane(Vector3.up, 0);

		private BlitzBoltSkill.Factory factory;
		private MarkPoint startPoint;
		private CinemachineBrain brain;
		private CameraVisionLocation cameraVision;

		[Inject]
		private void Construct(BlitzBoltSkill.Factory factory, MarkPoint markPoint, CinemachineBrain brain, CameraVisionLocation cameraVision)
		{
			this.factory = factory;
			this.startPoint = markPoint;
			this.brain = brain;
			this.cameraVision = cameraVision;
		}

		private void Start()
		{
			character.Model.ActiveSkillsRegistrator.Registrate(this);
		}

		protected override void Update()
		{
			base.Update();

			if (SkillStatus == SkillStatus.Preparing)
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
			}
		}

		public override void BeginProcess()
		{
			character.Model.Freeze(true);
			character.Model.Markers.EnableSingleTargetLine(true);
			base.BeginProcess();
		}

		public override void CancelProcess()
		{
			character.Model.Markers.EnableSingleTargetLine(false);
			character.Model.Freeze(false);
			base.CancelProcess();
		}

		public override ISkill Create()
		{
			return factory.Create();
		}

		public class Factory : PlaceholderFactory<BlitzBoltSkill> { }
	}
}