using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleBehaviour : MonoBehaviour
{

    private Animation anim;
    // Use this for initialization
    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animation>();
       // Debug.Log(anim.Play("Empty"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
