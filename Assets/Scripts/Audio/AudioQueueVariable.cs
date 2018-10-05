using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName="References/Audio Queue Variable")]
public class AudioQueueVariable : ScriptableObject {
	public Queue<AudioClip> value = new Queue<AudioClip>();

	public void Enqueue(SfxEntry sfx) {
		value.Enqueue(sfx.clip);
	}
}
