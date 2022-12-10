using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.TooltipSystem
{
	public class UITooltipMessage : MonoBehaviour
	{
		[field: SerializeField] public TMPro.TextMeshProUGUI Message { get; private set; }

		public TooltipMessageType Type { get; private set; }

		public void SetMessage(TooltipMessageType type)
		{
			switch (type)
			{
				case TooltipMessageType.InvalidTarget:
				{
					Message.text = "Invalid Target";
					Message.color = Color.red;

					Type = TooltipMessageType.InvalidTarget;
					break;
				}
				case TooltipMessageType.NotEnoughMovement:
				{
					Message.text = "Not Enough Movement";
					Message.color = Color.yellow;

					Type = TooltipMessageType.NotEnoughMovement;
					break;
				}
				case TooltipMessageType.CanNotReachDesination:
				{
					Message.text = "Can't Reach Destination";
					Message.color = Color.white;

					Type = TooltipMessageType.CanNotReachDesination;
					break;
				}
				case TooltipMessageType.OutOfRange:
				{
					Message.text = "Out Of Range";
					Message.color = Color.red;

					Type = TooltipMessageType.OutOfRange;
					break;
				}
				default:
				{
					Message.text = "";

					Type = TooltipMessageType.None;
					break;
				}
			}

			gameObject.SetActive(Type != TooltipMessageType.None);
		}
	}

	public enum TooltipMessageType
	{
		None,
		InvalidTarget,
		NotEnoughMovement,
		CanNotReachDesination,
		OutOfRange,
	}
}