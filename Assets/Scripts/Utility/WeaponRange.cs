[System.Serializable]
/// <summary>
/// Help class which is an int range containing a minimum value and a maximum value.
/// Also contains some help functions for calculating weapon ranges.
/// </summary>
public class WeaponRange {

	public int min;
	public int max;


	public WeaponRange() {}
	public WeaponRange(int min, int max){
		this.min = min;
		this.max = max;
	}

	public bool InRange(int distance) {
		return (min <= distance && distance <= max);
	}

    public override string ToString() {
        if (min != max) {
            return min + "-" + max;
        }
        return min.ToString();
    }

}
