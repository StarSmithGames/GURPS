using Cinemachine;

using EPOOutline;

using Game.Entities.AI;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.CursorSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.NavigationSystem;
using Game.Systems.VFX;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.SheetSystem.Skills
{
    public abstract class TargetSkill : ActiveSkill
    {
		protected MarkPoint startPoint;
		protected List<IDamageable> targets = new List<IDamageable>();
		protected List<LineTargetVFX> lines = new List<LineTargetVFX>();

		private ActiveTargetSkillData TargetSkillData => Data as ActiveTargetSkillData;
		private IDamageable currentTarget;
		private LineTargetVFX currentLine;

		private Dictionary<ProjectileVFX, IDamageable> projectilesWithTargets = new Dictionary<ProjectileVFX, IDamageable>();
		private int projectileCompletedCount = 0;

		private bool clampOnTarget = true;
		private Vector3 worldPosition;
		private Plane plane = new Plane(Vector3.up, 0);
		private OutlineData targetOutline;

		private CinemachineBrain brain;
		private CameraVisionLocation cameraVision;
		private CursorSystem.CursorSystem cursorSystem;
		private LineTargetVFX.Factory lineTargetFactory;
		private CombatFactory combatFactory;

		[Inject]
		private void Construct(CinemachineBrain brain,
			CameraVisionLocation cameraVision,
			CursorSystem.CursorSystem cursorSystem,
			LineTargetVFX.Factory lineTargetFactory,
			CombatFactory combatFactory)
		{
			this.startPoint = character.Model.MarkPoint;

			this.brain = brain;
			this.cameraVision = cameraVision;
			this.cursorSystem = cursorSystem;
			this.lineTargetFactory = lineTargetFactory;
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

				if (currentTarget != null && clampOnTarget)
				{
					worldPosition = currentTarget.MarkPoint.transform.position;
				}

				currentLine.DrawLine(new Vector3[] { startPoint.transform.position, worldPosition });
				(character.Model.Controller as CharacterController3D).RotateTo(worldPosition);

				if (Input.GetMouseButtonDown(0))
				{
					if(currentTarget != null)
					{
						AddTarget(currentTarget);
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
			targetOutline = GlobalDatabase.Instance.allOutlines.Find((x) => x.outlineType == OutlineType.Target);

			cursorSystem.SetCursor(CursorType.Base);
			cameraVision.IsCanMouseClick = false;

			character.Model.Freeze(true);

			AddLine();

			base.BeginProcess();
		}

		protected virtual void AttackProcess()
		{
			StartCooldown();
			SetStatus(SkillStatus.Running);

			character.Model.TaskSequence
			.Append(() =>
			{
				for (int i = 0; i < targets.Count; i++)
				{
					var projectile = GetProjectile();

					projectilesWithTargets.Add(projectile, targets[i]);

					projectile
						.SetStart(startPoint.transform.position, startPoint.transform.forward)
						.SetTarget(targets[i].MarkPoint.transform)
						.Launch(OnProjectileCompleted);
				}
			})
			//.Append(currentCombat.AttackAnimation)
			//.Append(new TaskWaitAttack(character.Model.AnimatorController))
			.Execute();
		}

		protected abstract ProjectileVFX GetProjectile();

		private void AddLine()
		{
			currentLine = lineTargetFactory.Create();
			currentLine.SetState(LineTargetState.Target);
			lines.Add(currentLine);
		}
		
		private void AddTarget(IDamageable damageable)
		{
			targets.Add(damageable);

			currentLine.SetState(LineTargetState.Targeted);

			if (targets.Count == TargetSkillData.targetCount)
			{
				AttackProcess();
			}
			else
			{
				AddLine();
			}
		}

		private void SetTarget(IDamageable damageable)
		{
			if (currentTarget != null)
			{
				currentTarget.Outline.ResetData();
			}

			currentTarget = damageable != character.Model ? damageable : null;

			if (currentTarget != null)
			{
				currentTarget.Outline.SetData(targetOutline);
			}
		}

		protected override void SetStatus(SkillStatus status)
		{
			if (status == SkillStatus.Canceled || status == SkillStatus.Faulted || status == SkillStatus.Successed)
			{
				cursorSystem.SetCursor(CursorType.Hand);
				cameraVision.IsCanMouseClick = true;

				for (int i = 0; i < lines.Count; i++)
				{
					lines[i].DespawnIt();
				}
				character.Model.Freeze(false);

				SetTarget(null);
			}
			else if (status == SkillStatus.Running)
			{
				for (int i = 0; i < lines.Count; i++)
				{
					lines[i].FadeOut();
				}
			}

			base.SetStatus(status);
		}

		protected override void ResetSkill()
		{
			base.ResetSkill();

			currentTarget = null;
			targets.Clear();
			lines.Clear();

			projectileCompletedCount = 0;
			projectilesWithTargets.Clear();
		}

		protected void OnProjectileCompleted(ProjectileVFX projectile)
		{
			projectilesWithTargets.TryGetValue(projectile, out IDamageable damageable);

			if (damageable != null)
			{
				ICombat currentCombat = combatFactory.Create(character.Model, damageable);
				currentCombat.DealDamage();
			}

			projectileCompletedCount++;

			if (projectileCompletedCount == targets.Count)
			{
				SetStatus(SkillStatus.Successed);
			}
		}
	}
}