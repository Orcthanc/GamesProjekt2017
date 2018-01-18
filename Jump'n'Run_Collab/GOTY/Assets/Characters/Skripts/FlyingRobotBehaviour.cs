using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingRobotBehaviour : Enemy {

    public GameObject player;
    public float speed = 2;
    public float rotationSpeed = 100;
    private Transform target;

    public new void Start()
    {
        base.Start();
        target = new GameObject().transform;
        speed *= Mathf.Sign(Random.Range(-1, 1));
    }

    public override void Move()
    {
        target.position = transform.position;
        target.LookAt(player.transform);
        target.Rotate(new Vector3(0, 180, 0), Space.Self);

        float step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, step);

        charController.Move(transform.right * Time.deltaTime * speed);
    }

    public override bool CheckAgro()
    {
        return true;
    }

    public override void Shoot()
    {
    }
}
