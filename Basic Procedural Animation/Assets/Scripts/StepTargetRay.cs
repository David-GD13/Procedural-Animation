using UnityEngine;

public class StepTargetRay : MonoBehaviour
{
	[Header("References :")]
	[SerializeField] private float _rayLength = 5f;
	[SerializeField] private float _sphereRadius = 2.5f;
	[SerializeField] private LayerMask _terrainLayer;

	[Header("Debug References :")]
	[SerializeField] private bool _show = false;

	public Vector3 Point => _point;
	public Vector3 Normal => _hitNormal;
	public LayerMask TerrainLayer => _terrainLayer;
	public float Length => _rayLength;

	private Vector3 _hitNormal;
	private Vector3 _point;

	void Awake()
	{
		Ray lRay = new Ray(transform.position, transform.forward);

		if (Physics.Raycast(lRay, out RaycastHit lHit, _rayLength, _terrainLayer))
		{
			_point = lHit.point;
			_hitNormal = lHit.normal;
		}
	}

	void Update()
	{
		Ray lRay = new Ray(transform.position, transform.forward);

		if (Physics.Raycast(lRay, out RaycastHit lHit, _rayLength, _terrainLayer))
		{
			_point = lHit.point;
			_hitNormal = lHit.normal;
		}
	}

	void OnDrawGizmos()
	{
		if (_show)
			Debug.DrawRay(transform.position, transform.forward * _rayLength, Color.red);
	}
}
