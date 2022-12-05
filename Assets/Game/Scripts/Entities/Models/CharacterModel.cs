using EPOOutline;

using Game.Systems;
using Game.Systems.AnimatorController;
using Game.Systems.BattleSystem;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.DialogueSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.InventorySystem;
using Game.Systems.SheetSystem;
using Game.Systems.SheetSystem.Abilities;
using Game.Systems.SheetSystem.Skills;

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Entities.Models
{
	public interface ICharacterModel : IEntityModel, ISheetable, ICombatable, IActor, ICameraReporter
	{
		bool IsWithRangedWeapon { get; }//rm
		float CharacterRange { get; }//rm

		ICharacter Character { get; }

		ActiveSkillsRegistrator ActiveSkillsRegistrator { get; }
		AnimatorController AnimatorController { get; }
		CharacterOutfit Outfit { get; }
		Markers Markers { get; }
		Outlinable Outline { get; }

		bool IsCanBattleMove { get; }

		CharacterModel.Data GetData();
	}

	public abstract partial class CharacterModel : EntityModel, ICharacterModel
	{
		public bool InAction => AnimatorController.IsAnimationProcess || IsHasTarget;

		public float CharacterRange => /*equipment.WeaponCurrent.Main.CurrentItem?.GetItemData<WeaponItemData>().weaponRange ??*/ 0f;
		public bool IsWithRangedWeapon { get; private set; }

		public virtual ICharacter Character { get; protected set; }
		public ISheet Sheet => Character.Sheet;

		public ActiveSkillsRegistrator ActiveSkillsRegistrator
		{
			get
			{
				if(activeSkillsRegistrator == null)
				{
					activeSkillsRegistrator = new ActiveSkillsRegistrator();
				}

				return activeSkillsRegistrator;
			}
		}
		private ActiveSkillsRegistrator activeSkillsRegistrator;

		public CharacterOutfit Outfit { get; private set; }
		public AnimatorController AnimatorController { get; private set; }
		public CameraPivot CameraPivot { get; private set; }
		public MarkPoint MarkPoint { get; protected set; }

		public Transform DialogueTransform => Transform;//rm


		[Inject]
		private void Construct(
			CharacterOutfit outfit,
			AnimatorController animatorControl,
			Outlinable outline,
			Markers markerController,
			DialogueSystem dialogueSystem,
			Barker barker,
			CameraPivot cameraPivot,
			MarkPoint markPoint,
			CombatFactory combatFactory)
		{
			Outfit = outfit;
			AnimatorController = animatorControl;
			Outline = outline;
			Markers = markerController;
			CameraPivot = cameraPivot;
			MarkPoint = markPoint;

			this.dialogueSystem = dialogueSystem;
			this.barker = barker;
			this.combatFactory = combatFactory;
		}

		protected override IEnumerator Start()
		{
			Outline.enabled = false;
			Markers.Reset();

			InitializePersonality();
			AnimatorController.Initialize();
			
			yield return null;

			//equipment = (Sheet as CharacterSheet).Equipment;

			Controller.onReachedDestination += OnReachedDestination;
			//equipment.WeaponCurrent.onEquipWeaponChanged += OnEquipWeaponChanged;

			signalBus?.Subscribe<SignalStartDialogue>(OnDialogueStarted);
			signalBus?.Subscribe<SignalEndDialogue>(OnDialogueEnded);

			//yield return new WaitForSeconds(2.5f);
			//CheckReplicas();
		}

		protected override void OnDestroy()
		{
			signalBus?.Unsubscribe<SignalStartDialogue>(OnDialogueStarted);
			signalBus?.Unsubscribe<SignalEndDialogue>(OnDialogueEnded);

			Controller.onReachedDestination -= OnReachedDestination;

			base.OnDestroy();

			//if (equipment != null)
			//{
			//	equipment.WeaponCurrent.onEquipWeaponChanged -= OnEquipWeaponChanged;
			//}
		}

		private void Update()
		{
			Markers.TargetDecal.transform.position = Navigation.CurrentNavMeshDestination;

			if (InBattle)
			{
				BattleTick();
			}
		}

		protected virtual void InitializePersonality()
		{
			//Character = new Character(this, data);
		}

		public override void SetDestination(Vector3 destination, float maxPathDistance = -1)
		{
			if (!InBattle)
			{
				base.SetDestination(destination, maxPathDistance);

				//Fade-In TargetMarker
				if (Controller.IsHasTarget)
				{
					if (!Markers.TargetDecal.IsEnabled)
					{
						Markers.TargetDecal.EnableIn();
					}
				}
			}
			else
			{
				SetDestinationInBattle(destination, maxPathDistance);
			}
		}

		private void OnReachedDestination()
		{
			if (!InBattle)
			{
				//Fade-Out TargetMarker
				if (Markers.TargetDecal.IsEnabled)
				{
					Markers.TargetDecal.EnableOut();
				}
			}
			else
			{
				OnReachedDestinationInBattle();
			}
		}

		private void OnEquipWeaponChanged()
		{
			//IsWithRangedWeapon = equipment.WeaponCurrent.Main.CurrentItem?.IsRangedWeapon ?? false;
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

		public bool IsLineAnimationProcess => LineAnimationCoroutine != null;
		private Coroutine LineAnimationCoroutine = null;

		private IEnumerator LineAnimation()
		{
			while (Navigation.NavMeshInvertedPercentRemainingDistance < 0.99f)
			{
				var path = Navigation.CurrentPath.Path;
				var point = Navigation.FindPointAlongPath(Navigation.CurrentPath, Navigation.NavMeshInvertedPercentRemainingDistance);

				float minDistance = float.MaxValue;
				int index = -1;
				for (int i = 0; i < path.Count; i++)
				{
					var distance = Vector3.Distance(point, path[i]);
					if (distance < minDistance)
					{
						minDistance = distance;
						index = i;
					}
				}

				if (index != -1)
				{
					path[index] = point;

					if (index > 0)
					{
						for (int i = 0; i < index; i++)
						{
							path.Remove(path[i]);
						}
					}
				}

				Markers.LineMarker.DrawLine(path.ToArray());
				yield return null;
			}

			LineAnimationCoroutine = null;
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

		public virtual bool IsHasSomethingToSay => Actor.barks != null || Actor.dialogues != null;
		public virtual bool IsHasImportantToSay => (Actor.barks != null && IsHasFreshAndImportantBarks()) || (Actor.dialogues != null && IsHasFreshAndImportantDialogues());
		public virtual bool IsInDialogue { get; set; }

		public bool IsBarksInWorld => barksInWorld != null;
		private Coroutine barksInWorld = null;


		protected DialogueSystem dialogueSystem;
		protected Barker barker;
	
		public virtual bool TalkWith(IActor actor)
		{
			if (actor.IsHasSomethingToSay)
			{
				if (actor is IInteractable interactable)
				{
					Stop();

					if (interactable.InteractionPoint.IsInRange(Transform.position))
					{
						dialogueSystem.StartDialogue(this, actor);
					}
					else
					{
						new GoToPointAction(interactable.InteractionPoint, () => dialogueSystem.StartDialogue(this, actor)).Execute(this);
					}
				}
				else
				{
					dialogueSystem.StartDialogue(this, actor);
				}

				return true;
			}

			return false;
		}

		public virtual void Bark(BarkTree barkTree)
		{
			switch (barkTree.barkType)
			{
				case BarkType.First:
				{
					//TODO
					break;
				}
				case BarkType.Random:
				{
					var bark = barkTree.allNodes.RandomItem();

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

			barkTree.TreeData.isFirstTime = false;
			//Markers.Exclamation.Hide();
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
			RefreshMarkers();

			if (Actor.useBarks)
			{
				if (Actor.useBarksInWorld && Actor.barksInWorld != null)
				{
					if (IsBarksInWorld)
					{
						StopCoroutine(barksInWorld);
						barksInWorld = null;
					}
					barksInWorld = StartCoroutine(BarksInWorld());
				}
			}
		}

		private IEnumerator BarksInWorld()
		{
			while (true)
			{
				while (IsInDialogue)
				{
					yield return null;
				}

				yield return new WaitWhile(() => barker.IsShowing);

				while (IsInDialogue)
				{
					yield return null;
				}

				yield return new WaitForSeconds(1f);
				Bark(Actor.barksInWorld);
				yield return null;
			}
		}

		private void RefreshMarkers()
		{
			if (IsHasImportantToSay)
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
				IsInDialogue = true;
			}
		}
		private void OnDialogueEnded(SignalEndDialogue signal)
		{
			if (signal.dialogue == Actor.dialogues)
			{
				IsInDialogue = false;
			}
		}
	}

	//IBattlable, Battle & Animations implementation
	partial class CharacterModel
	{
		public bool InBattle => CurrentBattle != null;
		public BattleExecutor CurrentBattle { get; private set; }

		public bool IsCanBattleMove => Sheet.Stats.Move.CurrentValue >= 0.1f;
		private bool IsMoveAvailable => InBattle && isMineTurn && IsCanBattleMove;

		private bool isMineTurn = false;

		protected virtual void BattleTick()
		{
			if (isMineTurn)
			{
				if (IsHasTarget)
				{
					//if (!IsLineAnimationProcess)
					//{
					//	LineAnimationCoroutine = StartCoroutine(LineAnimation());
					//}
				}
				else
				{
					Markers.LineMarker.DrawLine(Navigation.CurrentPath.Path.ToArray());
				}
			}
		}

		public virtual bool JoinBattle(BattleExecutor battle)
		{
			BattleUnsubscribe();

			CurrentBattle = battle;

			BattleSubscribe();

			signalBus?.Fire(new SignalJoinBattleLocal());

			return true;
		}

		public virtual bool LeaveBattle()
		{
			BattleUnsubscribe();
			CurrentBattle = null;

			signalBus?.Fire(new SignalLeaveBattleLocal());

			return true;
		}

		
		private void SetDestinationInBattle(Vector3 destination, float maxPathDistance)
		{
			base.SetDestination(destination, maxPathDistance);

			Markers.LineMarker.DrawLine(Navigation.CurrentPath.Path.ToArray());//redraw last choice destination
			Markers.LineMarker.SetMaterialSpeed(0);//stop line
		}

		private void OnReachedDestinationInBattle()
		{
			Markers.LineMarker.EnableOut(() =>
			{
				Markers.LineMarker.Clear();
				Markers.LineMarker.SetMaterialSpeedToDefault();

				if (IsMoveAvailable)
				{
					Markers.LineMarker.EnableIn();
				}
			});
		}

		private void OnBattleStateChanged(BattleExecutorState oldState, BattleExecutorState newState)
		{
			if(newState == BattleExecutorState.PreBattle)
			{
				Markers.FollowDecal.Enable(true);
				Markers.TargetDecal.Enable(false);
				Markers.AreaDecal.Enable(false);
			}
			else if(newState == BattleExecutorState.EndBattle)
			{
				Markers.Reset();
			}
		}

		private void OnBattleOrderChanged(BattleOrder order)
		{
			if (order == BattleOrder.Turn)//update turn
			{
				isMineTurn = CurrentBattle.CurrentInitiator as Object == this;

				if (IsMoveAvailable)
				{
					Markers.LineMarker.EnableIn();
				}
				else
				{
					if (Markers.LineMarker.IsEnabled)
					{
						Markers.LineMarker.EnableOut();
					}
				}

				Markers.TargetDecal.Enable(IsMoveAvailable);
			}
		}
	
		private void BattleSubscribe()
		{
			CurrentBattle.onBattleOrderChanged += OnBattleOrderChanged;
			CurrentBattle.onBattleStateChanged += OnBattleStateChanged;
		}

		private void BattleUnsubscribe()
		{
			if (CurrentBattle != null)
			{
				CurrentBattle.onBattleOrderChanged -= OnBattleOrderChanged;
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
			}
		}
	}

	//ICombatable implementation
	partial class CharacterModel
	{
		[field: SerializeField] public Vector3 DamagePosition { get; private set; }
		[field: SerializeField] public InteractionPoint BattlePoint { get; private set; }
		[field: SerializeField] public InteractionPoint OpportunityPoint { get; private set; }

		protected CombatFactory combatFactory;
		protected ICombat currentCombat;

		public bool CombatWith(IDamageable damageable)
		{
			if (InBattle)
			{
				bool isCanReach = (Sheet.Stats.Move.CurrentValue - Navigation.FullPath.Distance) >= 0 && Sheet.Stats.Move.CurrentValue != 0;

				if (isCanReach)
				{
					Combat(damageable.BattlePoint.IsInRange(Transform.position));
				}
			}
			else
			{
				Combat(damageable.BattlePoint.IsInRange(Transform.position));
			}

			return true;

			void Combat(bool isInRange = false)
			{
				currentCombat = combatFactory.Create(this, damageable);

				if (!isInRange)
				{
					//Move
					TaskSequence
						.Append(new GoToTaskAction(this, damageable.BattlePoint.GetIteractionPosition(this)));
				}
				//Rotate
				TaskSequence
					.Append(new RotateToTaskAction(this, damageable.Transform));

				//Attack
				TaskSequence
					.Append(currentCombat.AttackAnimation)
					.Append(new TaskWaitAttack(AnimatorController))
					.Execute();
			}
		}

		public virtual Damage GetDamage()
		{
			return new Damage()
			{
				owner = this,
				amount = GetDamageFromTable(),
				damageType = DamageType.Crushing,
			};
		}

		public virtual void Die()
		{
			Controller.Enable(false);
			AnimatorController.Death();
		}

		protected Vector2 GetDamageFromTable()
		{
			return new Vector2(1, 7);
		}
	}
}