using EPOOutline;

using Game.Systems.BattleSystem;
using Game.Systems.DamageSystem;
using Game.Systems.DialogueSystem;
using Game.Systems.FloatingTextSystem;
using Game.Systems.InteractionSystem;

using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

using Zenject;

namespace Game.Entities
{
	public partial class StubEntity : Entity, IBattlable, IActor
	{
		public event UnityAction onBattleChanged;

		public bool InBattle => CurrentBattle != null;
		public bool InAction => AnimatorControl.IsAnimationProcess || IsHasTarget;

		public TaskSequence TaskSequence { get; private set; }
		public AnimatorControl AnimatorControl { get; private set; }
		public Markers Markers { get; private set; }
		public Outlinable Outlines { get; private set; }
		public Battle CurrentBattle { get; private set; }

		[Inject]
		private void Construct(AnimatorControl animatorControl,
			FloatingSystem floatingTextSystem,
			DialogueSystem dialogueSystem, Barker barker,
			Markers markerController, Outlinable outline)
		{
			AnimatorControl = animatorControl;

			this.floatingSystem = floatingTextSystem;
			this.dialogueSystem = dialogueSystem;
			this.barker = barker;
			Markers = markerController;
			Outlines = outline;


			TaskSequence = new TaskSequence(this);
		}

		protected override IEnumerator Start()
		{
			signalBus?.Subscribe<StartDialogueSignal>(OnDialogueStarted);

			Outlines.enabled = false;

			ResetMarkers();

			Markers.Exclamation.Enable(false);
			Markers.Question.Enable(false);
			
			yield return base.Start();
			yield return new WaitForSeconds(2.5f);
			CheckReplicas();
		}

		protected override void OnDestroy()
		{
			signalBus?.Unsubscribe<StartDialogueSignal>(OnDialogueStarted);
		}

		protected virtual void Update()
		{
			Markers.TargetMarker.transform.position = Navigation.CurrentNavMeshDestination;
			Markers.TargetMarker.DrawCircle();

			Markers.FollowMarker.DrawCircle();

			Markers.LineMarker.DrawLine(Navigation.NavMeshAgent.path.corners);
		}

		protected virtual void ResetMarkers()
		{
			Markers.FollowMarker.Enable(false);

			Markers.TargetMarker.transform.parent = null;
			Markers.TargetMarker.Enable(false);

			Markers.AreaMarker.Enable(false);

			Markers.LineMarker.Enable(false);
		}

		public virtual bool JoinBattle(Battle battle)
		{
			if (CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
				CurrentBattle.onBattleUpdated -= OnBattleUpdated;
			}
			CurrentBattle = battle;
			CurrentBattle.onBattleStateChanged += OnBattleStateChanged;
			CurrentBattle.onBattleUpdated += OnBattleUpdated;

			onBattleChanged?.Invoke();

			return true;
		}

		public virtual bool LeaveBattle()
		{
			if (CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
				CurrentBattle.onBattleUpdated -= OnBattleUpdated;
			}
			CurrentBattle = null;

			onBattleChanged?.Invoke();

			return true;
		}

		public virtual void Attack() { }

		protected virtual void OnBattleStateChanged()
		{
			if (InBattle)
			{
				switch (CurrentBattle.CurrentState)
				{
					case BattleState.PreBattle:
					{
						Markers.FollowMarker.Enable(true);
						break;
					}
					case BattleState.EndBattle:
					{
						Markers.FollowMarker.Enable(false);

						break;
					}
				}
			}
			else
			{
				ResetMarkers();
			}
		}

		protected virtual void OnBattleUpdated() { }
	}

	//IActor implementation
	partial class StubEntity
	{
		public virtual bool IsHaveSomethingToSay => (ActorSettings.barks != null && IsHasFreshAndImportantBarks()) || (ActorSettings.dialogues != null && IsHasFreshAndImportantDialogues());
		public virtual bool IsInDialogue { get; set; }

		public ActorSettings ActorSettings => actorSettings;
		[SerializeField] protected ActorSettings actorSettings;

		protected DialogueSystem dialogueSystem;
		protected Barker barker;

		public virtual void Bark()
		{
			if (barker == null || ActorSettings.barks == null)
			{
				Debug.LogError($"{gameObject.name} barker == null || ActorSettings.barks == null", gameObject);
				return;
			}
			if (barker.IsShowing) return;

			var bark = ActorSettings.barks.allNodes.FirstOrDefault();

			switch (ActorSettings.barks.barkType)
			{
				case BarkType.First:
				case BarkType.Random:
				{
					bark = ActorSettings.barks.allNodes.RandomItem();

					if (bark is I2StatementNode node)
					{
						ShowBarkSubtitles(node.statement.GetCurrent());
					}

					break;
				}
				case BarkType.Sequence:
				{
					//TODO
					break;
				}
			}

			ActorSettings.barks.TreeData.isFirstTime = false;
			//Markers.Exclamation.Hide();
		}


		private bool IsHasFreshAndImportantBarks()
		{
			return ActorSettings.barks.TreeData.isFirstTime && ActorSettings.isImportanatBark;
		}
		private bool IsHasFreshAndImportantDialogues()
		{
			return ActorSettings.dialogues.TreeData.isFirstTime;
		}


		protected void ShowBarkSubtitles(I2AudioText subtitles)
		{
			if (subtitles != null)
			{
				barker.Text.text = subtitles.Text;
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

		private void OnDialogueStarted(StartDialogueSignal signal)
		{
			if (signal.dialogue == actorSettings.dialogues)
			{
				CheckReplicas();
			}
		}
	}

	//IDamegeable, IKillable implementation
	partial class StubEntity
	{
		protected FloatingSystem floatingSystem;

		public override Damage GetDamage()
		{
			return new Damage()
			{
				amount = GetDamageFromTable(),
				damageType = DamageType.Crushing,
			};
		}

		public override void ApplyDamage<T>(T value)
		{
			if (value is Damage damage)
			{
				float dmg = (int)Mathf.Max(damage.DMG - 2, 0);

				if (dmg == 0)
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
	}
}