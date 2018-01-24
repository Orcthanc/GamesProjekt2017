using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

    Animator animator;

    public void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        animator.SetTrigger("push");
    }

    public void Update()
    {
        animator.SetTrigger("unpush");
        animator.SetTrigger("push");
        animator.Update(Time.deltaTime);
    }
}
