using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera class which keeps track of the action in the game by following cursor 
/// and characters around the map.
/// </summary>
public class TacticsCamera : MonoBehaviour {

#region Singleton
	private static TacticsCamera instance = null;
	private void Awake() {
		if (instance != null) {
			Destroy(gameObject);
		}
		else {
			instance = this;
		}
	}
#endregion

	public static Bounds boxCollider = new Bounds();
	public static bool boxActive = false;
	public TacticsMoveVariable selectedCharacter;
	public FloatVariable cameraPosX;
	public FloatVariable cameraPosY;

	[Header("Cursor Position")]
	public IntVariable cursorX;
	public IntVariable cursorY;
	public int moveWidth;
	public int moveHeight;
	
	private Vector3 _origin;
	private Vector3 _diff;
	private bool _drag;
	private Camera _camera;


	private void Start() {
		_camera = GetComponent<Camera>();
	}

	/// <summary>
	/// Sets the camera position in such a wat that the map cursor appears in the camera box.
	/// </summary>
	public void UpdateCameraPositionCursor() {
		Vector3 currentPosition = _camera.transform.localPosition;
		Vector3 nextPosition = new Vector3(
			Mathf.Clamp(currentPosition.x, cursorX.value-moveWidth, cursorX.value+moveWidth),
			Mathf.Clamp(currentPosition.y, cursorY.value-moveHeight, cursorY.value+moveHeight),
			currentPosition.z
		);
		transform.localPosition = AdjustPosition(nextPosition);
		cameraPosX.value = transform.localPosition.x;
		cameraPosY.value = transform.localPosition.y;
	}

	/// <summary>
	/// Sets the camera position in such a wat that the selected character appears in the camera box.
	/// </summary>
	public void UpdateCameraPositionFollow() {
		Vector3 currentPosition = _camera.transform.localPosition;
		Vector3 nextPosition = new Vector3(
			Mathf.Clamp(currentPosition.x, selectedCharacter.value.posx-moveWidth, selectedCharacter.value.posx+moveWidth),
			Mathf.Clamp(currentPosition.y, selectedCharacter.value.posy-moveHeight, selectedCharacter.value.posy+moveHeight),
			currentPosition.z
		);
		transform.localPosition = AdjustPosition(nextPosition);
		cameraPosX.value = transform.localPosition.x;
		cameraPosY.value = transform.localPosition.y;
	}

	/// <summary>
	/// Adjusts the position to make sure that the camera position stays inside the camera box.
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	private Vector3 AdjustPosition(Vector3 input) {
		if (!boxActive)
			return input;

		float vertExtent = _camera.orthographicSize;
		float horizExtent = vertExtent * _camera.rect.width * (float)Screen.width / (float)Screen.height; 
 
		return new Vector3(
			Mathf.Clamp(input.x, boxCollider.min.x + horizExtent, boxCollider.max.x - horizExtent),
			Mathf.Clamp(input.y, boxCollider.min.y + vertExtent, boxCollider.max.y - vertExtent),
			input.z);
	}
}
