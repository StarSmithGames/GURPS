using Game.Entities.AI;
using Game.Managers.FactionManager;
using Game.Systems.BattleSystem;
using Game.Systems.CameraSystem;
using Game.Systems.CombatDamageSystem;
using Game.Systems.DialogueSystem;
using Game.Systems.InteractionSystem;
using Game.Systems.SheetSystem;

using Sirenix.OdinInspector;

using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using Zenject;

namespace Game.Entities.Models
{
	public partial class DummyModel : DamageableModel, IAI, ISheetable, ICombatable, IActor, ICameraReporter, IFactionable
	{
		[field: InlineProperty]
		[field: SerializeField] public Faction Faction { get; private set; }
		[field: SerializeField] public CameraPivot CameraPivot { get; private set; }

		public ModelData data;

		public override ISheet Sheet
		{
			get
			{
				if (sheet == null)
				{
					sheet = new ModelSheet(data);
				}

				return sheet;
			}
		}
		private ISheet sheet;


		public Brain Brain { get; private set; }

		[Inject]
		private void Construct(DialogueSystem dialogueSystem)
		{
			this.dialogueSystem = dialogueSystem;

			Brain = new DummyAI(this);
			Brain.StartBrain();
		}
	}

	//IActor implementation
	public partial class DummyModel
	{
		public Transform DialogueTransform => transform;

		public ActorSettings Actor => Sheet.Settings.actor;
		[field: SerializeField] public Barker Barker { get; private set; }


		public bool IsHasSomethingToSay => Actor.barks != null || Actor.dialogues != null;
		public bool IsHasImportantToSay => (Actor.barks != null && IsHasFreshAndImportantBarks()) || (Actor.dialogues != null && IsHasFreshAndImportantDialogues());
		public bool IsInDialogue { get; set; }

		public BarkTree barksInBattle;
		public BarkTree barksTakeDamage;

		private DialogueSystem dialogueSystem;

		//Dummy can't start dialogue
		public bool TalkWith(IActor actor) => false;

		public ISheet GetSheet()
		{
			return Sheet;
		}

		public void Bark(BarkTree barkTree)
		{

		}

		private void Bark(I2StatementNode bark)
		{
			Assert.IsNotNull(bark, "Bark In Battle == null");

			ShowBarkSubtitles(bark.statement.GetCurrent());
		}

		private void ShowBarkSubtitles(I2AudioText subtitles)
		{
			if (subtitles != null)
			{
				Barker.Text.text = subtitles.Text;
				Barker.Show();
			}
		}

		private bool IsHasFreshAndImportantBarks()
		{
			return Actor.barks.TreeData.isFirstTime && Actor.isImportanatBark;
		}
		private bool IsHasFreshAndImportantDialogues()
		{
			return Actor.dialogues.TreeData.isFirstTime;
		}
	}

	//IBattlable implementation
	public partial class DummyModel
	{
		public bool InBattle => CurrentBattle != null;
		public bool InAction { get; }
		public BattleExecutor CurrentBattle { get; private set; }

		public event UnityAction onBattleChanged;

		public bool JoinBattle(BattleExecutor battle)
		{
			if(CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
				CurrentBattle.onBattleOrderChanged -= OnBattleOrderChanged;
			}

			CurrentBattle = battle;

			CurrentBattle.onBattleStateChanged += OnBattleStateChanged;
			CurrentBattle.onBattleOrderChanged += OnBattleOrderChanged;

			return true;
		}

		public bool LeaveBattle()
		{
			if (CurrentBattle != null)
			{
				CurrentBattle.onBattleStateChanged -= OnBattleStateChanged;
			}

			CurrentBattle = null;
			return true;
		}

		private void OnBattleStateChanged(BattleExecutorState oldState, BattleExecutorState newState)
		{
			if(newState == BattleExecutorState.PreBattle)
			{
				Bark(barksInBattle.allNodes.FirstOrDefault() as I2StatementNode);
			}

			if (newState == BattleExecutorState.EndBattle)
			{
				Bark(barksInBattle.allNodes.LastOrDefault() as I2StatementNode);
			}
		}

		private void OnBattleOrderChanged(BattleOrder order)
		{
			if(order == BattleOrder.Turn)
			{
				if(CurrentBattle.CurrentInitiator as Object == this)
				{
					Bark(barksInBattle.allNodes.RandomItem(1, barksInBattle.allNodes.Count - 1) as I2StatementNode);
				}
			}
		}
	}

	//ICombatable implementation
	public partial class DummyModel
	{
		[field: Space]
		[field: SerializeField] public override InteractionPoint BattlePoint { get; protected set; }
		[field: SerializeField] public InteractionPoint OpportunityPoint { get; protected set; }

		public bool CombatWith(IDamageable damageable)
		{
			return false;
		}
	}
}