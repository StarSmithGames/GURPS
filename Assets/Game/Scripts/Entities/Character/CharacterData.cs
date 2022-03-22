using Game.Systems.InventorySystem;
using Sirenix.OdinInspector;

using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Data/Character")]
    public class CharacterData : ScriptableObject
    {
        [PreviewField]
        public Sprite itemSprite;

        [ListDrawerSettings(ListElementLabelName = "Tittle")]
        [InfoBox("@LocalizationInfo", InfoMessageType.Warning)]
        public List<Localization> localizations = new List<Localization>();

        public InventorySettings inventory;

        public Localization GetLocalization(SystemLanguage language)
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