using UnityEngine;

public static class ColorExtensions
{
	public static string ToHEX(this Color color, bool useAlpha = false)
	{
		return "#" + (useAlpha ? ColorUtility.ToHtmlStringRGBA(color) : ColorUtility.ToHtmlStringRGB(color));
	}
}