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
	public float moveSpeed = 0.1f;

	[Header("Cursor Position")]
	public IntVariable cursorX;
	public IntVariable cursorY;
	public int moveWidth;
	public int moveHeight;
	
	private Vector3 _origin;
	private Vector3 _diff;
	private Camera _camera;
	private Vector3 _goal = new Vector3(0f,0f,-10f);


	private void Start() {
		_camera = GetComponent<Camera>();
	}

	private void Update() {
		transform.localPosition = Vector3.MoveTowards(transform.localPosition, _goal, moveSpeed);
		cameraPosX.value = transform.localPosition.x;
		cameraPosY.value = transform.localPosition.y;
	}

	/// <summary>
	/// Sets the camera position in such a wat that the map cursor appears in the camera box.
	/// </summary>
	public void UpdateCameraPositionCursor() {
		Vector3 currentPosition = _goal;
		Vector3 nextPosition = new Vector3(
			Mathf.Clamp(currentPosition.x, cursorX.value-moveWidth+2, cursorX.value+moveWidth+2),
			Mathf.Clamp(currentPosition.y, cursorY.value-moveHeight, cursorY.value+moveHeight),
			currentPosition.z
		);
		_goal = AdjustPosition(nextPosition);
	}

	/// <summary>
	/// Sets the camera position in such a wat that the selected character appears in the camera box.
	/// </summary>
	public void UpdateCameraPositionFollow() {
		Vector3 currentPosition = _goal;
		Vector3 nextPosition = new Vector3(
			Mathf.Clamp(currentPosition.x, selectedCharacter.value.posx-moveWidth, selectedCharacter.value.posx+moveWidth),
			Mathf.Clamp(currentPosition.y, selectedCharacter.value.posy-moveHeight, selectedCharacter.value.posy+moveHeight),
			currentPosition.z
		);
		_goal = AdjustPosition(nextPosition);
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
		float horizExtent = vertExtent * 0.5f * (float)Screen.width / (float)Screen.height + 1.5f;

		//Debug.Log(string.Format("Clamp {0}  Min: {1} , Max: {2}", horizExtent, boxCollider.min.x + horizExtent, boxCollider.max.x - horizExtent));
		//Debug.Log(string.Format("Clamp {0}  Min: {1} , Max: {2}", vertExtent, boxCollider.min.y + vertExtent, boxCollider.max.y - vertExtent));

		return new Vector3(
			Mathf.Clamp(input.x, boxCollider.min.x + horizExtent +3.2f, boxCollider.max.x - horizExtent +1),
			Mathf.Clamp(input.y, boxCollider.min.y + vertExtent, boxCollider.max.y - vertExtent),
			input.z);
	}
}
