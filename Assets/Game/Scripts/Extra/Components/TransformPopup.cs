using DG.Tweening;
using UnityEngine.Events;
using UnityEngine;

public class TransformPopup
{
	public bool IsIn { get; private set; }

	public bool IsProccess => sequence?.IsPlaying() ?? false;

	private Sequence sequence;
	private Vector3 one = Vector3.one, zero = Vector3.zero;

	private Transform transform;
	private float durationIn, durationOut;

	public TransformPopup(Transform transform, float durationIn = 0.5f, float durationOut = 0.5f)
	{
		this.transform = transform;
		this.durationIn = durationIn;
		this.durationOut = durationOut;
	}

	public void SetIn()
	{
		transform.localScale = one;

		IsIn = true;
	}
	public void SetOut()
	{
		transform.localScale = zero;

		IsIn = false;
	}

	public void PopIn(UnityAction onStart = null, UnityAction onComplete = null)
	{
		onStart?.Invoke();

		TryForceComplete();

		sequence = DOTween.Sequence();
		sequence
			.Append(transform.DOScale(one, durationIn))
			.OnComplete(() =>
			{
				onComplete?.Invoke();

				IsIn = true;
			});
	}
	public void PopOut(UnityAction onStart = null, UnityAction onComplete = null)
	{
		onStart?.Invoke();

		TryForceComplete();

		sequence = DOTween.Sequence();
		sequence
			.Append(transform.DOScale(zero, durationOut))
			.OnComplete(() =>
			{
				onComplete?.Invoke();

				IsIn = false;
			});
	}

	private void TryForceComplete()
	{
		if (sequence != null)
		{
			if (sequence.IsPlaying() == true && sequence.IsActive())
			{
				sequence.Complete(true);
			}
		}
	}
}