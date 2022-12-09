using UnityEngine;

using Zenject;

namespace Game.Systems.VFX
{
    public class RadialAreaDecalVFX : DecalVFX
    {
		public float Range => radius;
        [SerializeField] private float radius;

		public void SetRange(float value)
		{
			radius = value;
			projector.size = new Vector3(radius * 2, radius * 2, 10);
		}

		public Vector3 GetHitPoint()
		{
			Ray ray = new Ray(transform.position, Vector3.down);
			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				return hit.point;
			}

			return Vector3.zero;
		}

		private void OnDrawGizmosSelected()
		{
			projector.size = new Vector3(radius * 2, radius * 2, 10);

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(GetHitPoint(), radius);
		}

		public class Factory : PlaceholderFactory<RadialAreaDecalVFX> { }
	}
}