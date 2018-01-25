using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class LevelStarter : MonoBehaviour {

    public string scene;
	// Usethis for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if(Input.anyKeyDown){
            SceneManager.LoadScene(scene);
        }
	}
}
