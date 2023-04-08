using UnityEngine;

public class StepTargetRaysContainer : MonoBehaviour
{
	[SerializeField] private IKBodyController _bodyController;
	[SerializeField] float _stepLength = 3;

	private Vector3 _offset;

	void Awake() => _offset = transform.localPosition;
	void Update() => transform.localPosition = _bodyController.Direction * _stepLength + _offset;
}
