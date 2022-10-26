using Game.Entities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.SheetSystem
{
	public sealed class Effects
	{
		public event UnityAction onRegistratorChanged;
		public event UnityAction<IEffect> onRegistratorApplied;
		public event UnityAction<IEffect> onRegistratorRemoved;

		public List<IEffect> CurrentEffects => registrator.registers;

		private Registrator<IEffect> registrator;

		private ISheet sheet;
		private EffectFactory effectFactory;

		public Effects(ICharacter character, EffectFactory effectFactory)
		{
			this.sheet = character.Sheet;
			this.effectFactory = effectFactory;

			registrator = new Registrator<IEffect>();
		}

		public IEffect Apply(EffectData data)
		{
			var effect = effectFactory.Create(data, sheet);
			registrator.Registrate(effect);

			effect.onActivationChanged += OnActivationChanged;
			effect.Activate();
			onRegistratorApplied?.Invoke(effect);
			onRegistratorChanged?.Invoke();

			return effect;
		}

		public List<IEffect> Apply(IEnumerable<EffectData> datas)
		{
			List<IEffect> effects = new List<IEffect>();
			foreach (var data in datas)
			{
				effects.Add(Apply(data));
			}

			return effects;
		}

		public void Remove(IEffect effect)
		{
			if (registrator.UnRegistrate(effect))
			{
				effect.onActivationChanged -= OnActivationChanged;
				effect.Deactivate();
				onRegistratorRemoved?.Invoke(effect);
				onRegistratorChanged?.Invoke();
			}
		}

		private void OnActivationChanged(IEffect effect)
		{
			if (!effect.IsActive)
			{
				Remove(effect);
			}
		}
	}

	public interface IEffect : IActivation
	{
		event UnityAction<IEffect> onActivationChanged;

		event UnityAction<float> onProgress;

		float Progress { get; }

		EffectData Data { get; }
	}

	public abstract class Effect : IEffect
	{
		public event UnityAction<IEffect> onActivationChanged;
		public abstract event UnityAction<float> onProgress;

		public bool IsActive { get; private set; }
		public float Progress { get; protected set; }
		public virtual EffectData Data { get; }

		public virtual void Activate()
		{
			IsActive = true;

			onActivationChanged?.Invoke(this);
		}

		public virtual void Deactivate()
		{
			IsActive = false;

			onActivationChanged?.Invoke(this);
		}
	}

	public sealed class InstantEffect : Effect
	{
		public override event UnityAction<float> onProgress;

		public override EffectData Data => data;
		private InstantEffectData data;

		private List<Enchantment> enchantments;

		public InstantEffect(InstantEffectData data, ISheet sheet)
		{
			this.data = data;

			enchantments = data.enchantments.Select((x) => x.GetEnchantment(sheet)).ToList();
		}

		public override void Activate()
		{
			base.Activate();

			onProgress?.Invoke(1);

			for (int i = 0; i < enchantments.Count; i++)
			{
				enchantments[i].Activate();
			}

			Deactivate();
		}

		public class Factory : PlaceholderFactory<InstantEffectData, ISheet, InstantEffect> { }
	}

	public sealed class InflictEffect : Effect
	{
		public override event UnityAction<float> onProgress;

		public bool IsBlinked => data.isBlinkOnEnd;

		private List<Enchantment> enchantments;

		public override EffectData Data => data;
		private InflictEffectData data;
		private AsyncManager asyncManager;

		public InflictEffect(InflictEffectData data, ISheet sheet, AsyncManager asyncManager)
		{
			this.data = data;
			this.asyncManager = asyncManager;

			enchantments = data.enchantments.Select((x) => x.GetEnchantment(sheet)).ToList();
		}

		public override void Activate()
		{
			base.Activate();

			onProgress?.Invoke(1f);

			for (int i = 0; i < enchantments.Count; i++)
			{
				enchantments[i].Activate();
			}
		}

		public override void Deactivate()
		{
			for (int i = 0; i < enchantments.Count; i++)
			{
				if (enchantments[i].IsReversible)
				{
					enchantments[i].Deactivate();
				}
			}

			base.Deactivate();
		}

		public class Factory : PlaceholderFactory<InflictEffectData, ISheet, InflictEffect> { }
	}

	public sealed class ProcessEffect : Effect
	{
		public override event UnityAction<float> onProgress;

		public bool IsBlinked => data.isBlinkOnEnd;

		private List<Enchantment> enchantments;

		public override EffectData Data => data;
		private ProcessEffectData data;
		private AsyncManager asyncManager;

		public ProcessEffect(ProcessEffectData data, ISheet sheet, AsyncManager asyncManager)
		{
			this.data = data;
			this.asyncManager = asyncManager;

			enchantments = data.enchantments.Select((x) => x.GetEnchantment(sheet)).ToList();
		}

		public override void Activate()
		{
			base.Activate();

			asyncManager.StartCoroutine(Process());
		}

		private IEnumerator Process()
		{
			float t = 0;
			int key = 0;
			float precision = 1e-2f;//maybe need 1e-1f
			bool isCompleted = false;

			while (t <= data.duration)
			{
				onProgress?.Invoke(t / data.duration);

				if (!isCompleted)
				{
					float keyTime = data.curve.keys[key].time;

					if (Mathf.Abs(t - keyTime) <= precision)
					{
						//execution
						for (int i = 0; i < enchantments.Count; i++)
						{
							enchantments[i].Activate();
						}

						if (key + 1 >= data.curve.keys.Length)
						{
							isCompleted = true;
						}

						key++;
					}
				}

				t += Time.deltaTime;

				yield return null;
			}

			onProgress?.Invoke(1);

			base.Deactivate();
		}

		public class Factory : PlaceholderFactory<ProcessEffectData, ISheet, ProcessEffect> { }
	}


	public sealed class EffectFactory : PlaceholderFactory<EffectData, ISheet, IEffect> { }

	public sealed class CustomEffectFactory : IFactory<EffectData, ISheet, IEffect>
	{
		private InstantEffect.Factory instantFactory;
		private ProcessEffect.Factory processFactory;
		private InflictEffect.Factory infictFactory;

		public CustomEffectFactory(InstantEffect.Factory instantFactory, ProcessEffect.Factory processFactory, InflictEffect.Factory infictFactory)
		{
			this.instantFactory = instantFactory;
			this.processFactory = processFactory;
			this.infictFactory = infictFactory;
		}

		public IEffect Create(EffectData data, ISheet sheet)
		{
			if(data is InstantEffectData instantData)
			{
				return instantFactory.Create(instantData, sheet);
			}
			else if(data is ProcessEffectData processData)
			{
				return processFactory.Create(processData, sheet);
			}
			else if(data is InflictEffectData inflictData)
			{
				return infictFactory.Create(inflictData, sheet);
			}

			throw new NotImplementedException();
		}
	}
}