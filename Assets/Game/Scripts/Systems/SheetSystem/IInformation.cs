using Sirenix.OdinInspector;
using System.Collections.Generic;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
	[System.Serializable]
	public class EntityInformation
    {
        public virtual string Name
        {
            get
            {
                if (localizations.Count > 0)
                {
                    return GetLocalization().name;
                }

                return "";
            }
        }

        public string NameColored
		{
			get
			{
                string name = string.IsNullOrEmpty(Name) ? "EMPTY" : Name;
                return $"<color=#{ColorUtility.ToHtmlStringRGBA(nameColor)}>{name}</color>";
            }
		}

        public string nameId;

        [ListDrawerSettings(ListElementLabelName = "Tittle")]
        [InfoBox("@LocalizationInfo", InfoMessageType.Warning)]
        public List<Localization> localizations = new List<Localization>();

        public Color nameColor = Color.white;

        public bool isHasPortrait = false;
        [ShowIf("isHasPortrait")]
        [PreviewField]
        public Sprite portrait;

        public Localization GetLocalization(SystemLanguage language = SystemLanguage.English)
        {
            return localizations.Find((x) => x.language == language) ?? localizations[0];
        }

        public bool IsHasPortrait => isHasPortrait && portrait != null;

        private string LocalizationInfo => "Required :\n" + SystemLanguage.English.ToString();


		[System.Serializable]
        public class Localization
        {
            public SystemLanguage language = SystemLanguage.English;

            public string name;

            private string Tittle => language.ToString() + " " + (!(string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name)));
        }
    }

	[System.Serializable]
    public class HumanoidInformation : EntityInformation { }

    [System.Serializable]
    public class ModelInformation : EntityInformation { }
}