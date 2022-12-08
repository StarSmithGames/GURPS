using Cinemachine;

using EPOOutline;

using Game.Entities;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.CursorSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.NavigationSystem;
using Game.Systems.VFX;

using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
	public class BlitzBoltSkill : ActiveSkill
	{
		public override SkillData Data => data;
		protected BlitzBoltData data;

		private Vector3 worldPosition;
		private Plane plane = new Plane(Vector3.up, 0);

		private IDamageable target;
		private OutlineData targetOutline;

		private ElectricBallProjectileVFX.Factory electricBallFactory;
		private MarkPoint startPoint;
		private CinemachineBrain brain;
		private CameraVisionLocation cameraVision;
		private CursorSystem.CursorSystem cursorSystem;
		private CombatFactory combatFactory;

		[Inject]
		public void Construct(BlitzBoltData data, ICharacter character,
			ElectricBallProjectileVFX.Factory electricBallFactory,
			CinemachineBrain brain,
			CameraVisionLocation cameraVision,
			CursorSystem.CursorSystem cursorSystem,
			CombatFactory combatFactory)
		{
			this.data = data;
			this.character = character;

			this.electricBallFactory = electricBallFactory;
			this.startPoint = character.Model.MarkPoint;
			this.brain = brain;
			this.cameraVision = cameraVision;
			this.cursorSystem = cursorSystem;
			this.combatFactory = combatFactory;
		}

		protected override void Update()
		{
			base.Update();

			if (isCooldown) return;

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
				(character.Model.Controller as CharacterController3D).RotateTo(worldPosition);


				if (Input.GetMouseButtonDown(0))
				{
					if (target != null)
					{
						AttackProcess();
					}
				}
				else if (Input.GetMouseButtonDown(1))
				{
					CancelProcess();
				}
			}
		}

		public override void BeginProcess()
		{
			cursorSystem.SetCursor(CursorType.Base);
			cameraVision.IsCanMouseClick = false;

			targetOutline = GlobalDatabase.Instance.allOutlines.Find((x) => x.outlineType == OutlineType.Target);

			character.Model.Freeze(true);
			character.Model.Markers.EnableSingleTargetLine(true);
			base.BeginProcess();
		}

		private void AttackProcess()
		{
			StartCooldown();
			SetStatus(SkillStatus.Running);

			character.Model.TaskSequence
				.Append(() =>
				{
					var projectile = electricBallFactory.Create();
					projectile
						.SetStart(startPoint.transform.position, startPoint.transform.forward)
						.SetTarget(target.MarkPoint.transform)
						.Launch(OnProjectileCompleted);
				})
				//.Append(currentCombat.AttackAnimation)
				//.Append(new TaskWaitAttack(character.Model.AnimatorController))
				.Execute();
		}

		private void SetTarget(IDamageable damageable)
		{
			if (target != null)
			{
				target.Outline.ResetData();
			}

			target = damageable != character.Model ? damageable : null;

			if (target != null)
			{
				target.Outline.SetData(targetOutline);
			}
		}

		protected override void SetStatus(SkillStatus status)
		{
			if(status == SkillStatus.Canceled || status == SkillStatus.Faulted || status == SkillStatus.Successed)
			{
				cursorSystem.SetCursor(CursorType.Hand);
				cameraVision.IsCanMouseClick = true;

				character.Model.Markers.EnableSingleTargetLine(false);
				character.Model.Freeze(false);

				SetTarget(null);
			}
			else if(status == SkillStatus.Running)
			{
				character.Model.Markers.EnableSingleTargetLine(false);
			}

			base.SetStatus(status);
		}

		private void OnProjectileCompleted()
		{
			ICombat currentCombat = combatFactory.Create(character.Model, target);
			currentCombat.DealDamage();

			SetStatus(SkillStatus.Successed);
		}

		public class Factory : PlaceholderFactory<BlitzBoltData, ICharacter, BlitzBoltSkill> { }
	}
}