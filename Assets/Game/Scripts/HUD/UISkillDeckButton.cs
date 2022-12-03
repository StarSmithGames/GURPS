using Game.Entities;
using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.Systems.SheetSystem;
using Game.UI;
using Game.UI.CanvasSystem;
using Zenject;

namespace Game.HUD
{
    public class UISkillDeckButton : UIButton
    {
		private WindowSkillDeck WindowSkillDeck
		{
			get
			{
				if (windowSkillDeck == null)
				{
					windowSkillDeck = subCanvas.WindowsRegistrator.GetAs<WindowSkillDeck>();
				}

				return windowSkillDeck;
			}
		}
		private WindowSkillDeck windowSkillDeck;

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
			if (inputManager.GetKeyDown(KeyAction.SkillDeck))
			{
				OnClick();
			}
		}

		private void OnClick()
		{
			ICharacter leader = partyManager.PlayerParty.LeaderParty;

			if (WindowSkillDeck.IsShowing)
			{
				WindowSkillDeck.Hide();
			}
			else
			{
				WindowSkillDeck.SetSkills(leader.Skills.SkillDeck);
				WindowSkillDeck.Show();

				if (leader.Model.IsHasTarget)
				{
					leader.Model.Stop();
				}
			}
		}
	}
}