using Cinemachine;

using EPOOutline;

using Game.Entities.AI;
using Game.Systems.BattleSystem.TargetSystem;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.CursorSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.NavigationSystem;
using Game.Systems.VFX;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Zenject;

using static UnityEngine.Networking.UnityWebRequest;

namespace Game.Systems.SheetSystem.Skills
{
    public abstract partial class TargetSkill : ActiveSkill
    {
		protected MarkPoint startPoint;

		private ActiveTargetSkillData TargetSkillData => Data as ActiveTargetSkillData;

		private List<IDamageable> targets = new List<IDamageable>();

		private Dictionary<ProjectileVFX, IDamageable> projectilesWithTargets = new Dictionary<ProjectileVFX, IDamageable>();
		private int projectileCompletedCount = 0;
		private bool isHasRange = true;

		private TargetController targetController;
		private CombatFactory combatFactory;

		[Inject]
		private void Construct(TargetController targetController, CombatFactory combatFactory)
		{
			this.startPoint = character.Model.MarkPoint;
			this.targetController = targetController;
			this.combatFactory = combatFactory;
		}

		protected override void Update()
		{
			base.Update();

			if (isCooldown) return;

			if (SkillStatus == SkillStatus.Preparing)
			{
				targetController.Tick();
				(character.Model.Controller as CharacterController3D).RotateTo(targetController.LastWorldPosition);

				if (Input.GetMouseButtonDown(0))
				{
					targetController.AddTarget(targetController.LastTarget);
				}
				else if (Input.GetMouseButtonDown(1))
				{
					CancelProcess();
				}
			}
		}

		public override void BeginProcess()
		{
			character.Model.Freeze(true);

			targetController.onTargetChanged += OnTargetChanged;
			targetController.onTargetValid += OnTargetValid;
			targetController.Begin(character, TargetSkillData);
			isHasRange = TargetSkillData.range.rangeType == RangeType.Custom;

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

		protected override void SetStatus(SkillStatus status)
		{
			base.SetStatus(status);

			if (status == SkillStatus.Canceled || status == SkillStatus.Done)
			{
				character.Model.Freeze(false);

				targetController.FadeOutProps();

				if (status == SkillStatus.Canceled)
				{
					ResetSkill();
				}
			}
			else if (status == SkillStatus.Running)
			{
				targetController.FadeOutProps();
			}
		}

		protected override void ResetSkill()
		{
			base.ResetSkill();

			projectileCompletedCount = 0;
			projectilesWithTargets.Clear();

			targetController.End();
		}

		private bool OnTargetValid(IDamageable damageable)
		{
			//Null Check
			if (damageable == null) return false;

			//Range Check
			if (isHasRange)
			{
				if (!Range.IsIn(character.Model.Transform.position, damageable.MarkPoint.transform.position, TargetSkillData.range.range)) return false;
			}
			else
			{
				if (TargetSkillData.range.rangeType == RangeType.None)
				{
					if (damageable != character.Model) return false;
				}
			}

			//Self Check
			if (damageable == character.Model)
			{
				if (!TargetSkillData.isCanTargetSelf) return false;
			}

			return true;
		}

		private void OnTargetChanged(IDamageable damageable)
		{
			//Add
			targets.Add(damageable);

			if (targets.Count == TargetSkillData.targetCount)
			{
				AttackProcess();
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
}