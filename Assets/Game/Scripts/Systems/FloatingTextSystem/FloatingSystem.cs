using UnityEngine;

using DG.Tweening;
using Cinemachine;

namespace Game.Systems.FloatingTextSystem
{
	public class FloatingSystem
	{
		private FloatingText lastDamageType = null;

		private FloatingText.Factory floatingTextFactory;
		private CinemachineBrain brain;

		public FloatingSystem(FloatingText.Factory floatingTextFactory, CinemachineBrain brain)
		{
			this.floatingTextFactory = floatingTextFactory;
			this.brain = brain;
		}

		public void CreateText(Vector3 position, string text, Color? color = null, AnimationType type = AnimationType.BasicDamage)
		{
			switch (type)
			{
				case AnimationType.BasicDamage:
				{
					var obj = Create();

					Sequence sequence = DOTween.Sequence();

					position.y += 2;

					obj.transform.DOMove(position, 0.5f).SetEase(Ease.OutQuint);
					sequence
						.Append(obj.Text.DOFade(0f, 0.7f))
						.AppendCallback(obj.DespawnIt);
					break;
				}

				case AnimationType.AdvanceDamage:
				{
					var obj = Create();

					obj.transform.localScale = Vector3.zero;

					float offsetX = Random.Range(-1.5f, 1.5f);
					if (Mathf.Abs(offsetX) <= 0.2f) offsetX += Mathf.Sign(offsetX) * 0.1f;

					obj.transform.position += Vector3.up;

					Vector3 endPosition = obj.transform.position + (Vector3.right * offsetX) + (Vector3.up * Random.Range(0f, 1f));

					Sequence sequence = DOTween.Sequence();

					sequence
						.Append(obj.transform.DOJump(endPosition, Random.Range(0.5f, 1.5f), 1, 1f))
						.Join(obj.transform.DOScale(1f, 0.5f))
						.Append(obj.Text.DOFade(0f, 0.5f))
						.AppendCallback(obj.DespawnIt);
					break;
				}

				case AnimationType.BasicDamageType:
				{
					if(lastDamageType != null && lastDamageType.Text.text == text)
					{
						break;
					}

					var obj = Create();
					lastDamageType = obj;

					obj.transform.localScale = Vector3.zero;
					
					position.y += 1.5f;

					Sequence sequence = DOTween.Sequence();
					sequence
						.Append(obj.transform.DOScale(1f, 0.5f))
						.Join(obj.transform.DOMove(position, 0.5f).SetEase(Ease.OutQuint))
						.AppendInterval(1f)
						.Append(obj.transform.DOMove(position + Vector3.up * 5f, 1f))
						.Join(obj.Text.DOFade(0f, 0.7f))
						.AppendCallback(() =>
						{
							obj.DespawnIt();
							lastDamageType = null;
						});
					break;
				}

				default:
				{
					var obj = Create();

					Sequence sequence = DOTween.Sequence();
					sequence
						.AppendInterval(1f)
						.AppendCallback(obj.DespawnIt);
					break;
				}
			}

			FloatingText Create()
			{
				var obj = floatingTextFactory.Create();

				obj.Text.color = color ?? Color.white;
				obj.Text.text = text;
				obj.transform.position = position;
				obj.transform.rotation = brain.OutputCamera.transform.rotation;//billboard, add to update?

				return obj;
			}
		}
	}

	public enum AnimationType
	{
		None,
		BasicDamage,
		AdvanceDamage,
		BasicDamageType,
	}
}