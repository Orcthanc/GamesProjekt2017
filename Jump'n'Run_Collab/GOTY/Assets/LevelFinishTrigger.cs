using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinishTrigger : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player")){
            SceneManager.LoadScene("LevelFinished");
        }
    }
}
