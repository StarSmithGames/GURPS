using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public abstract class ItemData : ScriptableObject
	{
		[TitleGroup("Information")]
		[HorizontalGroup("Information/Split", LabelWidth = 100)]
		[VerticalGroup("Information/Split/Left")]
		[PreviewField(ObjectFieldAlignment.Left, Height = 64)]
		[HideLabel]
		public Sprite itemSprite;

		[VerticalGroup("Information/Split/Right")]
		public string nameId;

		[VerticalGroup("Information/Split/Right")]
		public string descriptionId;

		[Space]
		[AssetList]
		[InlineEditor(InlineEditorModes.GUIAndPreview)]
		public ItemModel prefab;
		[Space]
		public bool isStackable = false;
		[ShowIf("isStackable")]
		public bool isInfinityStack = false;
		[ShowIf("@isStackable && !isInfinityStack")]
		[Range(1, 9999)]
		public int stackSize = 1;
		[Space]
		public bool isWeighty = true;
		[ShowIf("isWeighty")]
		[SuffixLabel("kg", true)]
		[MinValue(0.01f), MaxValue(99.99f)]
		public float weight = 0.01f;
		[Space]
		public bool isBreakable = false;
		[ShowIf("isBreakable")]
		public DecaySettings decay;
		[Space]
		public ItemRarity rarity;

		public string GetName() => !nameId.IsEmpty() ? LocalizationSystem.LocalizationSystem.TranslateStatic(nameId, LocalizationSystem.LocalizationSystem.CurrentLocaleStatic) : "NULL Name";
		public string GetDescription() => !descriptionId.IsEmpty() ? LocalizationSystem.LocalizationSystem.TranslateStatic(descriptionId, LocalizationSystem.LocalizationSystem.CurrentLocaleStatic) : "NULL Description";
	}

	[InlineProperty]
	[System.Serializable]
	public class DecaySettings
	{
		[Range(0, 100)]
		[SuffixLabel("%/day")]
		public float decayOverTime = 0f;
		[Range(0, 100)]
		[SuffixLabel("%/day")]
		public float decayInside = 0f;
		[Range(0, 100)]
		[SuffixLabel("%/day")]
		public float decayOutsie = 0f;
	}

	public enum ItemRarity
	{
		Common,
		Rare,
		Epic,
		Legendary,
		Set,
	}
}