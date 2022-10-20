using Game.Entities;
using Game.Systems.SheetSystem;
using Game.UI;
using Game.UI.CanvasSystem;
using Game.UI.Windows;

using UnityEngine;

using Zenject;

namespace Game.Systems.BattleSystem
{
    public class WindowEntityInformation : WindowBase
    {
        [field: SerializeField] public TMPro.TextMeshProUGUI Name { get; private set; }
        [field: SerializeField] public UIBar HealthBar { get; private set; }
        [field: SerializeField] public TMPro.TextMeshProUGUI Level { get; private set; }

        private ISheet sheet;

        private UISubCanvas subCanvas;

        [Inject]
        private void Construct(UISubCanvas subCanvas)
		{
            this.subCanvas = subCanvas;

        }

		private void Start()
		{
            Enable(false);
            subCanvas.WindowsRegistrator.Registrate(this);
        }

		private void OnDestroy()
		{
            subCanvas.WindowsRegistrator.UnRegistrate(this);
        }

        public void SetSheet(ISheet sheet)
		{
            this.sheet = sheet;

            HealthBar.SetStat(sheet?.Stats.HitPoints, sheet?.Settings.isImmortal ?? false);

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (sheet == null) return;

            Name.text = sheet.Information.Name;
            Level.text = sheet.Characteristics.Identity.CurrentValue != Identity.Lifeless ? sheet.Characteristics.Level.Output : "";
        }
    }
}