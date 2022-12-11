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

namespace Game.Systems.SheetSystem.Skills
{
    public abstract partial class TargetSkill : ActiveSkill
    {
		protected ActiveTargetSkillData TargetSkillData => Data as ActiveTargetSkillData;

		protected List<IDamageable> targets = new List<IDamageable>();

		protected Dictionary<ProjectileVFX, IDamageable> projectilesWithTargets = new Dictionary<ProjectileVFX, IDamageable>();
		protected int projectileCompletedCount = 0;

		private TargetController targetController;
		private CombatFactory combatFactory;

		[Inject]
		private void Construct(TargetController targetController, CombatFactory combatFactory)
		{
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
					targetController.AddTarget(targetController.CurrentTarget);
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
						.SetStart(character.Model.MarkPoint.transform.position, character.Model.MarkPoint.transform.forward)
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
				targetController.onTargetChanged -= OnTargetChanged;
				targetController.onTargetValid -= OnTargetValid;
				targetController.FadeOutProps();
				
				character.Model.Freeze(false);

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
			targets.Clear();

			targetController.End();
		}

		private bool OnTargetValid(IDamageable damageable)
		{
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