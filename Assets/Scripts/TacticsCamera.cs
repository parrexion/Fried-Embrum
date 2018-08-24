using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera class which keeps track of the action in the game by following cursor 
/// and characters around the map.
/// </summary>
public class TacticsCamera : MonoBehaviour {

	public BoxCollider2D boxCollider;
	public TacticsMoveVariable selectedCharacter;

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
		transform.localPosition = AdjustPosition(transform.localPosition);
	}

	/// <summary>
	/// Sets the camera position in such a wat that the map cursor appears in the camera box.
	/// </summary>
	public void UpdateCameraPositionCursor() {
		Vector3 currentPosition = Camera.main.transform.localPosition;
		Vector3 nextPosition = new Vector3(
			Mathf.Clamp(currentPosition.x, cursorX.value-moveWidth, cursorX.value+moveWidth),
			Mathf.Clamp(currentPosition.y, cursorY.value-moveHeight, cursorY.value+moveHeight),
			currentPosition.z
		);
		transform.localPosition = AdjustPosition(nextPosition);
	}

	/// <summary>
	/// Sets the camera position in such a wat that the selected character appears in the camera box.
	/// </summary>
	public void UpdateCameraPositionFollow() {
		Vector3 currentPosition = Camera.main.transform.localPosition;
		Vector3 nextPosition = new Vector3(
			Mathf.Clamp(currentPosition.x, selectedCharacter.value.posx-moveWidth, selectedCharacter.value.posx+moveWidth),
			Mathf.Clamp(currentPosition.y, selectedCharacter.value.posy-moveHeight, selectedCharacter.value.posy+moveHeight),
			currentPosition.z
		);
		transform.localPosition = AdjustPosition(nextPosition);
	}

	/// <summary>
	/// Adjusts the position to make sure that the camera position stays inside the camera box.
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	private Vector3 AdjustPosition(Vector3 input) {
		float vertExtent = _camera.orthographicSize;
		float horizExtent = vertExtent * _camera.rect.width * (float)Screen.height / (float)Screen.width;
 
		Bounds areaBounds = boxCollider.bounds;
 
		Debug.Log(string.Format("Clamp:  {0}  {1}", vertExtent, horizExtent));
		Debug.Log(string.Format("Clamp:  {0}  {1}", areaBounds.min.x + horizExtent +6, areaBounds.max.x - horizExtent +6));
		Debug.Log(string.Format("Clamp:  {0}  {1}", areaBounds.min.y + vertExtent, areaBounds.max.y - vertExtent));
		return new Vector3(
			Mathf.Clamp(input.x, areaBounds.min.x + horizExtent +6, areaBounds.max.x - horizExtent +6),
			Mathf.Clamp(input.y, areaBounds.min.y + vertExtent, areaBounds.max.y - vertExtent),
			input.z);
	}
}
