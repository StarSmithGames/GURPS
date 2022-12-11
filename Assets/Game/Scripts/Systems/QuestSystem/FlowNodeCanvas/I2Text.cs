using I2.Loc;

using NodeCanvas.DialogueTrees;

using ParadoxNotion;

using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[System.Serializable]
public class I2Text : IStatement // >:c
{
	public string Text => value;

	public string language;
	public string value;

#if UNITY_EDITOR
	public bool isShowFoldout = false;

	public virtual void OnGUI(string label = "", bool isTextArea = true)
	{
		string foldoutLabel = (string.IsNullOrEmpty(value) ? "Empty" : value.CapLength(30)).Replace("\n", "/n ");
		isShowFoldout = EditorGUILayout.Foldout(isShowFoldout, $"{language}: {foldoutLabel}", true);

		if (isShowFoldout)
		{
			GUILayout.BeginVertical("box");
			OnProperties(label, isTextArea);
			GUILayout.EndVertical();
		}
	}

	protected virtual void OnProperties(string label, bool isTextArea)
	{
		if (string.IsNullOrEmpty(label))
		{
			if (isTextArea)
			{
				value = GUILayout.TextArea(value, 200, GUILayout.MinHeight(100));
			}
			else
			{
				value = EditorGUILayout.TextField(value);
			}
		}
		else
		{
			if (isTextArea)
			{
				value = GUILayout.TextArea(value, 200, GUILayout.MinHeight(100));
			}
			else
			{
				value = EditorGUILayout.TextField(label, value);
			}
		}
	}
#endif
}

[System.Serializable]
public class I2AudioText : I2Text
{
	public AudioClip audio;

#if UNITY_EDITOR
	protected override void OnProperties(string label = "", bool isTextArea = true)
	{
		base.OnProperties(label, isTextArea);
		audio = (AudioClip)EditorGUILayout.ObjectField("Audio", audio, typeof(AudioClip), true);
	}
#endif
}

[System.Serializable]
public class I2Texts<T> where T : I2Text, new()
{
	public string Text => GetCurrent().Text;

	public List<T> texts = new List<T>();

	public T GetCurrent()
	{
		LocalizationManager.UpdateSources();
		var langs = LocalizationManager.GetAllLanguages(true);

#if UNITY_EDITOR
		ResizeTexts(langs);
#endif

		if (texts.Count > 0)
		{
			return texts[0];
		}

		return null;
	}

#if UNITY_EDITOR

	public bool isShowFoldout = false;

	public void ResizeTexts(List<string> langs)
	{
		//try resize and save data
		if (langs.Count != texts.Count)
		{
			var cachedList = new List<T>(texts);
			texts.Clear();
			for (int i = 0; i < langs.Count; i++)
			{
				if (i < cachedList.Count && cachedList[i].language == langs[i])
				{
					texts.Add(cachedList[i]);
				}
				else
				{
					var text = cachedList.Find((x) => x.language == langs[i]);

					if(text != null)
					{
						texts.Add(text);
					}
					else
					{
						texts.Add(new T());
						texts[i].language = langs[i];
						texts[i].value = "This is a text!";
					}
				}
			}
		}
		else// check language order
		{
			for (int i = 0; i < langs.Count; i++)
			{
				if(texts[i].language != langs[i])
				{
					T text = null;
					for (int j = i; j < texts.Count; j++)
					{
						if (texts[j].language == langs[i])
						{
							text = texts[j];
							break;
						}
					}

					if (text != null)//swap
					{
						int index = texts.IndexOf(text);
						var swap = texts[i];
						texts[i] = text;
						texts[index] = swap;
					}
					else
					{
						texts[i] = new T();
						texts[i].language = langs[i];
						texts[i].value = "This is a text!";
					}
				}
			}
		}
	}

	public void OnGUI(string label, bool isTextArea = false)
	{
		bool lastFoldout = isShowFoldout;

		string foldoutLabel = GetCurrent().value.CapLength(50).Replace("\n", "/n ");
		isShowFoldout = EditorGUILayout.Foldout(isShowFoldout, $"{label}: {(string.IsNullOrEmpty(foldoutLabel) ? "Empty" : foldoutLabel)}", true);
		if (isShowFoldout)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);
			GUILayout.BeginVertical();
			for (int i = 0; i < texts.Count; i++)
			{
				texts[i].OnGUI(isTextArea : isTextArea);
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		else
		{
			if(lastFoldout != isShowFoldout)
			{
				for (int i = 0; i < texts.Count; i++)
				{
					if (texts[i].isShowFoldout)
					{
						texts[i].isShowFoldout = false;
					}
				}
			}
		}
	}
#endif
}