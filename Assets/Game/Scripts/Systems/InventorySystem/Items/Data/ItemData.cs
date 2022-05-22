using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

namespace Game.Systems.InventorySystem
{
	public abstract class ItemData : ScriptableObject
	{
		public string ItemName
		{
			get
			{
				if (localizations.Count > 0)
				{
					string n = GetLocalization().itemName;
					return string.IsNullOrEmpty(n) ? name : n;
				}

				return name;
			}
		}

		public string ItemDescription
		{
			get
			{
				if (localizations.Count > 0)
				{
					return GetLocalization().itemDescription;
				}

				return "";
			}
		}

		[PreviewField]
		public Sprite itemSprite;

		[ListDrawerSettings(ListElementLabelName = "Tittle")]
		[InfoBox("@LocalizationInfo", InfoMessageType.Warning)]
		public List<Localization> localizations = new List<Localization>();

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

		public Localization GetLocalization(SystemLanguage language = SystemLanguage.English)
		{
			return localizations.Find((x) => x.language == language) ?? localizations[0];
		}

		private string LocalizationInfo => "Required :\n" + SystemLanguage.English.ToString();
		[System.Serializable]
		public class Localization
		{
			public SystemLanguage language = SystemLanguage.English;

			public string itemName;
			[TextArea(5, 5)]
			public string itemDescription;

			private string Tittle => language.ToString() + " " + (!(string.IsNullOrEmpty(itemName) || string.IsNullOrWhiteSpace(itemName))) + " " + (!(string.IsNullOrEmpty(itemDescription) || string.IsNullOrWhiteSpace(itemDescription)));
		}
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