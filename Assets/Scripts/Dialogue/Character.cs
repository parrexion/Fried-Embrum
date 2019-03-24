using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour {

	public ScrObjEntryReference character;
	public IntVariable poseIndex;

	private int moveIndex;
	private Vector2 movePosition;

	[SerializeField] private Image characterSprite = null;
	

	// Use this for initialization
	void Start () {
		moveIndex = -1;
		characterSprite.enabled = false;
	}

	public void UpdateCharacter() {
		if (character.value == null) {
			characterSprite.enabled = false;
		}
		else {
			characterSprite.enabled = true;
			PortraitEntry ce = (PortraitEntry)character.value;
			characterSprite.sprite = ce.poses[poseIndex.value];
		}
	}

	public void SetMoveDirection(Vector2 movePosition, int moveIndex) {
		this.movePosition = movePosition;
		this.moveIndex = moveIndex;
	}

	public void MoveCharacter(float moveSpeed) {
		if (moveIndex != -1) {
			StartCoroutine(Animation(movePosition, moveSpeed));
		}
	}

	IEnumerator Animation(Vector2 movePosition, float moveSpeed) {
		Vector2 startPosition = transform.position;
		// Debug.Log("start     " + startPosition.ToString());
		// Debug.Log("end     " + movePosition.ToString());
		// Debug.Log("char     " + character.value.ToString());
		float dist = 0;
		while (dist <= moveSpeed) {
			dist += Time.deltaTime;
			transform.position = Vector2.Lerp(movePosition, startPosition, dist / moveSpeed);
			yield return null;
		}
		transform.position = startPosition;
		moveIndex = -1;
		yield break;
	}

}
