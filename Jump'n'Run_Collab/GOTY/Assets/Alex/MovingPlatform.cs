using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public Transform movingPlatform;
    public Transform start;
    public Transform ziel;
    public float speed;
    private Vector3 newPosition;
    private string currentState;
    public float resetTime;
	
	void Start () {
        currentState = "Moving to Position 2";
        CurrentState();
	}
	void Update()
    {
      
    }
	// Update is called once per frame
	void FixedUpdate () {
       
        movingPlatform.position = Vector3.MoveTowards(movingPlatform.position, newPosition, speed * Time.deltaTime);
       
    }

    public void CurrentState()
    {
        if (currentState=="Moving to Position 1")
        {
            currentState = "Moving to Position 2";
            newPosition = ziel.position;
        }
        else if (currentState == "Moving to Position 2")
        {
            currentState = "Moving to Position 1";
            newPosition = start.position;
        }
        Invoke("CurrentState", resetTime);
    }
}
