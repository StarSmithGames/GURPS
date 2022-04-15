using Game.Systems.VFX;

using UnityEngine;

namespace Game.Systems.VFX
{
	public class LineRendererSplineVFX : LineRendererVFX
	{
		public void SetPath(Vector3[] path)
		{
			Line.useWorldSpace = true;

			Line.positionCount = path.Length;
			Line.SetPositions(path);
		}

		public void Path(Vector3 from, Vector3 to)
		{
			SetPath(new Vector3[]
			{
			from,
			to,
			});
		}

		public void Clear()
		{
			Line.positionCount = 0;
		}
	}
}