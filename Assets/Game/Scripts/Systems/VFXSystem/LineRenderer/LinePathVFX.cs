using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Zenject;

namespace Game.Systems.VFX
{
    public class LinePathVFX : LineVFX
	{
		public class Factory : PlaceholderFactory<LinePathVFX> { }
	}
}