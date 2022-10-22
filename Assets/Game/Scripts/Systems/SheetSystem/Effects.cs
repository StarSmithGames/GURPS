using Game.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.SheetSystem
{
	public sealed class Effects
	{
		public event UnityAction onRegistratorChanged;

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

		public void Apply(EffectData data)
		{
			var effect = effectFactory.Create(data, sheet);
			registrator.Registrate(effect);

			onRegistratorChanged?.Invoke();

			effect.onActivationChanged += OnActivationChanged;
			effect.Activate();
		}

		public void Apply(IEnumerable<EffectData> datas)
		{
			foreach (var data in datas)
			{
				Apply(data);
			}
		}

		public void Remove(IEffect effect)
		{
			if (registrator.UnRegistrate(effect))
			{
				effect.onActivationChanged -= OnActivationChanged;

				onRegistratorChanged?.Invoke();

				effect.Deactivate();
			}
		}

		private void OnActivationChanged(IEffect effect)
		{
			Remove(effect);
		}
	}

	public interface IEffect : IActivation
	{
		event UnityAction<IEffect> onActivationChanged;
	}

	public abstract class Effect : IEffect
	{
		public event UnityAction<IEffect> onActivationChanged;

		public bool IsActive { get; private set; }

		protected ISheet sheet;

		public Effect(ISheet sheet)
		{
			this.sheet = sheet;
		}

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

		protected ExecutableEnchantment GetEnchantment(EffectType type)//>:c
		{
			switch (type)
			{
				case AddHealthPoints addHealthPoints:
				{
					return new AddStatEnchantment(sheet.Stats.HitPoints, addHealthPoints.add);
				}
			}

			throw new NotImplementedException();
		}
	}

	public sealed class InstantEffect : Effect
	{
		private List<ExecutableEnchantment> enchantments;

		private InstantEffectData data;

		public InstantEffect(InstantEffectData data, ISheet sheet) : base(sheet)
		{
			this.data = data;

			enchantments = data.enchantments.Select((x) => GetEnchantment(x)).ToList();
		}

		public override void Activate()
		{
			base.Activate();

			for (int i = 0; i < enchantments.Count; i++)
			{
				enchantments[i].Execute();
			}

			Deactivate();
		}

		public class Factory : PlaceholderFactory<InstantEffectData, ISheet, InstantEffect> { }
	}

	public sealed class ProcessEffect : Effect
	{
		private List<ExecutableEnchantment> enchantments;

		private ProcessEffectData data;
		private AsyncManager asyncManager;

		public ProcessEffect(ProcessEffectData data, ISheet sheet, AsyncManager asyncManager) : base(sheet)
		{
			this.data = data;
			this.asyncManager = asyncManager;

			enchantments = data.enchantments.Select((x) => GetEnchantment(x)).ToList();
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

			while (t <= data.duration)
			{
				float keyTime = data.curve.keys[key].time;

				if(Mathf.Abs(t - keyTime) <= precision)
				{
					for (int i = 0; i < enchantments.Count; i++)
					{
						enchantments[i].Execute();
					}

					if (key + 1 >= data.curve.keys.Length)
					{
						yield break;
					}

					key++;
				}

				t += Time.deltaTime;

				yield return null;
			}
		}

		public class Factory : PlaceholderFactory<ProcessEffectData, ISheet, ProcessEffect> { }
	}


	public sealed class EffectFactory : PlaceholderFactory<EffectData, ISheet, IEffect> { }

	public sealed class CustomEffectFactory : IFactory<EffectData, ISheet, IEffect>
	{
		private InstantEffect.Factory instantFactory;
		private ProcessEffect.Factory processFactory;

		public CustomEffectFactory(InstantEffect.Factory instantFactory, ProcessEffect.Factory processFactory)
		{
			this.instantFactory = instantFactory;
			this.processFactory = processFactory;
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

			throw new NotImplementedException();
		}
	}
}