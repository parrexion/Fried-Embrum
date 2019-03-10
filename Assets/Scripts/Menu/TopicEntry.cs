using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopicEntry : ListEntry {

	public int index;

	
	public void FillData(int index, string topicName, bool newTopic) {
		this.index = index;
		entryName.text = topicName;
		icon.enabled = newTopic;
	}
}
