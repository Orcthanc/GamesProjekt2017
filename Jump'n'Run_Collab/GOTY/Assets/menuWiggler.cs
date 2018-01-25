using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Remoting.Metadata.W3cXsd2001;

public class menuWiggler : MonoBehaviour {

    float current;
	// Use this for initialization
	void Start () {
        current = 0f;
	}
	
	// Update is called once per frame
	void Update () {
        current = (current + 0.07f) % (Mathf.PI * 2);
        float sin = Mathf.Sin(current);
        Debug.Log(sin);
        transform.Translate(new Vector3(0f, sin/30, 0f));
	}
}
