using Sirenix.OdinInspector;

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
        public string nameId;

        [VerticalGroup("Information/Split/Right")]
        public string descriptionId;

        public virtual string GetName() => !nameId.IsEmpty() ? LocalizationSystem.LocalizationSystem.TranslateStatic(nameId, LocalizationSystem.LocalizationSystem.CurrentLocaleStatic) : "";
        public virtual string GetDescription() => !descriptionId.IsEmpty() ? LocalizationSystem.LocalizationSystem.TranslateStatic(descriptionId, LocalizationSystem.LocalizationSystem.CurrentLocaleStatic) : "NULL Description";

        public bool IsHasPortrait => portrait != null;
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