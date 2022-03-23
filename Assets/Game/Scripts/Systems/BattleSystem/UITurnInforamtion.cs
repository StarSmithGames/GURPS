using UnityEngine;

namespace Game.Systems.BattleSystem
{
	public class UITurnInforamtion : MonoBehaviour
	{
		[SerializeField] private TMPro.TextMeshProUGUI textMesh;

		public void SetText(string text)
		{
			textMesh.text = text;
		}
	}
}