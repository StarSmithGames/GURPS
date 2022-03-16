using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

[CreateAssetMenu(fileName = "VFXInstaller", menuName = "Installers/VFXInstaller")]
public class VFXInstaller : ScriptableObjectInstaller<VFXInstaller>
{
	public VFX characterMarker;

	public override void InstallBindings()
	{
		Container.BindInterfacesAndSelfTo<VFXManager>().AsSingle();
	}
}