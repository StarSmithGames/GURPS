using UnityEngine;
using Zenject;

namespace Game.UI.CanvasSystem
{
    [CreateAssetMenu(fileName = "UISubCanvasMapInstaller", menuName = "Installers/UISubCanvasMapInstaller")]
    public class UISubCanvasMapInstaller : ScriptableObjectInstaller<UISubCanvasMapInstaller>
    {
		public override void InstallBindings()
		{
		}
	}
}