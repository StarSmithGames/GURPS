using Sirenix.OdinInspector;
using System.Collections.Generic;

using UnityEngine;

namespace Game.Systems.SheetSystem
{
	public interface IInformation
	{
        string Name { get; }
    }

	[System.Serializable]
	public class EntityInformation : IInformation
    {
        public string Name
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

        [ListDrawerSettings(ListElementLabelName = "Tittle")]
        [InfoBox("@LocalizationInfo", InfoMessageType.Warning)]
        public List<Localization> localizations = new List<Localization>();

        public Localization GetLocalization(SystemLanguage language = SystemLanguage.English)
        {
            return localizations.Find((x) => x.language == language) ?? localizations[0];
        }

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
    public class EntityAvatarInformation : EntityInformation
    {
        [PreviewField]
        public Sprite icon;
    }

    [System.Serializable]
    public class CharacterInformation : EntityAvatarInformation { }

    [System.Serializable]
    public class NPCInformation : EntityAvatarInformation { }
}