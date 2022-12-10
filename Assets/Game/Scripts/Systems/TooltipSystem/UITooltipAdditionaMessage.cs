using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.TooltipSystem
{
    public class UITooltipAdditionaMessage : MonoBehaviour
    {
		[field: SerializeField] public TMPro.TextMeshProUGUI Message { get; private set; }

		public TooltipAdditionalMessageType Type { get; private set; }
		
		public void SetMessage(string add, TooltipAdditionalMessageType type)
		{
			switch (type)
			{
				case TooltipAdditionalMessageType.Projectiles:
				{
					Message.text = $"{add} projectiles";
					Message.color = Color.white;

					Type = TooltipAdditionalMessageType.Projectiles;
					break;
				}

				default:
				{
					Type = TooltipAdditionalMessageType.None;
					break;
				}
			}

			gameObject.SetActive(Type != TooltipAdditionalMessageType.None);
		}
	}

	public enum TooltipAdditionalMessageType
	{
		None,
		Projectiles,
	}
}