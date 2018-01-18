using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingRobot : Enemy {

	// Use this for initialization
	void Start () {
		
	}
	
    public override void Move()
    {

    }

    public override bool CheckAgro()
    {
        return true;
    }

    public override void Shoot()
    {
        throw new System.NotImplementedException();
    }
}
