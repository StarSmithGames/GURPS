using UnityEngine;

using Zenject;

namespace Game.Systems.CursorSystem
{
	public class CursorSystem : IInitializable
	{
		private CursorSettings settings;

		public CursorSystem(CursorSettings settings)
		{
			this.settings = settings;
		}

		public void Initialize()
		{
			SetCursor(CursorType.Hand);
		}

		public void SetCursor(CursorType cursor)
		{
			Cursor.SetCursor(GetCursorTexture(cursor), Vector2.zero, CursorMode.ForceSoftware);
		}

		private Texture2D GetCursorTexture(CursorType cursor)
		{
			if (cursor == CursorType.Base) return settings.cursor;
			else if (cursor == CursorType.Hand) return settings.cursorHand;
			else if (cursor == CursorType.Sword) return settings.cursorSword;
			else if (cursor == CursorType.Bow) return settings.cursorBow;
			else if (cursor == CursorType.Wand) return settings.cursorWand;

			return settings.cursor;
		}
	}

	[System.Serializable]
	public class CursorSettings
	{
		public Texture2D cursor;
		public Texture2D cursorHand;
		public Texture2D cursorSword;
		public Texture2D cursorBow;
		public Texture2D cursorWand;
	}

	public enum CursorType
	{
		Base,
		Hand,
		Sword,
		Bow,
		Wand,
	}
}