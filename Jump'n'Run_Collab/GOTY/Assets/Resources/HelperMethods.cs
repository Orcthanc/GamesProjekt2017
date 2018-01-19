using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperMethods {

    /// <summary>
    /// Returns a Quaternion slightly rotated by a random amount in a random direction.
    /// </summary>
    /// <param name="dir">The original direction</param>
    /// <param name="inaccuracy">The highest possible rotation that can be applied to \"dir\" by this function</param>
    /// <returns>A slightly randomified Quaternion</returns>
	public static Quaternion scatter(Quaternion dir, float inaccuracy)
    {
        return Quaternion.RotateTowards(dir, new Quaternion(Random.value, Random.value, Random.value, Random.value), Random.value * inaccuracy);
    }
}
