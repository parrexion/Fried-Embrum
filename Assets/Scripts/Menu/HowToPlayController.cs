using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HowToPlayController : MonoBehaviour {

	public GameObject controlsObject;

	public Text currentTopicText;
	public IntVariable currentChapterIndex;

	[Header("Screens")]
	public List<HelpScreenTopic> topics = new List<HelpScreenTopic>();
	private int screenIndex;
	private int offsetIndex;

	[Header("Scroll")]
	public Transform topicTemplate;
	public Transform topicListTransform;
	public int size;
	public GameObject upArrow;
	public GameObject downArrow;
	private List<TopicEntry> topicEntryList = new List<TopicEntry>();


	private void Start() {
		controlsObject.SetActive(false);

		topicEntryList.Add(topicTemplate.GetComponent<TopicEntry>());
		for (int i = 1; i < size; i++) {
			Transform t = Instantiate(topicTemplate, topicListTransform);
			topicEntryList.Add(t.GetComponent<TopicEntry>());
		}
		topics.RemoveAll((x) => (x).unlockChapter > currentChapterIndex.value);
		topics.Sort((x,y) => string.Compare(x.topic, y.topic));
		SetupScreens();
	}

	/// <summary>
	/// Updates the state of the how to play screen.
	/// </summary>
	/// <param name="active"></param>
    public void UpdateState(bool active) {
        controlsObject.SetActive(active);
		SetupScreens();
	}

	/// <summary>
	/// Moves one screen to the left if possible.
	/// </summary>
    public bool MoveUp() {
		if (screenIndex > 1) {
        	screenIndex--;
		}
		else if (offsetIndex > 0) {
        	offsetIndex--;
		}
		else if (screenIndex > 0) {
        	screenIndex--;
		}
		else {
			return false;
		}

		SetupScreens();
		return true;
    }

	/// <summary>
	/// Moves one screen to the right if possible.
	/// </summary>
    public bool MoveDown() {
		if (screenIndex < size - 2) {
        	screenIndex++;
		}
		else if (offsetIndex < topics.Count - (size)) {
        	offsetIndex++;
		}
		else if (screenIndex < size - 1) {
        	screenIndex++;
		}
		else {
			return false;
		}

		SetupScreens();
		return true;
    }

	/// <summary>
	/// Resets the help screen position back to the first one again.
	/// </summary>
	public void BackClicked() {
		screenIndex = 0;
		controlsObject.SetActive(false);
	}

	/// <summary>
	/// Checks if it's possible to click on OK in this screen.
	/// </summary>
	/// <returns></returns>
	public bool CheckOk() {
		return (screenIndex == topics.Count -1);
	}

	/// <summary>
	/// Shows the current controls screen as well as scroll arrows.
	/// </summary>
	private void SetupScreens() {
		currentTopicText.text = topics[screenIndex + offsetIndex].topic;

		upArrow.SetActive(screenIndex + offsetIndex != 0);
		downArrow.SetActive(screenIndex + offsetIndex < topics.Count-1);

		for (int i = 0; i < topicEntryList.Count; i++) {
			topicEntryList[i].topicName.text = topics[i + offsetIndex].topic;
			topicEntryList[i].newTopic.enabled = (currentChapterIndex.value == topics[i + offsetIndex].unlockChapter);
			topicEntryList[i].highlight.enabled = (i == screenIndex);
		}

		for (int i = 0; i < topics.Count; i++) {
			topics[i].screen.SetActive(i == screenIndex + offsetIndex);
		}
	}

}
