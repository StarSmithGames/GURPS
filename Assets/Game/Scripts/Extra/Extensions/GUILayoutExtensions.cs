using UnityEngine;
using UnityEngine.Events;

public static class GUILayoutExtensions
{
	public static bool DrawFoldout(bool foldout, string title, bool isSelected = false, bool drawFoldout = true, UnityAction header = null, UnityAction onClick = null, UnityAction onDelete = null)
	{
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();

		foldout = GUILayout.Toggle(foldout, "", drawFoldout ? "Foldout" : "Label");
		if (!drawFoldout)
		{
			GUILayout.Space(6f);
		}
		header?.Invoke();
		GUIStyle style = new GUIStyle(GUI.skin.box);
		style.margin = new RectOffset(0, 0, 0, 0);
		style.normal.textColor = isSelected ? ColorExtensions.Cornflowerblue : Color.white;
		style.hover.textColor = ColorExtensions.CornflowerblueLight;
		style.alignment = TextAnchor.MiddleLeft;
		if (GUILayout.Button(title, style, GUILayout.MaxWidth(1000.0f)))
		{
			if (Event.current.button == 1)
			{
				onDelete?.Invoke();
			}
			else
			{
				onClick?.Invoke();
			}
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		return foldout;
	}

	public static bool DrawMenuListItemButton(string title, bool isSelected = false)
	{
		GUIStyle style = new GUIStyle(GUI.skin.box);
		style.margin = new RectOffset(0, 0, 0, 0);
		style.normal.textColor = isSelected ? ColorExtensions.Gold : Color.white;
		style.hover.textColor = ColorExtensions.GoldLight;
		style.alignment = TextAnchor.MiddleLeft;

		if (GUILayout.Button(title, style, GUILayout.MaxWidth(1000.0f)))
		{
			return true;
		}

		return false;
	}

	public static bool DrawBigButton(string title, int size, Texture2D icon = null, int iconsize = -1)
	{
		GUIStyle style = new GUIStyle(GUI.skin.label);
		style.alignment = TextAnchor.MiddleCenter;
		style.fontStyle = FontStyle.Bold;
		style.normal.textColor = Color.white;
		style.focused.textColor = Color.white;
		style.hover.textColor = Color.white;

		style.hover.textColor = ColorExtensions.CornflowerblueLight;

		GUIStyle styleHorizontal = new GUIStyle(GUI.skin.box);

		GUILayout.BeginHorizontal(styleHorizontal);
		if (GUILayout.Button(title, style, GUILayout.MaxWidth(1000), GUILayout.Height(size)))
		{
			return true;
		}
		if (icon != null)
		{
			int s = iconsize == -1 ? size : iconsize;
			GUILayout.Label(icon, GUILayout.Width(s), GUILayout.Height(s));
		}
		GUILayout.EndHorizontal();

		//GUIStyle style = new GUIStyle(GUI.skin.box);
		//style.normal = GUI.skin.box.normal;
		//style.focused = GUI.skin.box.focused;
		//style.hover = GUI.skin.box.hover;

		//GUILayoutExtensions.MakeTexture(1, 1, Color.blue);
		//style.border = new RectOffset(10, 10, 10, 10);

		//style.border = GUI.skin.button.border;
		//style.padding = new RectOffset(7, 7, 7, 7);

		return false;
	}


	public static GUIStyle GetBoldStyle()
	{
		var boldStyle = new GUIStyle(GUI.skin.label);
		boldStyle.fontStyle = FontStyle.Bold;
		boldStyle.normal.textColor = Color.white;
		boldStyle.focused.textColor = Color.white;
		boldStyle.hover.textColor = Color.white;

		return boldStyle;
	}

	public static Texture2D MakeTexture(int width, int height, Color color)
	{
		Color[] pix = new Color[width * height];

		for (int i = 0; i < pix.Length; i++)
		{
			pix[i] = color;
		}

		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();

		return result;
	}
}