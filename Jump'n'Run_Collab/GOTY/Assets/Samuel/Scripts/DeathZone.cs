using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour {

	public void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "deathzone", true);
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawIcon(transform.position, "deathzone", true);
    }
}
