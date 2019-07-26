using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayController : MonoBehaviour {

	public GameObject controlsObject;

	public Text currentTopicText;
	public IntVariable currentDays;

	[Header("Screens")]
	public List<HelpScreenTopic> topics = new List<HelpScreenTopic>();

	[Header("Scroll")]
	public Transform topicPrefab;
	public Transform topicListParent;
	public int visibleSize;
	public GameObject upArrow;
	public GameObject downArrow;
	private EntryList<TopicEntry> topicEntryList = new EntryList<TopicEntry>(0);


	private void Start() {
		controlsObject.SetActive(false);
	}

	private void GenerateTopicList() {
		topicEntryList.ResetList();
		topicEntryList = new EntryList<TopicEntry>(visibleSize);

		topics.Sort((x,y) => string.Compare(x.topic, y.topic));
		for (int i = 0; i < topics.Count; i++) {
			if (topics[i].unlockDay > currentDays.value)
				continue;
			Transform t = Instantiate(topicPrefab, topicListParent);
			TopicEntry entry = topicEntryList.CreateEntry(t);
			bool newTopic = (currentDays.value == topics[i].unlockDay);
			entry.FillData(i, topics[i].topic, newTopic);
		}
		topicPrefab.gameObject.SetActive(false);
		UpdateTopicList();
	}

	/// <summary>
	/// Updates the state of the how to play screen.
	/// </summary>
	/// <param name="active"></param>
    public void UpdateState(bool active) {
        controlsObject.SetActive(active);
		GenerateTopicList();
	}

	/// <summary>
	/// Changes screen if possible.
	/// </summary>
    public bool Move(int dir) {
		topicEntryList.Move(dir);
		UpdateTopicList();
		return true;
    }
	
	/// <summary>
	/// Resets the help screen position back to the first one again.
	/// </summary>
	public void BackClicked() {
		controlsObject.SetActive(false);
	}

	/// <summary>
	/// Checks if it's possible to click on OK in this screen.
	/// </summary>
	/// <returns></returns>
	public bool CheckOk() {
		return (topicEntryList.GetEntry().index == topics.Count -1);
	}

	/// <summary>
	/// Shows the current controls screen as well as scroll arrows.
	/// </summary>
	private void UpdateTopicList() {
		int index = topicEntryList.GetEntry().index;
		currentTopicText.text = topics[index].topic;

		upArrow.SetActive(topicEntryList.CanScrollUp());
		downArrow.SetActive(topicEntryList.CanScrollDown());

		for (int i = 0; i < topics.Count; i++) {
			topics[i].screen.SetActive(i == index);
		}
	}

}
