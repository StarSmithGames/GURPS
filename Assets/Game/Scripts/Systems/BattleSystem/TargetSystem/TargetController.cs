using Cinemachine;

using EPOOutline;

using Game.Entities;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.CursorSystem;
using Game.Systems.NavigationSystem;
using Game.Systems.SheetSystem.Skills;
using Game.Systems.TooltipSystem;
using Game.Systems.VFX;

using Sirenix.OdinInspector;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.BattleSystem.TargetSystem
{
	public class TargetController
	{
		public UnityAction<IDamageable> onTargetChanged;
		public Func<IDamageable, bool> onTargetValid;

		public Vector3 LastWorldPosition { get; private set; }
		public IDamageable CurrentTarget { get; private set; }
		public IDamageable LastTarget { get; private set; }

		private List<IDamageable> targets = new List<IDamageable>();

		private ICharacter character;
		private ActiveTargetSkillData data;

		private Plane plane = new Plane(Vector3.up, 0);
		private List<LineTargetVFX> lines = new List<LineTargetVFX>();
		private List<RadialAreaDecalVFX> areas = new List<RadialAreaDecalVFX>();
		private LineTargetVFX currentLine;
		private RadialAreaDecalVFX currentArea;
		private RadialAreaDecalVFX rangeArea;
		private OutlineData targetOutline;
		private NavigationPath path;

		private CinemachineBrain brain;
		private CameraVisionLocation cameraVision;
		private PointerVFX pointer;
		private LineTargetVFX.Factory lineTargetFactory;
		private RadialAreaDecalVFX.Factory areaFactory;
		private TooltipSystem.TooltipSystem tooltipSystem;
		private CursorSystem.CursorSystem cursorSystem;

		public TargetController(
			CinemachineBrain brain,
			CameraVisionLocation cameraVision,
			PointerVFX pointer,
			LineTargetVFX.Factory lineTargetFactory,
			RadialAreaDecalVFX.Factory areaFactory,
			TooltipSystem.TooltipSystem tooltipSystem,
			CursorSystem.CursorSystem cursorSystem)
		{
			this.brain = brain;
			this.cameraVision = cameraVision;
			this.pointer = pointer;
			this.lineTargetFactory = lineTargetFactory;
			this.areaFactory = areaFactory;
			this.tooltipSystem = tooltipSystem;
			this.cursorSystem = cursorSystem;
		}

		public void Begin(ICharacter character, ActiveTargetSkillData data)
		{
			this.character = character;
			this.data = data;

			path = new NavigationPath();
			targetOutline = GlobalDatabase.Instance.allOutlines.Find((x) => x.outlineType == OutlineType.Target);

			cursorSystem.SetCursor(CursorType.Base);
			cameraVision.IsCanMouseClick = false;

			//Visual
			CreatePropsForTarget();
			UpdateTooltipPath();
			UpdateTooltipTargets();
		}

		public void End()
		{
			cursorSystem.SetCursor(CursorType.Hand);
			cameraVision.IsCanMouseClick = true;
			tooltipSystem.SetRuller(TooltipRulerType.None);
			tooltipSystem.SetMessage(TooltipMessageType.None);
			tooltipSystem.SetMessage("", TooltipAdditionalMessageType.None);

			CurrentTarget = null;
			LastTarget = null;
			targets.Clear();

			lines.Clear();
			areas.Clear();
			currentArea = null;
			rangeArea = null;
		}

		public void Tick()
		{
			//RayCast
			float distance;
			Ray ray = brain.OutputCamera.ScreenPointToRay(Input.mousePosition);
			if (plane.Raycast(ray, out distance))
			{
				LastWorldPosition = ray.GetPoint(distance);
			}

			SetTarget(cameraVision.CurrentObserve as IDamageable);

			DrawProps();

			UpdateTooltipRange();
		}

		public void AddTarget(IDamageable damageable)
		{
			if (damageable == null) return;
			if (targets.Count == data.targetCount) return;

			currentLine?.SetState(LineTargetState.Targeted);
			currentArea?.FadeTo(0.25f);
			UpdateTooltipTargets();

			targets.Add(damageable);

			if (targets.Count != data.targetCount)
			{
				CreatePropsForTarget();
			}

			onTargetChanged?.Invoke(damageable);
		}

		private void SetTarget(IDamageable damageable)
		{
			if (CurrentTarget != null)
			{
				CurrentTarget.Outline.ResetData();
			}

			LastTarget = CurrentTarget;
			CurrentTarget = (onTargetValid?.Invoke(damageable) ?? true) ? InnerValidTarget(damageable) : null;

			if (CurrentTarget != LastTarget)
			{
				if (damageable == null)
				{
					UpdateTooltipPath();
				}
				else
				{
					damageable.Outline.SetData(targetOutline);
					tooltipSystem.SetRullerChance("100%");
				}
			}

			ClampTarget();
		}

		private void ClampTarget()
		{
			if (CurrentTarget != null && data.isCanClampOnTarget)
			{
				LastWorldPosition = CurrentTarget.MarkPoint.transform.position;
			}
		}

		private IDamageable InnerValidTarget(IDamageable damageable)
		{
			//Null Check
			if (damageable == null) return null;

			//Range Check
			if (data.range.IsHasRange)
			{
				if (!Range.IsIn(character.Model.MarkPoint.transform.position, damageable.MarkPoint.transform.position, data.range.range)) return null;
			}
			else
			{
				if (data.range.rangeType == RangeType.None)
				{
					if (damageable != character.Model) return null;
				}
			}

			//Self Check
			if (damageable == character.Model)
			{
				if (!data.isCanTargetSelf) return null;
			}

			return damageable;
		}


		private void DrawProps()
		{
			pointer.SetPosition(LastWorldPosition);

			path.SetPath(new Vector3[] { character.Model.MarkPoint.transform.position, LastWorldPosition });

			if (data.path.pathType == PathType.Line)
			{
				currentLine?.DrawLine(path.Path.ToArray());//TargetSkillData.path.drawPath
			}
			else if (data.path.pathType == PathType.Ballistic)
			{
				currentLine?.DrawLine(KinematicBallistic.GetTraectory(character.Model.MarkPoint.transform.position, LastWorldPosition));
			}

			currentArea?.DrawDecal(LastWorldPosition);//TargetSkillData.isAoE
		}

		public void FadeOutProps()
		{
			pointer.Enable(false);
			for (int i = 0; i < lines.Count; i++)
			{
				lines[i].FadeOut();
			}
			if (data.isAoE)
			{
				for (int i = 0; i < areas.Count; i++)
				{
					areas[i].FadeTo(0);
				}
			}
			if (data.range.IsHasRange)
			{
				rangeArea.FadeTo(0);
			}
		}

		private void CreatePropsForTarget()
		{
			pointer.Enable(true);

			if (data.path.drawPath)
			{
				//Line
				currentLine = lineTargetFactory.Create();
				currentLine.SetState(LineTargetState.Target);
				lines.Add(currentLine);
			}
			if (data.isAoE)
			{
				//Area
				currentArea = areaFactory.Create();
				currentArea.SetRange(data.AoE.range);
				currentArea.SetFade(0).FadeTo(0.8f);
				areas.Add(currentArea);
			}

			if (data.range.IsHasRange && rangeArea == null)
			{
				rangeArea = areaFactory.Create();
				rangeArea.SetRange(data.range.range);
				rangeArea.SetFade(0).FadeTo(0.8f);
				rangeArea.DrawDecal(character.Model.Transform.position);
			}
		}

		private void UpdateTooltipRange()
		{
			if (data.range.IsHasRange)
			{
				if (!Range.IsIn(character.Model.Transform.position, LastWorldPosition, data.range.range))
				{
					tooltipSystem.SetMessage(TooltipMessageType.OutOfRange);
				}
				else
				{
					tooltipSystem.SetMessage(TooltipMessageType.None);
				}
			}
		}

		private void UpdateTooltipPath()
		{
			tooltipSystem.SetRullerPath(path);
			tooltipSystem.SetRuller(TooltipRulerType.CustomPath);
		}

		private void UpdateTooltipTargets()
		{
			if (data.targetCount > 1)
			{
				tooltipSystem.SetMessage($"{lines.Count}/{data.targetCount}", TooltipAdditionalMessageType.Projectiles);
			}
		}
	}

	[System.Serializable]
	public class TargetRange
	{
		public RangeType rangeType = RangeType.None;

		[Min(0)]
		[SuffixLabel("m", true)]
		[ShowIf("rangeType", RangeType.Custom)]
		public float range;

		public bool IsHasRange => rangeType == RangeType.Custom;
	}

	[System.Serializable]
	public class TargetPath
	{
		public bool drawPath = true;
		public PathType pathType = PathType.Line;
	}

	[System.Serializable]
	public class TargetAoE
	{
		[LabelText("AoE Type")]
		public AoEType AoEType = AoEType.Circle;
		[Min(1)]
		[SuffixLabel("m", true)]
		public float range = 1;
	}

	public enum RangeType
	{
		None,
		Max,
		Custom,
	}

	public enum AoEType
	{
		Circle,
	}

	public enum PathType
	{
		Line,
		Ballistic,
		Custom
	}
}