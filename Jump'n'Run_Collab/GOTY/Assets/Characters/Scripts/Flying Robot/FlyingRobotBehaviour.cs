using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FlyingRobotBehaviour : Enemy {

    public GameObject player;
    public float speed = 2;
    public float rotationSpeed = 100;
    public float distanceToPlayer = 15;
    public float inaccuracy = 10;
    public float viewDist = 50;
    public GameObject projectile;
    private Transform target;
    private Animation anim;
    [SerializeField]
    private Transform projectileSpawn;

    public new void Start()
    {
        charController = GetComponent<CharacterController>();
        target = new GameObject().transform;
        speed *= Mathf.Sign(Random.Range(-1, 1));
        anim = GetComponentInChildren<Animation>();
    }

    /// <summary>
    /// Moves the robot
    /// </summary>
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

    /// <summary>
    /// Used to check agro while not agroed (is player in sight?)
    /// </summary>
    /// <returns>Returns true if the enemy should start attacking the player</returns>
    public override bool CheckAgro()
    {
        if (!anim.isPlaying)
            anim.Play("Idle");
        return SeesPlayer();
    }

    /// <summary>
    /// Checks if the player is within line of sight
    /// </summary>
    /// <returns>true, if the player is in sight</returns>
    bool SeesPlayer()
    {
        // bit shift the index of the layer to get a bit mask 
        int layermask = 1 << 9;
        int othermask = 0xFFFF ^ layermask;

        RaycastHit hit;

        Debug.Log(!Physics.Raycast(transform.position, player.transform.position - transform.position, viewDist, othermask));
        Debug.Log(Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, viewDist, layermask));

        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, viewDist, layermask))
        {
            Debug.DrawRay(transform.position, player.transform.position - transform.position, Color.red, 1f);
            if(hit.collider.gameObject.layer.Equals(9))
                if(Mathf.Abs(Vector3.Angle(
                        from: transform.forward,
                        to: hit.point - transform.position) 
                        - 180) < 90)
                {
                    return true;
                }
        }
        return false;
    }

    /// <summary>
    /// Spawns a new projectile
    /// </summary>
    public override void Shoot()
    {
        anim.Play("Shoot");
        Instantiate(projectile, projectileSpawn.transform.position, HelperMethods.scatter(projectileSpawn.transform.rotation, inaccuracy));
    }
}
