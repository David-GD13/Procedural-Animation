using UnityEngine;

public class IKBodyController : MonoBehaviour
{
	[Header("References :")]
	[SerializeField] private Transform _normalReference;
	[SerializeField] private Transform[] _legIKTargets;
	[SerializeField] private Vector3 _groundOffset;

	[Header("Movement References")]
	[SerializeField] private float _speed = 10f;
	[SerializeField] private float _turnSpeed = 135f;
	[SerializeField, Range(1, 20)] private float _heightSmoothing = 5f;

	[Header("Debug References :")]
	[SerializeField] private bool _show = false;
	[SerializeField] private float _rayLength = 10f;

	public Vector3 Direction => _direction;

	private Vector3 TargetPosition
	{
		get
		{
			Vector3 lAverage = default;
			int lIKTargetsCount = _legIKTargets.Length;

			for (int i = 0; i < lIKTargetsCount; i++)
				lAverage += _legIKTargets[i].position;

			lAverage /= lIKTargetsCount;
			lAverage.x = Mathf.Round(lAverage.x * 100f) / 100f;
			lAverage.y = Mathf.Round(lAverage.y * 100f) / 100f;
			lAverage.z = Mathf.Round(lAverage.z * 100f) / 100f;

			return lAverage + _groundOffset;
		}
	}

	private Vector3 TargetNormal
	{
		get
		{
			Vector3 lAverage = default;
			int lIKTargetsCount = _legIKTargets.Length;

			for (int i = 0; i < lIKTargetsCount; i++)
				lAverage += _legIKTargets[i].up;

			lAverage /= lIKTargetsCount;

			return lAverage;
		}
	}

	private Vector3 _position = default;
	private Vector3 _direction = default;
	private Vector3 _velocity = default;
	private float _angleY;

	void Start()
	{
		_position = transform.position;
	}

	void FixedUpdate()
	{
		Move();
		SetBodyOrientation();
		SetBodyHeight();
	}

	private void Move()
	{
		float lMoveInput = Input.GetAxis("Vertical");
		_direction = Vector3.forward * lMoveInput;
		_velocity = _direction * _speed * Time.deltaTime;
		transform.Translate(_velocity);
	}

	private void SetBodyHeight()
	{
		_position.x = transform.position.x;
		_position.y = Mathf.Approximately(_position.y, TargetPosition.y) ? TargetPosition.y : Mathf.Lerp(_position.y, TargetPosition.y, Time.deltaTime * _heightSmoothing);
		_position.z = transform.position.z;
		//_position = Vector3.Lerp(_position, TargetPosition, Time.deltaTime * _heightSmoothing);
		transform.position = _position;
	}

	private void SetBodyOrientation()
	{
		_normalReference.up = TargetNormal;
		_angleY += Input.GetAxis("Horizontal") * _turnSpeed * Time.deltaTime;
		transform.rotation = _normalReference.rotation * Quaternion.AngleAxis(_angleY, Vector3.up);
	}

	void OnDrawGizmos()
	{
		if (_show) 
			Debug.DrawRay(_normalReference.position, TargetNormal * _rayLength, Color.magenta);
	}
}
