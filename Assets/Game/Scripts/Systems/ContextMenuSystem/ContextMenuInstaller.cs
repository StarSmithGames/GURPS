using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

public class ContextMenuInstaller : Installer<ContextMenuInstaller>
{
	public override void InstallBindings()
	{
		Container.Bind<ContextMenuHandler>().AsSingle();
	}
}