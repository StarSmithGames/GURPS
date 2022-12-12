using DG.Tweening;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace Game.Systems.VFX
{
	public class DecalVFX : PoolableObject
	{
		public bool IsEnabled { get => isEnable; protected set => isEnable = value; }
		private bool isEnable = true;

		[SerializeField] protected DecalProjector projector;

		private void Start()
		{
			projector.material = new Material(projector.material);
		}

		public void Enable(bool trigger)
		{
			IsEnabled = trigger;
			projector.enabled = IsEnabled;
		}

		public void DrawDecal(Vector3 position)
		{
			transform.position = position + Vector3.up * 2;
		}

		public DecalVFX SetFade(float value)
		{
			projector.fadeFactor = value;
			Enable(projector.fadeFactor != 0);

			return this;
		}

		public void FadeTo(float end, float duration = 0.25f, bool despawnItOnEnd = false, UnityAction callback = null)
		{
			Enable(true);

			if (end == 0)
			{
				DOTween
					.To(() => projector.fadeFactor, x => projector.fadeFactor = x, 0f, duration)
					.OnComplete(() =>
					{
						Enable(false);
						if (despawnItOnEnd)
						{
							DespawnIt();
						}
						callback?.Invoke();
					});
			}
			else
			{
				DOTween
					.To(() => projector.fadeFactor, x => projector.fadeFactor = x, end, duration)
					.OnComplete(() =>
					{
						if (despawnItOnEnd)
						{
							DespawnIt();
						}
						callback?.Invoke();
					});
			}
		}

		public void SetColor(Color color)
		{
			projector.material.color = color;
		}
	}
}