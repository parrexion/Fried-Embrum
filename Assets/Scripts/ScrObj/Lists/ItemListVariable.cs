using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Variables/Item List")]
public class ItemListVariable : ScriptableObject {

	public List<ItemEntry> items = new List<ItemEntry>();
}
