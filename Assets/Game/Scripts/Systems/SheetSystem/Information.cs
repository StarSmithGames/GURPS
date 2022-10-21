using Sirenix.OdinInspector;

using System;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
    [System.Serializable]
    public class Information
	{
        [TitleGroup("Information")]
        [HorizontalGroup("Information/Split", LabelWidth = 100)]
        [VerticalGroup("Information/Split/Left")]
        [PreviewField(ObjectFieldAlignment.Left, Height = 64)]
        [HideLabel]
        public Sprite portrait;

        [VerticalGroup("Information/Split/Right")]
        [HorizontalGroup("Information/Split/Right/SplitName")]
        [LabelText("@NameLabel")]
        public string name;
        [VerticalGroup("Information/Split/Right")]
        [HorizontalGroup("Information/Split/Right/SplitName")]
        [HideLabel]
        public bool isNameId = true;

        [VerticalGroup("Information/Split/Right")]
        [HorizontalGroup("Information/Split/Right/SplitDescription")]
        [LabelText("@DescriptionLabel")]
        public string description;
        [VerticalGroup("Information/Split/Right")]
        [HorizontalGroup("Information/Split/Right/SplitDescription")]
        [HideLabel]
        public bool isDescriptionId = true;

        public virtual string GetName() => isNameId ? (!name.IsEmpty() ? LocalizationSystem.LocalizationSystem.TranslateStatic(name, LocalizationSystem.LocalizationSystem.CurrentLocaleStatic) : "") : name;
        public virtual string GetDescription() => isDescriptionId ? (!description.IsEmpty() ? LocalizationSystem.LocalizationSystem.TranslateStatic(description, LocalizationSystem.LocalizationSystem.CurrentLocaleStatic) : "NULL Description") : description;

        public bool IsHasPortrait => portrait != null;

        private string NameLabel => isNameId ? "Name Id" : "Name";
        private string DescriptionLabel => isDescriptionId ? "Description Id" : "Description";
    }

    [System.Serializable]
	public class EntityInformation : Information
    {
        public virtual string Name => GetName();

        public string NameColored
		{
			get
			{
                string name = string.IsNullOrEmpty(Name) ? "EMPTY" : Name;
                return $"<color=#{ColorUtility.ToHtmlStringRGBA(nameColor)}>{name}</color>";
            }
		}

        [VerticalGroup("Information/Split/Right")]
        public Color nameColor = Color.white;
	}

	[System.Serializable]
    public class HumanoidInformation : EntityInformation { }

    [System.Serializable]
    public class ModelInformation : EntityInformation { }
}