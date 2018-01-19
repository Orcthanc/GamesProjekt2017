using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingRobotBehaviour : Enemy {

    public GameObject player;
    public float speed = 2;
    public float rotationSpeed = 100;
    public float distanceToPlayer = 15;
    public float inaccuracy = 10;
    public GameObject projectile;
    private Transform target;
    private Animation anim;
    [SerializeField]
    private Transform projectileSpawn;

    public new void Start()
    {
        base.Start();
        target = new GameObject().transform;
        speed *= Mathf.Sign(Random.Range(-1, 1));
        anim = GetComponentInChildren<Animation>();
    }

    public override void Move()
    {
        if (!anim.isPlaying)
            anim.Play("Idle");

        target.position = transform.position;
        target.LookAt(player.transform);
        target.Rotate(new Vector3(0, 180, 0), Space.Self);

        float step = rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, step);

        charController.Move(transform.right * Time.deltaTime * speed);

        if(Mathf.Abs((transform.position - player.transform.position).magnitude) > distanceToPlayer)
        {
            charController.Move(-transform.forward * Time.deltaTime * Mathf.Abs(speed) * 0.9f);
        }
        else
        {
            charController.Move(transform.forward * Time.deltaTime * Mathf.Abs(speed) * 0.5f);
        }
    }

    public override bool CheckAgro()
    {
        if (!anim.isPlaying)
            anim.Play("Idle");
        return true;
    }

    public override void Shoot()
    {
        anim.Play("Shoot");
        GameObject temp = Instantiate(projectile, projectileSpawn.transform.position, HelperMethods.scatter(projectileSpawn.transform.rotation, inaccuracy));
    }
}
