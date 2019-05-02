using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CircleRing {
	public float dist;
	public List<Transform> objects;
}

public class CirclePlacer : MonoBehaviour {

	public CircleRing[] rings;


	public void UpdatePlacements() {
		for (int i = 0; i < rings.Length; i++) {
			PlaceCircle(rings[i]);
		}
	}

	private void PlaceCircle(CircleRing ring) {
		float sectorSize = 2 * Mathf.PI / ring.objects.Count;
		for (int i = 0; i < ring.objects.Count; i++) {
			ring.objects[i].GetComponent<RectTransform>().anchoredPosition = ring.dist * new Vector3(Mathf.Cos(sectorSize*i),Mathf.Sin(sectorSize*i),0f);
		}
	}
}
