using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;

using Zenject;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.Localization;
#endif

namespace Game.Systems.LocalizationSystem
{
    public partial class LocalizationSystem : IDisposable
    {
		public string CurrentLocale => LocalizationSettings.SelectedLocale.name;

		public bool IsLocaleProcess => localeCoroutine != null;
		private Coroutine localeCoroutine;

		private SignalBus signalBus;
		private AsyncManager asyncManager;

		public LocalizationSystem(SignalBus signalBus, AsyncManager asyncManager)
		{
			this.signalBus = signalBus;
			this.asyncManager = asyncManager;

			LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
		}

		public void Dispose()
		{
			LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
		}

		public string Translate(string id)
		{
			return LocalizationSettings.StringDatabase.GetLocalizedString(GetTableById(id), id);
		}

		public void TranslateAsync(string id, UnityAction<string> callback)
		{
			AsyncOperationHandle<string> load = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(GetTableById(id), id);

			if (load.IsDone)
			{
				callback?.Invoke(load.Result);
			}
			else
			{
				load.Completed += (o) => callback?.Invoke(o.Result);
			}
		}

		public string[] GetAllLanguages()
		{
			return LocalizationSettings.AvailableLocales.Locales.Select((x) => x.name).ToArray();
		}


		public void ChangeLocale(int local)
		{
			if (!IsLocaleProcess)
			{
				localeCoroutine = asyncManager.StartCoroutine(SetLocale(local));
			}
		}

		private IEnumerator SetLocale(int locale)
		{
			yield return LocalizationSettings.InitializationOperation;
			LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[locale];

			localeCoroutine = null;
		}


		private static string GetTableById(string id)
		{
			if (id.StartsWith("sheet"))
			{
				return "Sheet";
			}
			else if (id.StartsWith("ui"))
			{
				return "UI";
			}
			else if (id.StartsWith("quest"))
			{
				return "Quests";
			}

			return "";
		}

		private void OnLocaleChanged(Locale locale)
		{
			signalBus?.Fire(new SignalLocalizationChanged());
		}
	}

	//Static
	public partial class LocalizationSystem
	{
#if UNITY_EDITOR
		public static string CurrentLocaleStatic => LocalizationEditorSettings.GetLocales().First().name;

		public static string TranslateStatic(string id, string language)
		{
			var tableCollection = LocalizationEditorSettings.GetStringTableCollection(GetTableById(id));
			Locale locale = LocalizationEditorSettings.GetLocale(LocalizationEditorSettings.GetLocales().First((x) => x.name == language).Identifier);
			var table = (StringTable)tableCollection.GetTable(locale.Identifier);

			return table.GetEntry(id).LocalizedValue;
		}

		public static string[] GetLocales()
		{
			return LocalizationEditorSettings.GetLocales().Select((x) => x.name).ToArray();
		}
#endif
	}
}