[System.Serializable]
public class InventoryTuple {
	public int index;
	public ItemEntry item;
	public int charge;
	public bool droppable;


	public int GetMissingCharges() {
		if (!item)
			return 0;
		return item.maxCharge - charge;
	}

	public float ChargeCost() {
		if (!item)
			return 0;
		return item.cost / (float)item.maxCharge;
	}
}