using UnityEngine;
using UnityEngine.AI;
using System.Security;
using System.Collections;

public class RobotBehaviour : Enemy
{

    private Animator anim;
    private NavMeshAgent agent;
    private Transform player;
    private Transform eyes;
    private Transform muzzle;
    private Transform lastSeen;
    private LineRenderer shot;
    private float preferedDistance;
    private bool aiming;

    public float inaccuracyModifier;
    public float viewDistance;
    public float shootDistance;


    public new void Start()
    {
        //base.Start();
        anim = gameObject.GetComponentInChildren<Animator>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        agent.autoTraverseOffMeshLink = true;
        eyes = transform.Find("Eyes");
        muzzle = Find(transform,"Muzzle");
        shot = GetComponent<LineRenderer>();
        preferedDistance = Random.Range(shootDistance / 2, shootDistance);


        if (eyes == null)
            throw new System.Exception(gameObject.ToString() + " says: Eyes not found, how am I supposed to see ?!?");

        if (muzzle == null)
            throw new System.Exception(gameObject.ToString() + " says: Muzzle not found, how am I supposed exterminate ?!?");
        
    }

    // Update is called once per frame

    public new void Update()
    {
        Debug.DrawLine(muzzle.position, player.position);
        base.Update();

        anim.Update(Time.deltaTime);

        if (lastSeen != null && agent.destination != lastSeen.position)
        {
            Destinate(lastSeen);
        }

        LookAt(agent.nextPosition);

        if (agent.speed > 0.0f)
        {
            setWalk();
        }


        if (Distance(player) < preferedDistance)
        {
            Destinate(gameObject.transform);
            setAim();
            LookAt(player);
        }

        updateLastSeen();
    }

    /// <summary>
    /// Shoots in the direction of the player...
    /// </summary>
    public override void Shoot()
    {
        if (aiming)
        {
            int layermask = 1 << 9;
            int othermask = 0xFFFF ^ layermask;

            Vector3 spread = new Vector3(RandomSpread(),RandomSpread(), RandomSpread());
            Vector3 dir = Vector3.Normalize(player.position - muzzle.position) + spread;
            RaycastHit hitInfo;

            Debug.DrawRay(muzzle.position, dir, Color.green, 1f);

            if (Physics.Raycast(muzzle.position, dir, out hitInfo, shootDistance))
            {
                StartCoroutine(Shoot((dir * hitInfo.distance)));
            }
            else
            {
                StartCoroutine(Shoot(muzzle.position + (dir * shootDistance)));
            }


            if (!Physics.Raycast(muzzle.position, dir, shootDistance, othermask)
                && Physics.Raycast(muzzle.position, dir, shootDistance, layermask))
            {
                Debug.Log("PlayerHit");
            }

            anim.SetTrigger("Shoot");
        }
    }

    /// <summary>
    /// Draws the shot animation
    /// </summary>
    /// <param name="target">Target of the animation</param>
    /// <returns>Some thing, used for Unitys coroutine</returns>
    private IEnumerator Shoot(Vector3 target)
    {
        shot.SetPosition(0, muzzle.position);
        shot.SetPosition(1, target);
        shot.enabled = true;
        yield return new WaitForSeconds(0.1f);
        shot.enabled = false;
    }

    /// <summary>
    /// Used to get a random Multiplier for the spread of the shot
    /// </summary>
    /// <returns>The multiplier for the spread of a shot</returns>
    public float RandomSpread(){
        return Random.Range(-inaccuracyModifier, inaccuracyModifier);
    }

    /// <summary>
    /// Starts the "Walk" animation
    /// </summary>
    public void setWalk()
    {
        aiming = false;
        anim.SetBool("Walk", true);
        anim.SetBool("Aim", false);
    }
    
    /// <summary>
    /// Starts the "Aim" animation
    /// </summary>
    public void setAim()
    {
        aiming = true;
        anim.SetBool("Walk", false);
        anim.SetBool("Aim", true);
    }

    /// <summary>
    /// Unimplemented in the current version of the script
    /// </summary>
    /// <returns>Always false...</returns>
    public override bool CheckAgro()
    {
        return false;
    }

    /// <summary>
    /// Does nothing in this script...
    /// </summary>
    public override void Move()
    {

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

        if(!Physics.Raycast(eyes.position, player.position - eyes.position, viewDistance, othermask)
           && Physics.Raycast(eyes.position, player.position - eyes.position, viewDistance, layermask)){
            
            Debug.DrawRay(eyes.position, player.position - eyes.position,Color.red,1f);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Updates lastSeen to the position of the player, if and only if SeesPlayer() returns true
    /// </summary>
    public void updateLastSeen(){
        if(SeesPlayer()){
            lastSeen = player;
        }       
    }

    /// <summary>
    /// Used to find a child of a transform by name
    /// 
    /// Example Usage:
    /// Find(transform, "Eyes");
    /// </summary>
    /// <param name="transform">Transform of wich the desired transform is a child of</param>
    /// <param name="name">Name of the child</param>
    /// <returns>The child</returns>
    public Transform Find(Transform transform,string name){
        if(transform.childCount == 0){
            return null;
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);
            if(t != null &&t.name.Equals(name)){
                return t;
            }

            t = Find(t, name);
            if(t != null){
                return t;
            }
        }
        return null;
    }
    /*
     * Oof
     */

    /// <summary>
    /// Shortcut for LookAt(target.position);
    /// </summary>
    /// <param name="target"></param>
    void LookAt(Transform target) {
        LookAt(target.position);
    }

    /// <summary>
    /// Shortcut for gameObject.transform.LookAt(new Vector3 (target.x, gameObject.transform.position.y,target.z)); 
    /// </summary>
    /// <param name="target"></param>
    void LookAt(Vector3 target) { 
        gameObject.transform.LookAt(new Vector3 (target.x, gameObject.transform.position.y,target.z)); 
    }

    /// <summary>
    /// Shortcut for agent.destination = target;
    /// </summary>
    /// <param name="target"></param>
    void Destinate(Vector3 target) {
        agent.destination = target;
    }

    /// <summary>
    /// Shortcut for agent.destination = target.position;
    /// </summary>
    /// <param name="target"></param>
    void Destinate(Transform target) {
        agent.destination = target.position;
    }

    /// <summary>
    /// Shortcut for return Distance(target.position);
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    float Distance(Transform target) {
        return Distance(target.position);
    }

    /// <summary>
    /// Shortcut for return Vector3.Distance(gameObject.transform.position, target);
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    float Distance (Vector3 target) {
        return Vector3.Distance(gameObject.transform.position, target);
    }
}
