using Sirenix.OdinInspector;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;

using Zenject;

namespace Game.Systems.VFX
{
	public abstract partial class ProjectileVFX : PoolableObject
	{
		[SerializeField] protected bool isRootMove = true;
		[SerializeField] protected bool isLocalSpaceRandomMove = false;
		[Space]
		[SerializeField] protected float moveSpeed = 1f;
		[SerializeField] protected AnimationCurve acceleration;
		[SerializeField] protected float acceleraionTime = 1;
		[Space]
		[SerializeField] protected bool isLookAt = false;
		[SerializeField] protected bool isHomingMove = false;
		[Space]
		[SerializeField] protected float range = 10f;
		[SerializeField] protected float impactRadius = 0;
		[Space]
		[SerializeField] protected bool attachAfterCollision = false;
		[SerializeField] protected float colliderRadius = 0.5f;
		[SerializeField] protected LayerMask layerMask;
		[SerializeField] protected bool ignoreColliders = true;

		//Muzzle effect
		//Impact effect

		protected Transform root;
		protected Transform target;
		protected List<Transform> ignoreList;
		protected Vector3 startPosition;
		protected Vector3 endPosition;
		protected Vector3 forward;
		protected Vector3 forwardPosition;
		protected Vector3 targetPosition;
		protected float startTime;

		protected UnityAction onCompleted;

		protected bool isCanMove;

		[Inject]
		private void Construct()
		{
			root = transform.root;
		}

		protected virtual void Update()
		{
			if (!isCanMove) return;

			//GetRandomDistance
			var delta = new Vector3();
			if (randomMoveCoordinates != MoveCoordinates.None)
			{
				UpdateSmootRandomhPos();
				delta = smootRandomPosition - oldSmootRandomPosition;
				oldSmootRandomPosition = smootRandomPosition;
			}

			//Acceleraion
			float accelerationY = 1;
			if (acceleration.length > 0)
			{
				var time = (Time.time - startTime) / acceleraionTime;
				accelerationY = acceleration.Evaluate(time);
			}

			//Position Rotation
			var moveDistance = Vector3.MoveTowards(root.position, isHomingMove ? target.position : targetPosition, moveSpeed * Time.deltaTime * accelerationY);
			var moveDistanceRandom = moveDistance + delta;
			if (isLookAt && isHomingMove)
			{
				root.LookAt(moveDistanceRandom);
			}

			if (isLocalSpaceRandomMove && isRootMove)
			{
				root.position = moveDistance;
				transform.localPosition += delta;
			}
			else
			{
				root.position = moveDistanceRandom;
			}

			//Raycast
			var distance = Vector3.Distance(root.position, targetPosition);
			var distanceNextFrame = moveSpeed * Time.deltaTime;
			if (distanceNextFrame > distance)
			{
				distanceNextFrame = distance;
			}
			if (distance <= colliderRadius)
			{
				Collision(new RaycastHit());//>:c
			}

			var direction = (targetPosition - root.position).normalized;
			RaycastHit raycastHit;
			if (Physics.Raycast(root.position, direction, out raycastHit, distanceNextFrame + colliderRadius, layerMask))
			{
				if (target == null || ignoreColliders && target.root == raycastHit.transform.root || !ignoreColliders)
				{
					targetPosition = raycastHit.point - direction * colliderRadius;
					Collision(raycastHit);
				}
			}
		}

		public virtual void Launch(UnityAction onCompleted = null)
		{
			this.onCompleted = onCompleted;

			startTime = Time.time;
			deltaSpeed = randomMoveSpeed * Random.Range(1, 1000 * randomRange + 1) / 1000 - 1;
			randomRadiusX = Random.Range(randomMoveRadius / 20, randomMoveRadius * 100) / 100;
			randomRadiusY = Random.Range(randomMoveRadius / 20, randomMoveRadius * 100) / 100;
			randomSpeed = Random.Range(randomMoveSpeed / 20, randomMoveSpeed * 100) / 100;
			randomDirection1 = Random.Range(0, 2) > 0 ? 1 : -1;
			randomDirection2 = Random.Range(0, 2) > 0 ? 1 : -1;
			randomDirection3 = Random.Range(0, 2) > 0 ? 1 : -1;

			oldSmootRandomPosition = smootRandomPosition = Vector3.zero;

			forwardPosition = root.position + forward * range;

			targetPosition = target == null ? forwardPosition : endPosition;

			isCanMove = true;
		}

		public ProjectileVFX SetIgnoreList(List<Transform> ignoreList)
		{
			this.ignoreList = ignoreList;

			return this;
		}

		public ProjectileVFX SetStart(Vector3 position, Vector3 forward)
		{
			startPosition = position;
			this.forward = forward;
			root.position = startPosition;
			if (!isLookAt)
			{
				root.forward = forward;
			}

			return this;
		}

		public ProjectileVFX SetTarget(Transform target)
		{
			endPosition = target == null ? Vector3.zero : target.position;
			this.target = target;

			if (isLookAt && target != null)
			{
				root.LookAt(target);
			}

			return this;
		}

		protected virtual void Collision(RaycastHit hit)
		{
			isCanMove = false;

			if (attachAfterCollision)
				root.parent = hit.transform == null ? target : hit.transform;

			onCompleted?.Invoke();
		}
	}

	public partial class ProjectileVFX
	{
		[Space]
		[SerializeField] private MoveCoordinates randomMoveCoordinates;
		[ShowIf("@randomMoveCoordinates != MoveCoordinates.None")]
		[SerializeField] private bool useDeviation = false;
		[ShowIf("@randomMoveCoordinates != MoveCoordinates.None")]
		[SerializeField] private float randomMoveRadius = 1.5f;
		[ShowIf("@randomMoveCoordinates != MoveCoordinates.None")]
		[SerializeField] private float randomMoveSpeed = 15;
		[ShowIf("@randomMoveCoordinates != MoveCoordinates.None")]
		[SerializeField] private float randomRange = 1;

		private Vector3 smootRandomPosition, oldSmootRandomPosition;
		private float deltaSpeed;
		private float randomSpeed, randomRadiusX, randomRadiusY;
		private int randomDirection1, randomDirection2, randomDirection3;

		private void UpdateSmootRandomhPos()
		{
			//TODO разброс на hitpoint'е
			//if (useDeviation)
			//{
			//	var deviation = Vector3.Distance(root.position, hit.point);// / effectSettings.MoveDistance;
			//	coord1 = randomDirection2 * Mathf.Sin(timeDegree) * randomRadiusX * deviation;
			//	coord2 = randomDirection3 * Mathf.Sin(timeDegree + (randomDirection1 * Mathf.PI / 2) * time + Mathf.Sin(delta)) * randomRadiusY * deviation;
			//}
			//else
			//{

			//}

			float coord1, coord2;
			var time = (Time.time - startTime);

			var timeDegree = time * randomSpeed;
			var delta = time * deltaSpeed;

			coord1 = randomDirection2 * Mathf.Sin(timeDegree) * randomRadiusX;
			coord2 = randomDirection3 * Mathf.Sin(timeDegree + (randomDirection1 * Mathf.PI / 2) * time + Mathf.Sin(delta)) * randomRadiusY;

			if (randomMoveCoordinates == MoveCoordinates.XY)
			{
				smootRandomPosition = new Vector3(coord1, coord2, 0);
			}
			else if (randomMoveCoordinates == MoveCoordinates.XZ)
			{
				smootRandomPosition = new Vector3(coord1, 0, coord2);
			}
			else if (randomMoveCoordinates == MoveCoordinates.YZ)
			{
				smootRandomPosition = new Vector3(0, coord1, coord2);
			}
			else if (randomMoveCoordinates == MoveCoordinates.XYZ)
			{
				smootRandomPosition = new Vector3(coord1, coord2, (coord1 + coord2) / 2 * randomDirection1);
			}
		}

		private void OnDrawGizmos()
		{
			
		}
	}

	public enum MoveCoordinates
	{
		None,
		XY,
		XZ,
		YZ,
		XYZ
	}
}