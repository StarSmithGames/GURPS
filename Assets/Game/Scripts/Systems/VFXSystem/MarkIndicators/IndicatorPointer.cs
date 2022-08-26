using Game.Managers.CharacterManager;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

namespace Game.Systems.VFX
{
	public class IndicatorPointer : MonoBehaviour
	{
		[SerializeField] private IndicatorVFX indicator;
		[SerializeField] private bool isPointer3d = false;
		[ShowIf("isPointer3d")]
		[SerializeField] private Transform pointer3DPrefab;
		[HideIf("isPointer3d")]
		[SerializeField] private Transform pointer2DPrefab;

		private Pointer CurrentPointer
		{
			get
			{
				if (currentPointer == null)
				{
					if (isPointer3d)
					{
						currentPointer = new Pointer(Instantiate(pointer3DPrefab));
					}
					else
					{
						//currentPointer = new Pointer(Instantiate(pointer2DPrefab, uiManager.transform));
					}
				}

				return currentPointer;
			}
		}
		private Pointer currentPointer;

		private Cinemachine.CinemachineBrain brain;
		private CharacterManager characterManager;

		[Inject]
		private void Construct(Cinemachine.CinemachineBrain brain, CharacterManager characterManager)
		{
			this.brain = brain;
			this.characterManager = characterManager;
		}

		private void Update()
		{
			//if (!indicator.IsSwowing)
			//{
			//	if (CurrentPointer.IsShowing)
			//	{
			//		CurrentPointer.Enable(false);
			//	}
			//	return;
			//}

			//Vector3 playerPosition = characterManager.CurrentParty.LeaderParty.transform.position;
			//Vector3 direction = transform.position - playerPosition;

			//Ray ray = new Ray(playerPosition, direction);
			//Debug.DrawRay(playerPosition, direction);

			////[0]-left [1]-right [2]-up [3]-down
			//var panels = GeometryUtility.CalculateFrustumPlanes(brain.OutputCamera);
			//int planeIndex = 0;

			//float minDistance = Mathf.Infinity;

			//for (int i = 0; i < panels.Length; i++)
			//{
			//	if (panels[i].Raycast(ray, out float distance))
			//	{
			//		if (distance < minDistance)
			//		{
			//			minDistance = distance;
			//			planeIndex = i;
			//		}
			//	}
			//}

			//if (direction.magnitude > minDistance)//show
			//{
			//	if (!CurrentPointer.IsShowing)
			//	{
			//		CurrentPointer.Enable(true);
			//	}
			//	CurrentPointer.Transform.position = isPointer3d ?
			//		ray.GetPoint(minDistance) :
			//		brain.OutputCamera.WorldToScreenPoint(ray.GetPoint(minDistance));

			//	if (!isPointer3d)
			//	{
			//		CurrentPointer.Transform.rotation = GetIconRotation(planeIndex);
			//	}
			//}
			//else//hide
			//{
			//	if (CurrentPointer.IsShowing)
			//	{
			//		CurrentPointer.Enable(false);
			//	}
			//}
		}

		private Quaternion GetIconRotation(int planeIndex)
		{
			if (planeIndex == 0)
			{
				return Quaternion.Euler(0, 0, -90f);
			}
			else if (planeIndex == 1)
			{
				return Quaternion.Euler(0, 0, 90f);
			}
			else if (planeIndex == 2)
			{
				return Quaternion.Euler(0, 0, 0);
			}

			return Quaternion.Euler(0, 0, 180f);
		}

		public class Pointer
		{
			public bool IsShowing { get; private set; }
			public Transform Transform { get; private set; }

			public Pointer(Transform pointer)
			{
				Transform = pointer;
				IsShowing = pointer.gameObject.activeSelf;
			}

			public void Enable(bool trigger)
			{
				IsShowing = trigger;
				Transform.gameObject.SetActive(trigger);
			}
		}
	}
}