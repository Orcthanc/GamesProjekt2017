using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleBehaviour : MonoBehaviour {

    private Animator anim;
    private string[] names;
	// Use this for initialization
    void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        Debug.Log(anim.ToString());

        int i = 0;
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        names = new string[clips.Length];
        foreach (AnimationClip clip in clips)
         {
             names[i++] = clip.name;
             Debug.Log(names[i - 1]);
         }
        anim.Play("Shoot");

        Debug.Log(AnimatorIsPlaying("Empty"));
    }

    // Update is called once per frame
    void Update()
    {
        anim.Update(Time.deltaTime);
    }

    bool AnimatorIsPlaying()
    {
        return anim.GetCurrentAnimatorStateInfo(0).length >
               anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    bool AnimatorIsPlaying(string stateName)
    {
        return AnimatorIsPlaying() && anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
}
