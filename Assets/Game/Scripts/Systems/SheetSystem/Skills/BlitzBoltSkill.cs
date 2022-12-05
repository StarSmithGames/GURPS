using Cinemachine;

using EPOOutline;

using Game.Entities.Models;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.CursorSystem;
using Game.Systems.InteractionSystem;
using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public class BlitzBoltSkill : Skill
	{
		private Vector3 worldPosition;
		private Plane plane = new Plane(Vector3.up, 0);

		private IDamageable target;
		private OutlineData targetOutline;
		private ICombat currentCombat;

		private BlitzBoltSkill.Factory factory;
		private MarkPoint startPoint;
		private CinemachineBrain brain;
		private CameraVisionLocation cameraVision;
		private CursorSystem.CursorSystem cursorSystem;
		private CombatFactory combatFactory;

		[Inject]
		private void Construct(BlitzBoltSkill.Factory factory,
			MarkPoint markPoint,
			CinemachineBrain brain,
			CameraVisionLocation cameraVision,
			CursorSystem.CursorSystem cursorSystem,
			CombatFactory combatFactory)
		{
			this.factory = factory;
			this.startPoint = markPoint;
			this.brain = brain;
			this.cameraVision = cameraVision;
			this.cursorSystem = cursorSystem;
			this.combatFactory = combatFactory;
		}

		private void Start()
		{
			character.Model.ActiveSkillsRegistrator.Registrate(this);
		}

		protected override void Update()
		{
			if (SkillStatus == SkillStatus.Preparing)
			{
				float distance;
				Ray ray = brain.OutputCamera.ScreenPointToRay(Input.mousePosition);
				if (plane.Raycast(ray, out distance))
				{
					worldPosition = ray.GetPoint(distance);
				}

				SetTarget(cameraVision.CurrentObserve as IDamageable);

				if (target != null)
				{
					worldPosition = target.MarkPoint.transform.position;
				}

				character.Model.Markers.LineMarker.DrawLine(new Vector3[] { startPoint.transform.position, worldPosition });

				if (Input.GetMouseButtonDown(0))
				{
					if (target != null)
					{
						Attack();
						character.Skills.CancelPreparation();
					}
				}
				else if (Input.GetMouseButtonDown(1))
				{
					character.Skills.CancelPreparation();
				}
			}
		}

		public override void BeginProcess()
		{
			cursorSystem.SetCursor(CursorType.Base);

			targetOutline = GlobalDatabase.Instance.allOutlines.Find((x) => x.outlineType == OutlineType.Target);

			character.Model.Freeze(true);
			character.Model.Markers.EnableSingleTargetLine(true);
			base.BeginProcess();
		}

		public override void CancelProcess()
		{
			cursorSystem.SetCursor(CursorType.Hand);

			character.Model.Markers.EnableSingleTargetLine(false);
			character.Model.Freeze(false);

			SetTarget(null);

			base.CancelProcess();
		}

		private void Attack()
		{
			currentCombat = combatFactory.Create(character.Model, target);

			character.Model.TaskSequence
				.Append(currentCombat.AttackAnimation)
				.Append(new TaskWaitAttack(character.Model.AnimatorController))
				.Execute();
		}

		private void SetTarget(IDamageable damageable)
		{
			if (target != null)
			{
				(target as Model).Outline.ResetData();
			}

			target = damageable != character.Model ? damageable : null;

			if(target != null)
			{
				(target as Model).Outline.SetData(targetOutline);
			}
		}

		public override ISkill Create()
		{
			return factory.Create();
		}

		public class Factory : PlaceholderFactory<BlitzBoltSkill> { }
	}
}