using Game.UI;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class MenuInstaller : MonoInstaller<MenuInstaller>
{
	public UISubCanvas subCanvas;

	public override void InstallBindings()
	{
		Container.BindInstance(subCanvas);
	}
}