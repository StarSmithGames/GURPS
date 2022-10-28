using Game.Managers.InputManager;
using Game.Managers.PartyManager;
using Game.UI;
using Game.UI.CanvasSystem;
using Zenject;

namespace Game.HUD
{
    public class UISkillDeckButton : UIButton
    {
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

		}
	}
}