using Game.Entities.Models;
using Game.Systems.SheetSystem.Effects;

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AreaOfEffect : MonoBehaviour
{
	[SerializeField] private InflictEffectData data;
	[SerializeField] private Collider col;

	private Dictionary<Collider, IEffect> colliders = new Dictionary<Collider, IEffect>();

	private void OnTriggerEnter(Collider other)
	{
		var model = other.GetComponent<ICharacterModel>();
		if (model != null)
		{
			colliders.Add(other, model.Character.Sheet.Effects.Apply(data));
		}
	}

	private void OnTriggerExit(Collider other)
	{
		colliders.TryGetValue(other, out IEffect effect);
		effect?.Deactivate();
		colliders.Remove(other);
	}
}