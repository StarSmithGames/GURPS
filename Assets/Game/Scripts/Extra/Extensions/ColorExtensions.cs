using UnityEngine;

public static class ColorExtensions
{
	public static Color Error => new Color32(255, 51, 51, 255);

	public static Color Cornflowerblue => new Color32(100, 149, 237, 255);
	public static Color CornflowerblueLight => new Color32(169, 196, 245, 255);
	public static Color Gold => new Color32(255, 215, 0, 255);
	public static Color GoldLight => new Color32(230, 194, 0, 255);

	public static string ToHEX(this Color color, bool useAlpha = false)
	{
		return "#" + (useAlpha ? ColorUtility.ToHtmlStringRGBA(color) : ColorUtility.ToHtmlStringRGB(color));
	}
}