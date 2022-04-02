using Game.Systems.SheetSystem;

using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

namespace Game.Entities
{
	[CreateAssetMenu(fileName = "EntityData", menuName = "Game/Entity")]
    public class EntityData : ScriptableObject
    {
        public string CharacterName
        {
            get
            {
                if (localizations.Count > 0)
                {
                    return GetLocalization().characterName;
                }

                return "";
            }
        }

        [PreviewField]
        public Sprite characterSprite;

        [ListDrawerSettings(ListElementLabelName = "Tittle")]
        [InfoBox("@LocalizationInfo", InfoMessageType.Warning)]
        public List<Localization> localizations = new List<Localization>();

        public CharacterSheetSettings characterSheet;

        public Localization GetLocalization(SystemLanguage language = SystemLanguage.English)
        {
            return localizations.Find((x) => x.language == language) ?? localizations[0];
        }

        private string LocalizationInfo => "Required :\n" + SystemLanguage.English.ToString();

        [System.Serializable]
        public class Localization
        {
            public SystemLanguage language = SystemLanguage.English;

            public string characterName;

            private string Tittle => language.ToString() + " " + (!(string.IsNullOrEmpty(characterName) || string.IsNullOrWhiteSpace(characterName)));
		}
	}
}