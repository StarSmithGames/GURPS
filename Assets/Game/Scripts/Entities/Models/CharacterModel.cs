using Game.Systems.BattleSystem;
using Game.Systems.InventorySystem;

using UnityEngine;

using Zenject;
using System.Collections;
using Game.Systems.DialogueSystem;
using System.Linq;
using UnityEngine.Events;
using EPOOutline;
using Game.Systems.InteractionSystem;
using Game.Systems.DamageSystem;
using Game.Systems.FloatingTextSystem;
using Game.Systems.SheetSystem;

namespace Game.Entities.Models
{
	public interface ICharacterModel : IEntityModel, IObservable, IInteractable, IBattlable, IActor, IDamegeable, IKillable
	{
		bool IsWithRangedWeapon { get; }//rm
		float CharacterRange { get; }//rm

		ICharacter Character { get; }

		AnimatorControl AnimatorControl { get; }
		CharacterOutfit Outfit { get; }
		Markers Markers { get; }
		Outlinable Outline { get; }

		CharacterModel.Data GetData();
	}

	public abstract partial class CharacterModel : EntityModel, ICharacterModel
	{
		public bool InAction => AnimatorControl.IsAnimationProcess || IsHasTarget;

		public float CharacterRange => equipment.WeaponCurrent.Main.Item?.GetItemData<WeaponItemData>().weaponRange ?? 0f;
		public bool IsWithRangedWeapon { get; private set; }

		public ICharacter Character { get; protected set; }

		public CharacterOutfit Outfit { get; private set; }
		public AnimatorControl AnimatorControl { get; private set; }

		public Transform DialogueTransform => Transform;

		private IEquipment equipment;

		[Inject]
		private void Construct(
			CharacterOutfit outfit,
			AnimatorControl animatorControl,
			Outlinable outline,
			Markers markerController,
			DialogueSystem dialogueSystem,
			Barker barker,
			FloatingSystem floatingSystem)
		{
			Outfit = outfit;
			AnimatorControl = animatorControl;
			Outline = outline;
			Markers = markerController;

			this.dialogueSystem = dialogueSystem;
			this.barker = barker;
			this.floatingSystem = floatingSystem;
			//equipment = (Sheet as CharacterSheet).Equipment;
		}

		protected override IEnumerator Start()
		{
			InitializePersonality();

			Controller.onReachedDestination += OnReachedDestination;
			//equipment.WeaponCurrent.onEquipWeaponChanged += OnEquipWeaponChanged;

			Outline.enabled = false;

			ResetMarkers();

			Markers.Exclamation.Enable(false);
			Markers.Question.Enable(false);

			signalBus?.Subscribe<SignalStartDialogue>(OnDialogueStarted);

			yield return base.Start();
			yield return new WaitForSeconds(2.5f);
			CheckReplicas();
		}

		protected override void OnDestroy()
		{
			//signalBus?.Unsubscribe<StartDialogueSignal>(OnDialogueStarted);

			base.OnDestroy();

			//if (equipment != null)
			//{
			//	equipment.WeaponCurrent.onEquipWeaponChanged -= OnEquipWeaponChanged;
			//}
		}

		protected virtual void Update()
		{
			Markers.TargetMarker.transform.position = Navigation.CurrentNavMeshDestination;
			Markers.TargetMarker.DrawCircle();

			Markers.FollowMarker.DrawCircle();

			Markers.LineMarker.DrawLine(Navigation.NavMeshAgent.path.corners);
		}

		protected virtual void InitializePersonality()
		{
			//Character = new Character(this, data);
		}

		private void OnEquipWeaponChanged()
		{
			IsWithRangedWeapon = equipment.WeaponCurrent.Main.Item?.IsRangedWeapon ?? false;
		}

		public Data GetData()
		{
			return new Data()
			{
				transform = new DefaultTransform()
				{
					position = transform.position,
					rotation = transform.rotation,
					scale = transform.localScale,
				}
			};
		}

		public struct Data
		{
			public DefaultTransform transform;
		}
	}

	//Visual
	partial class CharacterModel
	{
		public Markers Markers { get; protected set; }

		protected virtual void ResetMarkers()
		{
			Markers.FollowMarker.Enable(false);

			Markers.TargetMarker.transform.parent = null;
			Markers.TargetMarker.Enable(false);

			Markers.AreaMarker.Enable(false);

			Markers.LineMarker.Enable(false);
		}
	}

	//IActor implementation
	partial class CharacterModel
	{
		public ActorSettings Actor
		{
			get
			{
				if(actor == null)
				{
					actor = Character.Sheet.Settings.actor;
				}
				return actor;
			}
		}
		private ActorSettings actor;

		public virtual bool IsHaveSomethingToSay => (Actor.barks != null && IsHasFreshAndImportantBarks()) || (Actor.dialogues != null && IsHasFreshAndImportantDialogues());
		public virtual bool IsInDialogue { get; set; }

		protected DialogueSystem dialogueSystem;
		protected Barker barker;

		public virtual bool TalkWith(IActor actor)
		{
			if (actor.IsHaveSomethingToSay)
			{
				dialogueSystem.StartDialogue(this, actor);
				return true;
			}

			return false;
		}

		public virtual void Bark()
		{
			if (barker == null || Actor.barks == null)
			{
				Debug.LogError($"{gameObject.name} barker == null || ActorSettings.barks == null", gameObject);
				return;
			}
			if (barker.IsShowing) return;

			var bark = Actor.barks.allNodes.FirstOrDefault();

			switch (Actor.barks.barkType)
			{
				case BarkType.First:
				case BarkType.Random:
				{
					bark = Actor.barks.allNodes.RandomItem();

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

			Actor.barks.TreeData.isFirstTime = false;
			Markers.Exclamation.Hide();
		}

		public ISheet GetSheet()
		{
			return Character.Sheet;
		}

		private bool IsHasFreshAndImportantBarks()
		{
			return Actor.barks.TreeData.isFirstTime && Actor.isImportanatBark;
		}
		private bool IsHasFreshAndImportantDialogues()
		{
			return Actor.dialogues.TreeData.isFirstTime;
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

		private void OnDialogueStarted(SignalStartDialogue signal)
		{
			if (signal.dialogue == Actor.dialogues)
			{
				CheckReplicas();
			}
		}
	}

	//IBattlable implementation
	partial class CharacterModel
	{
		public event UnityAction onBattleChanged;

		public bool InBattle => CurrentBattle != null;
		public Battle CurrentBattle { get; private set; }

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
	}

	//Override Battle & Animations implementation
	partial class CharacterModel
	{
		public override void SetTarget(Vector3 point, float maxPathDistance = -1)
		{
			//base.SetTarget(point, InBattle ? Sheet.Stats.Move.CurrentValue : maxPathDistance);
		}

		public override void SetDestination(Vector3 destination, float maxPathDistance = -1)
		{
			base.SetDestination(destination, /*InBattle ? Sheet.Stats.Move.CurrentValue :*/ maxPathDistance);

			if (!InBattle)
			{
				//Fade-In TargetMarker
				if (Controller.IsHasTarget)
				{
					if (!Markers.TargetMarker.IsEnabled)
					{
						Markers.TargetMarker.EnableIn();
					}
				}
			}
		}

		public virtual void Attack() { }


		public override void Stop()
		{
			base.Stop();

			OnReachedDestination();
		}

		private void OnReachedDestination()
		{
			if (!InBattle)
			{
				//Fade-Out TargetMarker
				if (Markers.TargetMarker.IsEnabled)
				{
					Markers.TargetMarker.EnableOut();
				}
			}
		}

		protected virtual void OnBattleStateChanged()
		{
			if (InBattle)
			{
				switch (CurrentBattle.CurrentState)
				{
					case BattleState.PreBattle:
					{
						Markers.FollowMarker.Enable(true);

						Markers.TargetMarker.Enable(false);

						Markers.AreaMarker.Enable(false);

						Markers.LineMarker.Enable(false);
						break;
					}
					case BattleState.Battle:
					{
						Markers.FollowMarker.Enable(true);

						Markers.TargetMarker.Enable(true);

						Markers.AreaMarker.Enable(false);

						Markers.LineMarker.Enable(true);
						break;
					}
					case BattleState.EndBattle:
					{
						Markers.FollowMarker.Enable(false);

						Markers.TargetMarker.Enable(true);

						Markers.AreaMarker.Enable(false);

						Markers.LineMarker.Enable(false);
						break;
					}
				}
			}
			else
			{
				ResetMarkers();
			}
		}
		protected virtual void OnBattleUpdated()
		{
			bool isMineTurn = CurrentBattle.BattleFSM.CurrentTurn.Initiator == this && CurrentBattle.CurrentState != BattleState.EndBattle;

			Markers.LineMarker.Enable(InBattle && isMineTurn);
			Markers.TargetMarker.Enable(InBattle && isMineTurn);
		}
	}

	//IDamegeable, IKillable implementation
	partial class CharacterModel
	{
		public event UnityAction<IEntity> onDied;

		protected FloatingSystem floatingSystem;

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
						if (!Character.Sheet.Settings.isImmortal)
						{
							Character.Sheet.Stats.HitPoints.CurrentValue -= dmg;
						}
					}
					else if (damage.IsMagicalDamage)
					{

					}
				}
			}
		}

		public virtual void Kill()
		{
			Controller.Enable(false);
			onDied?.Invoke(Character);
		}

		protected Vector2 GetDamageFromTable()
		{
			return new Vector2(1, 7);
		}
	}
}