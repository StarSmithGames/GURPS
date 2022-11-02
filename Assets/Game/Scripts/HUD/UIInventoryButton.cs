using Game.Entities;
using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.Systems.SheetSystem;
using Game.UI;
using Game.UI.CanvasSystem;
using Zenject;

namespace Game.HUD
{
	public class UIInventoryButton : UIButton
	{
		private WindowCharacterSheet WindowCharacterSheet
		{
			get
			{
				if(windowCharacterSheet == null)
				{
					windowCharacterSheet = subCanvas.WindowsRegistrator.GetAs<WindowCharacterSheet>();
				}

				return windowCharacterSheet;
			}
		}
		private WindowCharacterSheet windowCharacterSheet;

		private UISubCanvas subCanvas;
		private InputManager inputManager;
		private PartyManager partyManager;

		[Inject]
		private void Construct(UISubCanvas subCanvas, InputManager inputManager, PartyManager partyManager)
		{
			this.subCanvas = subCanvas;
			this.inputManager = inputManager;
			this.partyManager = partyManager;
		}

		private void Start()
		{
			Button.onClick.AddListener(OnClick);
		}

		private void OnDestroy()
		{
			Button?.onClick.RemoveAllListeners();
		}

		private void Update()
		{
			if (inputManager.GetKeyDown(KeyAction.Inventory))
			{
				OnClick();
			}
		}

		private void OnClick()
		{
			ICharacter leader = partyManager.PlayerParty.LeaderParty;

			if (WindowCharacterSheet.IsShowing)
			{
				WindowCharacterSheet.Hide();
			}
			else
			{
				WindowCharacterSheet.SetSheet(leader.Sheet as CharacterSheet);
				WindowCharacterSheet.Show();

				if (leader.Model.IsHasTarget)
				{
					leader.Model.Stop();
				}
			}
		}
	}
}