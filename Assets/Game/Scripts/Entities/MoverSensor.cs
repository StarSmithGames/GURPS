using CMF;

using Sirenix.OdinInspector;

using UnityEngine;

using Zenject;

public class MoverSensor : MonoBehaviour
{
	public bool IsGrounded { get; private set; }

	//Current upwards (or downwards) velocity necessary to keep the correct distance to the ground;
	public Vector3 CurrentGroundAdjustmentVelocity { get; private set; }

	[HideInInspector] public Vector3[] raycastArrayPreviewPositions;

	[SerializeField] private Settings settings;

	private int currentLayer;
	private bool IsUsingExtendedSensorRange = true;
	private float baseSensorRange = 0f;


	private Sensor sensor;

	private CapsuleCollider collider;

	[Inject]
	private void Construct(CapsuleCollider collider)
	{
		this.collider = collider;
	}

	private void Start()
{
		sensor = new Sensor(transform.root, collider);
		RecalculateColliderDimensions();
		RecalibrateSensor();
	}

	private void OnValidate()
	{
		//Recalculate collider dimensions;
		if (gameObject.activeInHierarchy)
		{
			if (collider == null) collider = GetComponentInParent<CapsuleCollider>();
			RecalculateColliderDimensions();
		}
		//Recalculate raycast array preview positions;
		if (settings.sensorType == Sensor.CastType.RaycastArray)
			raycastArrayPreviewPositions = Sensor.GetRaycastStartPositions(settings.sensorArrayRows, settings.sensorArrayRayCount, settings.sensorArrayRowsAreOffset, 1f);
	}

	private void LateUpdate()//FixedUpdate()
	{
		//CheckForGround
		if (currentLayer != gameObject.layer)
			RecalculateSensorLayerMask();
		Check();
	}

	//Check if mover is grounded;
	//Store all relevant collision information for later;
	//Calculate necessary adjustment velocity to keep the correct distance to the ground;
	private void Check()
	{
		//Reset ground adjustment velocity;
		CurrentGroundAdjustmentVelocity = Vector3.zero;

		//Set sensor length;
		if (IsGrounded)
			sensor.castLength = baseSensorRange + (settings.colliderHeight * transform.root.localScale.x) * settings.stepHeightRatio;
		else
			sensor.castLength = baseSensorRange;

		sensor.Cast();

		//If sensor has not detected anything, set flags and return;
		if (!sensor.HasDetectedHit())
		{
			IsGrounded = false;
			return;
		}

		IsGrounded = true;

		//Get distance that sensor ray reached;
		float _distance = sensor.GetDistance();

		//Calculate how much mover needs to be moved up or down;
		float _upperLimit = ((settings.colliderHeight * transform.root.localScale.x) * (1f - settings.stepHeightRatio)) * 0.5f;
		float _middle = _upperLimit + (settings.colliderHeight * transform.root.localScale.x) * settings.stepHeightRatio;
		float _distanceToGo = _middle - _distance;

		//Set new ground adjustment velocity for the next frame;
		CurrentGroundAdjustmentVelocity = transform.root.up * (_distanceToGo / Time.fixedDeltaTime);
	}


	//Recalibrate sensor variables;
	private void RecalibrateSensor()
	{
		//Set sensor ray origin and direction;
		sensor.SetCastOrigin(collider.bounds.center);
		sensor.SetCastDirection(Sensor.CastDirection.Down);

		//Calculate sensor layermask;
		RecalculateSensorLayerMask();

		//Set sensor cast type;
		sensor.castType = settings.sensorType;

		//Calculate sensor radius/width;
		float radius = settings.colliderThickness / 2f * settings.sensorRadiusModifier;

		//Multiply all sensor lengths with '_safetyDistanceFactor' to compensate for floating point errors;
		float _safetyDistanceFactor = 0.001f;

		//Fit collider height to sensor radius;
		radius = Mathf.Clamp(radius, _safetyDistanceFactor, collider.radius * (1f - _safetyDistanceFactor));

		//Set sensor radius;
		sensor.sphereCastRadius = radius * transform.root.localScale.x;

		//Calculate and set sensor length;
		float length = 0f;
		length += (settings.colliderHeight * (1f - settings.stepHeightRatio)) * 0.5f;
		length += settings.colliderHeight * settings.stepHeightRatio;
		baseSensorRange = length * (1f + _safetyDistanceFactor) * transform.root.localScale.x;
		sensor.castLength = length * transform.root.localScale.x;

		//Set sensor array variables;
		sensor.ArrayRows = settings.sensorArrayRows;
		sensor.arrayRayCount = settings.sensorArrayRayCount;
		sensor.offsetArrayRows = settings.sensorArrayRowsAreOffset;
		sensor.isInDebugMode = settings.isInDebugMode;

		//Set sensor spherecast variables;
		sensor.calculateRealDistance = true;
		sensor.calculateRealSurfaceNormal = true;

		//Recalibrate sensor to the new values;
		sensor.RecalibrateRaycastArrayPositions();
	}

	private void RecalculateSensorLayerMask()
	{
		int layerMask = 0;
		int objectLayer = gameObject.layer;

		//Calculate layermask;
		for (int i = 0; i < 32; i++)
		{
			if (!Physics.GetIgnoreLayerCollision(objectLayer, i))
				layerMask = layerMask | (1 << i);
		}

		//Make sure that the calculated layermask does not include the 'Ignore Raycast' layer;
		if (layerMask == (layerMask | (1 << LayerMask.NameToLayer("Ignore Raycast"))))
		{
			layerMask ^= (1 << LayerMask.NameToLayer("Ignore Raycast"));
		}

		//Set sensor layermask;
		sensor.layermask = layerMask;

		//Save current layer;
		currentLayer = objectLayer;
	}


	//Set height of collider;
	private void SetColliderHeight(float newColliderHeight)
	{
		if (settings.colliderHeight == newColliderHeight)
			return;

		settings.colliderHeight = newColliderHeight;
		RecalculateColliderDimensions();
	}
	//Set thickness/width of collider;
	private void SetColliderThickness(float newColliderThickness)
	{
		if (settings.colliderThickness == newColliderThickness)
			return;

		if (newColliderThickness < 0f)
			newColliderThickness = 0f;

		settings.colliderThickness = newColliderThickness;
		RecalculateColliderDimensions();
	}
	//Set acceptable step height;
	private void SetStepHeightRatio(float newStepHeightRatio)
	{
		newStepHeightRatio = Mathf.Clamp(newStepHeightRatio, 0f, 1f);
		settings.stepHeightRatio = newStepHeightRatio;
		RecalculateColliderDimensions();
	}

	private void RecalculateColliderDimensions()
	{
		collider.height = settings.colliderHeight;
		collider.center = settings.colliderOffset * settings.colliderHeight;
		collider.radius = settings.colliderThickness / 2f;

		collider.center = collider.center + new Vector3(0f, settings.stepHeightRatio * collider.height / 2f, 0f);
		collider.height *= (1f - settings.stepHeightRatio);

		if (collider.height / 2f < collider.radius)
			collider.radius = collider.height / 2f;

		if (sensor != null)
			RecalibrateSensor();
	}

	private void OnDrawGizmos()
	{
		
	}

	[System.Serializable]
	public class Settings
	{
		public bool isInDebugMode = false;
		public Sensor.CastType sensorType = Sensor.CastType.Raycast;
		public float sensorRadiusModifier = 0.8f;
		[ShowIf("sensorType", Sensor.CastType.RaycastArray)]
		[Range(1, 5)]
		public int sensorArrayRows = 1;
		[ShowIf("sensorType", Sensor.CastType.RaycastArray)]
		[Range(3, 10)]
		public int sensorArrayRayCount = 6;
		[ShowIf("sensorType", Sensor.CastType.RaycastArray)]
		public bool sensorArrayRowsAreOffset = false;
		[Header("Collider Options :")]
		[Range(0f, 1f)]
		public float stepHeightRatio = 0.25f;
		public float colliderHeight = 2f;
		public float colliderThickness = 1f;
		public Vector3 colliderOffset = Vector3.zero;
	}
}