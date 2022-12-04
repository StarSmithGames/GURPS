using DG.Tweening;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace Game.Systems.VFX
{
	public class DecalVFX : MonoBehaviour
	{
		public bool IsEnabled { get => isEnable; protected set => isEnable = value; }
		private bool isEnable = true;

		[SerializeField] private DecalProjector projector;

		private void Start()
		{
			projector.material = new Material(projector.material);
		}

		public void Enable(bool trigger)
		{
			IsEnabled = trigger;
			projector.enabled = IsEnabled;
		}

		public void EnableIn(UnityAction callback = null)
		{
			projector.fadeFactor = 0;
			Enable(true);

			DOTween
				.To(() => projector.fadeFactor, x => projector.fadeFactor = x, 0.8f, 0.25f)
				.OnComplete(() => { callback?.Invoke(); });
		}

		public void EnableOut(UnityAction callback = null)
		{
			Enable(true);

			DOTween
				.To(() => projector.fadeFactor, x => projector.fadeFactor = x, 0f, 0.2f)
				.OnComplete(() =>
				{ 
					Enable(false);
					callback?.Invoke();
				});
		}

		public void SetColor(Color color)
		{
			projector.material.color = color;
		}

		[Button]
		private void RandomColor()
		{
			SetColor(Random.ColorHSV());
		}
	}
}