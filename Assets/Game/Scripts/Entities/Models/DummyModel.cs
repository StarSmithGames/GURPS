using Game.Entities.AI;
using Game.Managers.FactionManager;
using Game.Systems.BattleSystem;
using Game.Systems.CameraSystem;
using Game.Systems.DialogueSystem;
using Game.Systems.SheetSystem;

using Sirenix.OdinInspector;

using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using Zenject;

namespace Game.Entities.Models
{
	public partial class DummyModel : Model, IAI, ISheetable, IActor, IBattlable, ICameraReporter, IFactionable
	{
		[field: InlineProperty]
		[field: SerializeField] public Faction Faction { get; private set; }
		[field: SerializeField] public CameraPivot CameraPivot { get; private set; }

		public ModelData data;

		public ISheet Sheet
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


		private DialogueSystem dialogueSystem;

		//Dummy can't start dialogue
		public bool TalkWith(IActor actor) => false;

		public void Bark(BarkTree barkTree)
		{

		}

		public void BarkInBattle()
		{
			var bark = barksInBattle.TreeData.isFirstTime?
				barksInBattle.allNodes.FirstOrDefault() :
				barksInBattle.allNodes.RandomItem(1);

			Assert.IsNotNull(bark, "Bark In Battle == null");

			if (bark is I2StatementNode node)
			{
				ShowBarkSubtitles(node.statement.GetCurrent());
			}

			barksInBattle.TreeData.isFirstTime = false;
		}

		public ISheet GetSheet()
		{
			return Sheet;
		}

		protected void ShowBarkSubtitles(I2AudioText subtitles)
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
			CurrentBattle = battle;

			return true;
		}

		public bool LeaveBattle()
		{
			CurrentBattle = null;
			return true;
		}
	}
}