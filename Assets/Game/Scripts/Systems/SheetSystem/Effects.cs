using Game.Entities;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Game.Systems.SheetSystem.Effects
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

		public Effects(ISheet sheet, EffectFactory effectFactory)
		{
			this.sheet = sheet;
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
}