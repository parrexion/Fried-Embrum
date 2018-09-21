using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutsideEditor : MonoBehaviour {

	private void Awake () {
		
#if !UNITY_EDITOR
		Destroy(gameObject);
#endif
	}

}
