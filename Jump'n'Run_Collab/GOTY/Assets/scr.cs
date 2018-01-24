using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr : MonoBehaviour {

	void Start () {
        removeColliders(gameObject.transform);
	}

    void removeColliders(Transform go){
        int count = go.GetChildCount();
        Collider[] colliders = go.GetComponents<Collider>();

        foreach(Collider c in colliders){
            c.enabled = false;
        }

        for (int i = 0; i < count; i++){
            removeColliders(go.transform.GetChild(i));
        }
    }
}
