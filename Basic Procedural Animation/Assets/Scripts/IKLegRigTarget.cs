using System;
using UnityEngine;

public class IKLegRigTarget : MonoBehaviour
{
	[Header("References :")]
	[SerializeField] private StepTargetRay _stepTargetRay;
	[SerializeField] private IKLegRigTarget _adjacentFoot;
	[SerializeField] private IKLegRigTarget _oppositeFoot;
	[SerializeField] private bool _oppositeDependant;
	[SerializeField, Range(0.1f, 1f)] float _stepDuration = 0.3f;
	[SerializeField] float _stepDistanceThreshold = 3;
	[SerializeField] float _turnDistanceThreshold = 1.5f;
	[SerializeField] float _stepHeight = 1;
	[SerializeField] Vector3 footOffset = new Vector3(0, 0.35f, 0);

	[Header("Debug References")]
	[SerializeField] private bool _show = false;
	[SerializeField] private float _sphereRadius = 0.3f;

	private Action DoAction;
	private Vector3 _oldPosition, _currentPosition, _newPosition;
	private Vector3 _oldNormal, _currentNormal, _newNormal;
	private float _currentDistanceThreshold;
	private float _stepSpeed;
	private float _lerpDelta = 1;

	
	void Start()
	{
		_currentDistanceThreshold = _stepDistanceThreshold;
		_currentPosition = _oldPosition = _newPosition = _stepTargetRay.Point + footOffset;
		_currentNormal = _oldNormal = _newNormal = _stepTargetRay.Normal;

		SetStepDuration(_stepDuration);

		if (_oppositeDependant && _oppositeFoot)
			SetModeVoid();
		else
			SetModeCheckForStep();
	}

	void OnValidate() => SetStepDuration(_stepDuration);

	void Update()
	{
		transform.position = _currentPosition;
		transform.up = _currentNormal;

		DoAction();
	}

	private void SetModeVoid() => DoAction = DoActionVoid;
	private void DoActionVoid() { }

	private void SetModeCheckForStep() => DoAction = DoActionCheckForStep;
	private void DoActionCheckForStep()
	{
		_currentDistanceThreshold = IsSpotTurning() ? _turnDistanceThreshold : _stepDistanceThreshold;

		if (Vector3.Distance(_newPosition, _stepTargetRay.Point) > _currentDistanceThreshold && !_adjacentFoot.IsMoving() && _lerpDelta >= 1 && !_oppositeDependant)
			SetModePerformStep();
	}

	private void SetModePerformStep()
	{
		if (_oppositeFoot)
		{
			if (!_oppositeDependant && _oppositeFoot._oppositeDependant)
				_oppositeFoot.SetModePerformStep();
		}

		_lerpDelta = 0;
		_newPosition = _stepTargetRay.Point + footOffset;
		_newNormal = _stepTargetRay.Normal;

		DoAction = DoActionPerformStep;
	}

	private void DoActionPerformStep()
	{
		if (_lerpDelta < 1)
		{
			Vector3 lLerpedPosition = Vector3.Lerp(_oldPosition, _newPosition, _lerpDelta);
			lLerpedPosition.y += Mathf.Sin(_lerpDelta * Mathf.PI) * _stepHeight;

			_currentPosition = lLerpedPosition;
			_currentNormal = Vector3.Lerp(_oldNormal, _newNormal, _lerpDelta);
			_lerpDelta += Time.deltaTime * _stepSpeed;
		}
		else
		{
			_currentPosition = _oldPosition = _newPosition;
			_currentNormal = _oldNormal = _newNormal;

			if (_oppositeDependant)
				SetModeVoid();
			else
				SetModeCheckForStep();
		}
	}

	private void SetStepDuration(float pDuration) => _stepSpeed = 1 / pDuration;
	private bool IsMoving() => _lerpDelta < 1;
	private bool IsSpotTurning() => Input.GetAxisRaw("Horizontal") != 0 && Input.GetAxisRaw("Vertical") == 0;

	void OnDrawGizmos()
	{
		if (_show)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(transform.position, _sphereRadius);

			if (!_stepTargetRay) return;

			Vector3 lPoint = _stepTargetRay.Point != Vector3.zero ? _stepTargetRay.Point : transform.position;

			if (Vector3.Distance(transform.position, lPoint) > _currentDistanceThreshold)
			{
				Gizmos.color = Color.blue;
				Debug.DrawLine(transform.position, lPoint, Color.blue);
			}
			else
			{
				Gizmos.color = Color.red;
				Debug.DrawLine(transform.position, lPoint, Color.red);
			}

			Gizmos.DrawSphere(_stepTargetRay.Point, _sphereRadius);
		}
	}
}
