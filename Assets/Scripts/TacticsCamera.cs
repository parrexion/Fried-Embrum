using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsCamera : MonoBehaviour {

	public BoxCollider2D boxCollider;
	
	private Vector3 _origin;
	private Vector3 _diff;
	private bool _drag;
	private Camera _camera;

	private void Start() {
		_camera = GetComponent<Camera>();
	}

	private void LateUpdate() {
		if (Input.GetMouseButton(1)) {
			_diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position;
			if (!_drag) {
				_drag = true;
				_origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			}
		} else {
			_drag = false;
		}
		if (_drag) {
			Camera.main.transform.position = AdjustPosition(_origin-_diff);
		}
	}

	private Vector3 AdjustPosition(Vector3 input) {
		float vertExtent = _camera.orthographicSize;
		float horizExtent = vertExtent * Screen.width / Screen.height;
 
		Bounds areaBounds = boxCollider.bounds;
 
		return new Vector3(
			Mathf.Clamp(input.x, areaBounds.min.x + horizExtent, areaBounds.max.x - horizExtent),
			Mathf.Clamp(input.y, areaBounds.min.y + vertExtent, areaBounds.max.y - vertExtent),
			input.z);
	}
}
