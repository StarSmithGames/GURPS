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
			windowCharacterSheet = subCanvas.WindowsRegistrator.GetAs<WindowCharacterSheet>();

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
			if (windowCharacterSheet.IsShowing)
			{
				windowCharacterSheet.Hide();
			}
			else
			{
				windowCharacterSheet.SetSheet(partyManager.PlayerParty.LeaderParty.Sheet as CharacterSheet);
				windowCharacterSheet.Show();
			}
		}
	}
}