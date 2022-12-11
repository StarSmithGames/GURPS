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
    public abstract partial class TargetSkill : ActiveSkill
    {
		protected MarkPoint startPoint;
		protected List<IDamageable> targets = new List<IDamageable>();
		protected List<LineTargetVFX> lines = new List<LineTargetVFX>();
		protected List<RadialAreaDecalVFX> areas = new List<RadialAreaDecalVFX>();
		protected LineTargetVFX currentLine;
		protected RadialAreaDecalVFX currentArea;
		protected RadialAreaDecalVFX rangeArea;

		private ActiveTargetSkillData TargetSkillData => Data as ActiveTargetSkillData;
		private IDamageable currentTarget;
		private IDamageable lastTarget;

		private Dictionary<ProjectileVFX, IDamageable> projectilesWithTargets = new Dictionary<ProjectileVFX, IDamageable>();
		private int projectileCompletedCount = 0;

		private bool clampOnTarget = true;
		private Vector3 worldPosition;
		private Plane plane = new Plane(Vector3.up, 0);
		private NavigationPath path;
		private OutlineData targetOutline;

		private CinemachineBrain brain;
		private CameraVisionLocation cameraVision;
		private CursorSystem.CursorSystem cursorSystem;
		private PointerVFX pointer;
		private LineTargetVFX.Factory lineTargetFactory;
		private RadialAreaDecalVFX.Factory areaFactory;
		private CombatFactory combatFactory;
		private TooltipSystem.TooltipSystem tooltipSystem;

		[Inject]
		private void Construct(CinemachineBrain brain,
			CameraVisionLocation cameraVision,
			CursorSystem.CursorSystem cursorSystem,
			PointerVFX pointer,
			LineTargetVFX.Factory lineTargetFactory,
			RadialAreaDecalVFX.Factory areaFactory,
			CombatFactory combatFactory,
			TooltipSystem.TooltipSystem tooltipSystem)
		{
			this.startPoint = character.Model.MarkPoint;

			this.brain = brain;
			this.cameraVision = cameraVision;
			this.cursorSystem = cursorSystem;
			this.pointer = pointer;
			this.lineTargetFactory = lineTargetFactory;
			this.areaFactory = areaFactory;
			this.combatFactory = combatFactory;
			this.tooltipSystem = tooltipSystem;
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
				DrawProps();
				
				(character.Model.Controller as CharacterController3D).RotateTo(worldPosition);

				UpdateTooltipRange();

				if (Input.GetMouseButtonDown(0))
				{
					AddTarget(currentTarget);
				}
				else if (Input.GetMouseButtonDown(1))
				{
					CancelProcess();
				}
			}
		}

		public override void BeginProcess()
		{
			path = new NavigationPath();

			targetOutline = GlobalDatabase.Instance.allOutlines.Find((x) => x.outlineType == OutlineType.Target);

			cursorSystem.SetCursor(CursorType.Base);
			cameraVision.IsCanMouseClick = false;
			character.Model.Freeze(true);

			//Visual
			CreatePropsForTarget();
			UpdateTooltipPath();
			UpdateTooltipTargets();

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

			SetStatus(SkillStatus.Done);
		}

		private void AddTarget(IDamageable damageable)
		{
			if (damageable == null) return;

			//Range Check
			if (isHasRange)
			{
				if (!Range.IsIn(character.Model.Transform.position, currentTarget.MarkPoint.transform.position, TargetSkillData.range.range))
					return;
			}
			else
			{
				if(TargetSkillData.range.rangeType == RangeType.None)
				{
					if (damageable != character.Model) return;
				}
			}
			
			//Add
			targets.Add(damageable);
			
			//Visual
			currentLine?.SetState(LineTargetState.Targeted);
			currentArea?.FadeTo(0.25f);
			UpdateTooltipTargets();

			if (targets.Count == TargetSkillData.targetCount)
			{
				AttackProcess();
			}
			else
			{
				CreatePropsForTarget();
			}
		}

		private void SetTarget(IDamageable damageable)
		{
			if (currentTarget != null)
			{
				currentTarget.Outline.ResetData();
			}

			lastTarget = currentTarget;
			currentTarget = CheckTarget(damageable);

			if (currentTarget != null)
			{
				currentTarget.Outline.SetData(targetOutline);
			}

			if (currentTarget != lastTarget && SkillStatus == SkillStatus.Preparing)
			{
				OnTargetChanged();
			}

			ClampTarget();
		}

		private IDamageable CheckTarget(IDamageable damageable)
		{
			if (TargetSkillData.isCanTargetSelf && damageable == character.Model ||
				damageable != character.Model)
			{
				return damageable;
			}

			return null;
		}

		private void ClampTarget()
		{
			if (TargetSkillData.isCanClampOnTarget)
			{
				if (currentTarget != null && clampOnTarget)
				{
					worldPosition = currentTarget.MarkPoint.transform.position;
				}
			}
		}

		protected override void SetStatus(SkillStatus status)
		{
			base.SetStatus(status);

			if (status == SkillStatus.Canceled || status == SkillStatus.Done)
			{
				cursorSystem.SetCursor(CursorType.Hand);
				cameraVision.IsCanMouseClick = true;
				tooltipSystem.SetRuller(TooltipSystem.TooltipRulerType.None);
				tooltipSystem.SetMessage(TooltipSystem.TooltipMessageType.None);
				tooltipSystem.SetMessage("", TooltipSystem.TooltipAdditionalMessageType.None);

				SetTarget(null);

				character.Model.Freeze(false);

				FadeOutProps();

				if (status == SkillStatus.Canceled)
				{
					ResetSkill();
				}
			}
			else if (status == SkillStatus.Running)
			{
				FadeOutProps();
			}
		}

		protected override void ResetSkill()
		{
			base.ResetSkill();

			currentTarget = null;
			targets.Clear();
			lines.Clear();
			areas.Clear();
			currentArea = null;
			rangeArea = null;

			projectileCompletedCount = 0;
			projectilesWithTargets.Clear();
		}

		private void OnTargetChanged()
		{
			if(currentTarget == null)
			{
				UpdateTooltipPath();
			}
			else
			{
				tooltipSystem.SetRullerChance("100%");
			}
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

		protected abstract ProjectileVFX GetProjectile();
	}


	public partial class TargetSkill
	{
		private bool isHasRange = true;

		private void DrawProps()
		{
			pointer.SetPosition(worldPosition);

			path.SetPath(new Vector3[] { startPoint.transform.position, worldPosition });

			if (TargetSkillData.path.pathType == PathType.Line)
			{
				currentLine?.DrawLine(path.Path.ToArray());//TargetSkillData.path.drawPath
			}
			else if(TargetSkillData.path.pathType == PathType.Ballistic)
			{
				currentLine?.DrawLine(KinematicBallistic.GetTraectory(character.Model.MarkPoint.transform.position, worldPosition));
			}

			currentArea?.DrawDecal(worldPosition);//TargetSkillData.isAoE
		}

		private void FadeOutProps()
		{
			pointer.Enable(false);
			for (int i = 0; i < lines.Count; i++)
			{
				lines[i].FadeOut();
			}
			if (TargetSkillData.isAoE)
			{
				for (int i = 0; i < areas.Count; i++)
				{
					areas[i].FadeTo(0);
				}
			}
			if (isHasRange)
			{
				rangeArea.FadeTo(0);
			}
		}

		private void CreatePropsForTarget()
		{
			pointer.Enable(true);

			if (TargetSkillData.path.drawPath)
			{
				//Line
				currentLine = lineTargetFactory.Create();
				currentLine.SetState(LineTargetState.Target);
				lines.Add(currentLine);
			}
			if (TargetSkillData.isAoE)
			{
				//Area
				currentArea = areaFactory.Create();
				currentArea.SetRange(TargetSkillData.AoE.range);
				currentArea.SetFade(0).FadeTo(0.8f);
				areas.Add(currentArea);
			}

			isHasRange = TargetSkillData.range.rangeType == RangeType.Custom;
			if (isHasRange && rangeArea == null)
			{
				rangeArea = areaFactory.Create();
				rangeArea.SetRange(TargetSkillData.range.range);
				rangeArea.SetFade(0).FadeTo(0.8f);
				rangeArea.DrawDecal(character.Model.Transform.position);
			}
		}

		private void UpdateTooltipRange()
		{
			if (isHasRange)
			{
				if (!Range.IsIn(character.Model.Transform.position, worldPosition, TargetSkillData.range.range))
				{
					tooltipSystem.SetMessage(TooltipSystem.TooltipMessageType.OutOfRange);
				}
				else
				{
					tooltipSystem.SetMessage(TooltipSystem.TooltipMessageType.None);
				}
			}
		}

		private void UpdateTooltipPath()
		{
			tooltipSystem.SetRullerPath(path);
			tooltipSystem.SetRuller(TooltipSystem.TooltipRulerType.CustomPath);
		}

		private void UpdateTooltipTargets()
		{
			if (TargetSkillData.targetCount > 1)
			{
				tooltipSystem.SetMessage($"{targets.Count}/{TargetSkillData.targetCount}", TooltipSystem.TooltipAdditionalMessageType.Projectiles);
			}
		}
	}
}