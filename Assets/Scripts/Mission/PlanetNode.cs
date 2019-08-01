using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetNode : MonoBehaviour {

	public PlanetInfo info;


    private void Start() {
        SpriteRenderer planet = GetComponent<SpriteRenderer>();
		planet.color = info.planetColor;
		transform.localScale = new Vector3(info.size, info.size, info.size);
    }

}
