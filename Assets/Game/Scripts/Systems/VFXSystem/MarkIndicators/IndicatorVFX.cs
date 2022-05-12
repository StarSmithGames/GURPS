using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using System.Diagnostics;

namespace Game.Systems.VFX
{
	public class IndicatorVFX : MonoBehaviour
	{
		public bool IsSwowing { get; private set; }
		public bool IsHideProcess { get; private set; }

		[SerializeField] private MeshRenderer renderer;
		[SerializeField] private Transform body;
		[SerializeField] private Transform mesh;
		[Space]
		[SerializeField] private Transform relativePosition;

		private Material CurrentMaterial
		{
			get
			{
				if(currentMaterial == null)
				{
					currentMaterial = renderer.material;
				}

				return currentMaterial;
			}
		}
		private Material currentMaterial;

		private Sequence animation;

		private void Update()
		{
			if (IsSwowing)
			{
				if(relativePosition != null)
				{
					transform.position = new Vector3(relativePosition.position.x, transform.position.y, relativePosition.position.z);
				}
			}
		}

		public void Enable(bool trigger)
		{
			gameObject.SetActive(trigger);
			IsSwowing = trigger;
		}

		public void Show()
		{
			Vector3 endPositin = transform.position;
			Vector3 startPosition = endPositin - new Vector3(0, 0.25f, 0);

			transform.position = startPosition;
			body.localScale = Vector3.zero;
			SetAlpha(0f);
			Enable(true);

			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(transform.DOMove(endPositin, 0.25f))
				.Join(CurrentMaterial.DOFade(1f, 0.25f))
				.Join(body.DOScale(Vector3.one, 0.4f))
				.AppendInterval(0.5f)
				.Append(body.DORotate(new Vector3(0, 0, 15), 0.25f))
				.AppendCallback(PlayAnimation);
		}

		public void Hide()
		{
			IsHideProcess = true;

			animation?.Kill();

			Sequence sequence = DOTween.Sequence();

			sequence
				.Append(body.DORotate(new Vector3(0, 0, 0), 0.25f))
				.Join(transform.DOScale(0, 0.25f))
				.AppendCallback(() =>
				{
					Enable(false);
					IsHideProcess = false;
				});
		}

		private void PlayAnimation()
		{
			animation = DOTween.Sequence();

			animation
				.Append(body.DOLocalRotate(new Vector3(0, 360, 0), 0.85f, RotateMode.FastBeyond360))
				.SetRelative(true)
				.SetLoops(-1)
				.SetEase(Ease.Linear);
		}

		private void SetAlpha(float value)
		{
			Color color = CurrentMaterial.color;
			color.a = value;
			CurrentMaterial.color = color;
		}



		private bool isFlipped;

		public void FlipToBack()
		{
			if (isFlipped) return;
			isFlipped = true;

			transform.DOLocalRotate(
				new Vector3(180f, 0, 0), 1f, RotateMode.LocalAxisAdd);
		}
		public void FlipToFront()
		{
			if (!isFlipped) return;
			isFlipped = false;

			transform.DOLocalRotate(
				new Vector3(-180f, 0, 0), 1f, RotateMode.LocalAxisAdd);
		}
	}
}