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

        public string nameId;

        [ListDrawerSettings(ListElementLabelName = "Tittle")]
        [InfoBox("@LocalizationInfo", InfoMessageType.Warning)]
        public List<Localization> localizations = new List<Localization>();

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
    public class HumanoidEntityInformation : EntityInformation
    {
		public override string Name
		{
			get
			{
				string name = string.IsNullOrEmpty(base.Name) ? "EMPTY" : base.Name;
				return $"<color=#{ColorUtility.ToHtmlStringRGBA(nameColor)}>{name}</color>";
			}
		}

		public Color nameColor = Color.white;
    }

    [System.Serializable]
    public class CharacterInformation : HumanoidEntityInformation { }

    [System.Serializable]
    public class NPCInformation : HumanoidEntityInformation { }


    [System.Serializable]
    public class ContainerInformation : EntityInformation { }
}