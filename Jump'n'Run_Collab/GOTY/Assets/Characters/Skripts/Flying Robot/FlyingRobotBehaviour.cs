using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingRobotBehaviour : Enemy {

    public GameObject player;
    public float speed = 2;
    public float rotationSpeed = 100;
    public float distanceToPlayer = 15;
    public GameObject projectile;
    private Transform target;
    [SerializeField]
    private Transform projectileSpawn;

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

        if(Mathf.Abs((transform.position - player.transform.position).magnitude) > distanceToPlayer)
        {
            charController.Move(transform.forward * Time.deltaTime * speed * 0.5f);
        }
        else
        {
            charController.Move(-transform.forward * Time.deltaTime * speed * 0.5f);
        }
    }

    public override bool CheckAgro()
    {
        return true;
    }

    public override void Shoot()
    {
        GameObject temp = Instantiate(projectile, projectileSpawn.transform.position, projectileSpawn.transform.rotation);
        //temp.transform.position = transform.position;
    }
}
