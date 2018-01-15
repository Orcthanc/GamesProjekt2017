using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {

	public void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "spawnpoint");
    }
}
