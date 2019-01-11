using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OPMath {

	/// <summary>
	/// Utility class for automatic wrap around for index.
	/// Both lower and upper limits are inclusive.
	/// </summary>
	/// <param name="lower"></param>
	/// <param name="upper"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public static int FullLoop(int lower, int upper, int value) {
		int diff = 1 + upper - lower;
        if (diff <= 0) {
            Debug.LogError("Loop is not proper -  Min: " + lower + "  Max: " + upper);
            return lower;
        }
		while (value < lower)
			value += diff;
		while (value > upper)
			value -= diff;
		return value;
	}
}
