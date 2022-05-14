using EPOOutline;
using Game.Systems.CameraSystem;
using Game.Systems.DamageSystem;
using Game.Systems.DialogueSystem;
using Game.Systems.FloatingTextSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using NodeCanvas.DialogueTrees;

using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using DG.Tweening;

using Zenject;
using System.Collections;
using Sirenix.OdinInspector;

namespace Game.Entities
{
	public abstract partial class Entity : InteractableModel, IEntity, IActor
	{
		public MonoBehaviour MonoBehaviour => this;

		public virtual ISheet Sheet { get; private set; }

		public AnimatorControl AnimatorControl { get; private set; }

		public Markers Markers { get; private set; }
		public Outlinable Outlines { get; private set; }

		public CameraPivot CameraPivot { get; private set; }

		public TaskSequence TaskSequence { get; private set; }

		protected SignalBus signalBus;
		protected UIManager uiManager;

		[Inject]
		private void Construct(
			SignalBus signalBus,
			AnimatorControl animatorControl,
			NavigationController navigationController,
			CharacterController3D controller,
			Markers markerController,
			Outlinable outline,
			CameraPivot cameraPivot,
			UIManager uiManager,
			FloatingSystem floatingTextSystem,
			DialogueSystem dialogueSystem,
			Barker barker)
		{
			this.signalBus = signalBus;

			AnimatorControl = animatorControl;
			Navigation = navigationController;
			Controller = controller;
			Markers = markerController;
			Outlines = outline;
			CameraPivot = cameraPivot;
			this.uiManager = uiManager;
			this.floatingSystem = floatingTextSystem;

			this.dialogueSystem = dialogueSystem;
			this.barker = barker;

			TaskSequence = new TaskSequence(this);

			Validate();
		}

		protected virtual void OnDestroy()
		{
		}

		protected virtual IEnumerator Start()
		{
			Outlines.enabled = false;

			ResetMarkers();

			Markers.Exclamation.Enable(false);
			Markers.Question.Enable(false);

			yield return new WaitForSeconds(2.5f);
			CheckReplicas();
		}

		public void Freeze(bool trigger)
		{
			Controller.Freeze(trigger);
		}

		#region Observe
		public override void StartObserve()
		{
			base.StartObserve();
			uiManager.Battle.SetSheet(Sheet);
		}
		public override void EndObserve()
		{
			base.EndObserve();
			uiManager.Battle.SetSheet(null);
		}
		#endregion

		protected virtual void ResetMarkers()
		{
			Markers.FollowMarker.Enable(false);

			Markers.TargetMarker.transform.parent = null;
			Markers.TargetMarker.Enable(false);

			Markers.AreaMarker.Enable(false);

			Markers.LineMarker.Enable(false);
		}


		private void Validate()
		{
			Assert.IsNotNull(Navigation, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(Controller, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(Markers, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(Outlines, $"Entity {gameObject.name} lost component.");
			Assert.IsNotNull(CameraPivot, $"Entity {gameObject.name} lost component.");
		}
	}

	//IPathfinderable implementation
	partial class Entity
	{
		public event UnityAction onTargetChanged;
		public event UnityAction onDestinationChanged;

		public Transform Transform => transform;

		public bool IsHasTarget => Controller.IsHasTarget;

		public NavigationController Navigation { get; private set; }
		public CharacterController3D Controller { get; private set; }

		public virtual void SetTarget(Vector3 point, float maxPathDistance = -1)
		{
			Navigation.SetTarget(point, maxPathDistance: maxPathDistance);
			onTargetChanged?.Invoke();
		}

		public virtual void SetDestination(Vector3 destination, float maxPathDistance = -1)
		{
			Controller.SetDestination(destination, maxPathDistance: maxPathDistance);
			onDestinationChanged?.Invoke();
		}

		public virtual void Stop()
		{
			if (IsHasTarget)
			{
				Controller.Stop();
			}
		}
	}

	//IDamegeable, IKillable implementation
	partial class Entity
	{
		public event UnityAction<IEntity> onDied;

		protected FloatingSystem floatingSystem;


		public void Kill()
		{
			Controller.Enable(false);
			onDied?.Invoke(this);
		}

		public virtual Damage GetDamage()
		{
			return new Damage()
			{
				amount = GetDamageFromTable(),
				damageType = DamageType.Crushing,
			};
		}

		public virtual void ApplyDamage<T>(T value)
		{
			if (value is Damage damage)
			{
				float dmg = (int)Mathf.Max(damage.DMG - 2, 0);

				if(dmg == 0)
				{
					floatingSystem.CreateText(transform.TransformPoint(CameraPivot.settings.startPosition), "Miss!", type: AnimationType.BasicDamageType);
				}
				else
				{
					floatingSystem.CreateText(transform.TransformPoint(CameraPivot.settings.startPosition), damage.damageType.ToString(), type: AnimationType.BasicDamageType);

					if (damage.IsPhysicalDamage)
					{

						floatingSystem.CreateText(transform.TransformPoint(CameraPivot.settings.startPosition), dmg.ToString(), type: AnimationType.AdvanceDamage);
						if (!Sheet.Settings.isImmortal)
						{
							Sheet.Stats.HitPoints.CurrentValue -= dmg;
						}
					}
					else if (damage.IsMagicalDamage)
					{

					}
				}
			}
		}

		private Vector2 GetDamageFromTable()
		{
			return new Vector2(1, 7);
		}
	}
	
	//IActor implementation
	partial class Entity
	{
		public virtual bool IsHaveSomethingToSay => ActorSettings.barks != null;

		public ActorSettings ActorSettings => actorSettings;
		[SerializeField] protected ActorSettings actorSettings;

		protected DialogueSystem dialogueSystem;
		protected Barker barker;

		[Button]
		public virtual void Bark()
		{
			if (barker == null) return;
			if (barker.IsShowing) return;

			var bark = ActorSettings.barks.allNodes.FirstOrDefault();

			switch (ActorSettings.barks.barkType)
			{
				case BarkType.First:
				case BarkType.Random:
				{
					bark = ActorSettings.barks.allNodes.RandomItem();

					Statement subtitles = null;
					if (bark is I2StatementNode node)
					{
						subtitles = node.GetSubtitles();
					}

					ShowBarkSubtitles(subtitles);

					break;
				}
				case BarkType.Sequence:
				{
					//TODO
					break;
				}
			}

			Markers.Exclamation.Hide();
		}

		private void ShowBarkSubtitles(Statement subtitles)
		{
			if (subtitles != null)
			{
				barker.Text.text = subtitles.text;
				barker.Show();
			}
		}
	
		protected virtual void CheckReplicas()
		{
			if (IsHaveSomethingToSay)
			{
				Markers.Exclamation.Show();
			}
			else
			{
				if (Markers.Exclamation.IsSwowing && !Markers.Exclamation.IsHideProcess)
				{
					Markers.Exclamation.Hide();
				}
			}
		}
	}
}