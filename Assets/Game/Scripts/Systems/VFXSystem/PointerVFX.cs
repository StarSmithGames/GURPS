using UnityEngine;

namespace Game.Systems.VFX
{
    public class PointerVFX : MonoBehaviour
    {
		private void Start()
		{
            Enable(false);
		}

		public void Enable(bool trigger)
        {
            gameObject.SetActive(trigger);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}