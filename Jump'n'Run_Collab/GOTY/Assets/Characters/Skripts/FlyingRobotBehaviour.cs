using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingRobotBehaviour : Enemy {

    public GameObject player;
	
    public override void Move()
    {
        transform.LookAt(player.transform);
        transform.Rotate(new Vector3(0, 180, 0), Space.Self);
    }

    public override bool CheckAgro()
    {
        return true;
    }

    public override void Shoot()
    {
        Debug.Log(Random.value);
    }
}
