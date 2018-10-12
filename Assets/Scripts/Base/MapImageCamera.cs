using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapImageCamera : MonoBehaviour {

	public Transform[] mapPoints;
	public float speed = 1f;
	public IntVariable missionIndex;


	private void Start () {
		StartCoroutine(MoveToPoint(mapPoints[missionIndex.value].position));
	}

	public void ChangePoint() {
		StopAllCoroutines();
		StartCoroutine(MoveToPoint(mapPoints[missionIndex.value].position));
	}

	private IEnumerator MoveToPoint(Vector3 target) {
		Vector3 startPoint = transform.position;
		target += new Vector3(0,0,-10);
		float f = 0;
		while (f < 1f) {
			f += Time.deltaTime * speed;
			transform.position = Vector3.Lerp(startPoint, target, f);
			yield return null;
		}
	}
}
