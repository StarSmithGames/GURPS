using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.VFX
{
	public class Pointer3D : BasePointer
	{
		public class Factory : PlaceholderFactory<Pointer3D> { }
	}
}