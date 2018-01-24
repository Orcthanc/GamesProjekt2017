using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingPlattform : MonoBehaviour {

    public Transform start, goal;
    public float speed;
    public bool destroyOnTarget;

    private Vector3 m_start;
    private Vector3 m_toGoal;

    private int startFrame;
    private bool dontMove;

	// Use this for initialization
	void Start () {
        m_start = start.position;
        m_toGoal = goal.position - start.position;

        startFrame = Time.frameCount;

        Destroy(start.gameObject);
        Destroy(goal.gameObject);
	}

    int framesSinceStart()
    {
        return Time.frameCount - startFrame;
    }
	
	// Update is called once per frame
	void Update () {
        if (!dontMove)
        {
            transform.position = m_start + m_toGoal * (-Mathf.Cos(framesSinceStart() / (1000 / speed)) / 2 + 0.5f);
            if (destroyOnTarget && (framesSinceStart() / (1000 / speed)) > Mathf.PI)
            {
                dontMove = true;
                StartCoroutine(destroy());
            }
        }
	}

    IEnumerator destroy()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }
}
