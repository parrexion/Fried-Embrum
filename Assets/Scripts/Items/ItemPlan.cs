using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemPlan : ScriptableObject {

	public string planName;
	public bool invention;
	public ItemEntry item;
	public int level;
	public int cost;
	public int boost;
	public int power;
	public int weight;

}
