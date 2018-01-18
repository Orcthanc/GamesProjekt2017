using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class RobotBehaviour : MonoBehaviour {

    private Animator anim;
    private string[] names;
    int i;

	void Start () {
        anim = gameObject.GetComponent<Animator>();

        int i = 0;
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        names = new string[clips.Length];
        foreach(AnimationClip clip in clips){
            names[i++] = clip.name;
           // Debug.Log(names[i - 1]);
        }
        anim.Play("Walk");
	}
	
	// Update is called once per frame
	void Update () {
        anim.Update(Time.deltaTime);

        ++i;
        if((i %= 500) == 0){
            Debug.Log("-->Shoot");
            anim.Play("Shoot");
        }

   	}
}
