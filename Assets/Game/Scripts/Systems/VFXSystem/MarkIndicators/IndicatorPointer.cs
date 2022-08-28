using Game.Managers.PartyManager;

using UnityEngine;

using Zenject;

namespace Game.Systems.VFX
{
	public class IndicatorPointer : MonoBehaviour
	{
		[SerializeField] private IndicatorVFX indicator;
		[SerializeField] private bool isPointer3d = false;

		private IPointer pointer;

		private Cinemachine.CinemachineBrain brain;
		private PartyManager partyManager;
		private Pointer3D.Factory factory3D;
		private Pointer2D.Factory factory2D;

		[Inject]
		private void Construct(Cinemachine.CinemachineBrain brain, PartyManager partyManager, Pointer3D.Factory factory3D, Pointer2D.Factory factory2D)
		{
			this.brain = brain;
			this.partyManager = partyManager;
			this.factory3D = factory3D;
			this.factory2D = factory2D;
		}

		private void Update()
		{
			if (!indicator.IsSwowing)
			{
				DisposePointer();
				return;
			}

			Vector3 playerPosition = partyManager.PlayerParty.LeaderParty.Model.Transform.position;
			Vector3 direction = transform.position - playerPosition;

			Ray ray = new Ray(playerPosition, direction);
			//Debug.DrawRay(playerPosition, direction);

			//[0]-left [1]-right [2]-up [3]-down
			var panels = GeometryUtility.CalculateFrustumPlanes(brain.OutputCamera);
			int planeIndex = 0;

			float minDistance = Mathf.Infinity;

			for (int i = 0; i < panels.Length; i++)
			{
				if (panels[i].Raycast(ray, out float distance))
				{
					if (distance < minDistance)
					{
						minDistance = distance;
						planeIndex = i;
					}
				}
			}

			if (direction.magnitude > minDistance)//show
			{
				if (pointer == null)
				{
					CreatePointer();
				}
				pointer.Transform.position = isPointer3d ? 
					ray.GetPoint(minDistance) :
					brain.OutputCamera.WorldToScreenPoint(ray.GetPoint(minDistance));

				if (!isPointer3d)
				{
					pointer.Transform.rotation = GetIconRotation(planeIndex);
				}
			}
			else//hide
			{
				DisposePointer();
			}
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
	
		private void CreatePointer()
		{
			if (isPointer3d)
			{
				pointer = factory3D.Create();
			}
			else
			{
				pointer = factory2D.Create();
			}
		}

		private void DisposePointer()
		{
			if (pointer != null)
			{
				pointer.DespawnIt();
				pointer = null;
			}
		}
	}
}