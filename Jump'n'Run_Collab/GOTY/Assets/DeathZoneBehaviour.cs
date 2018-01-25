using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZoneBehaviour : MonoBehaviour
{

    public GameObject respawn;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player")){
            Debug.Log("ENTER _________");
            other.transform.position = respawn.transform.position;
            other.GetComponent<NewPlayerMovement>().Die();
        }
    }
}
