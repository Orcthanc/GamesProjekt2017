using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingRobotBehaviour : Enemy {

    public GameObject player;
    public float rotationSpeed = 100;
    private Transform target;

    public new void Start()
    {
        target = new GameObject().transform;
    }

    public override void Move()
    {
        target.position = transform.position;
        target.LookAt(player.transform);
        target.Rotate(new Vector3(0, 180, 0), Space.Self);

        float step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, step);

    }

    public override bool CheckAgro()
    {
        return true;
    }

    public override void Shoot()
    {
    }
}
