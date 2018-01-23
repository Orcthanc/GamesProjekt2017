using UnityEngine;
using UnityEngine.AI;
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
    private bool sees;

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
        updateLastSeen();

        if (eyes == null)
            throw new System.Exception(gameObject.ToString() + " says: Eyes not found, how am I supposed to see ?!?");

        if (muzzle == null)
            throw new System.Exception(gameObject.ToString() + " says: Muzzle not found, how am I supposed to exterminate ?!?");
        
    }

    // Update is called once per frame

    public new void Update()
    {
        shotCooldown -= Time.deltaTime;
        Debug.DrawLine(muzzle.position, player.position);

        anim.Update(Time.deltaTime);

        if (lastSeen != null && agent.destination != lastSeen.position)
        {
            Destinate(lastSeen);
        }

        LookAt(agent.nextPosition);

        if (agent.speed > 0.0f)
        {
            Debug.Log(agent.speed);
            setWalk();
        }


        if (Distance(player) < preferedDistance)
        {
            if(sees){
                Destinate(gameObject.transform);
                setAim();
                LookAt(player);

                if (shotCooldown < 0)
                {
                    shotCooldown += standardShotCooldown;
                    Shoot();
                }
            }
        }
        else{

        }
        updateLastSeen();
    }

    public override void Shoot()
    {
        Debug.Log(aiming + " " + sees);
        if (aiming && sees)
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
        updateLastSeen();
    }

    public override bool CheckAgro(){
        return false;
    }

    private IEnumerator Shoot(Vector3 target)
    {
        shot.SetPosition(0, muzzle.position);
        shot.SetPosition(1, target);
        shot.enabled = true;
        yield return new WaitForSeconds(0.1f);
        shot.enabled = false;
    }

    public float RandomSpread(){
        return Random.Range(-inaccuracyModifier, inaccuracyModifier);
    }

    public void setWalk()
    {
        Debug.Log("SetWalk");
        aiming = false;
        anim.SetBool("Walk", true);
        anim.SetBool("Aim", false);
    }

    public void setAim()
    {
        Debug.Log("SetAim");
        aiming = true;
        anim.SetBool("Walk", false);
        anim.SetBool("Aim", true);
    }

    public override void Move()
    {

    }

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

    public void updateLastSeen(){
        if(SeesPlayer()){
            sees = true;
            lastSeen = player;
            return;
        }
        sees = false;
    }

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
    void LookAt(Transform target) { LookAt(target.position); }
    void LookAt(Vector3 target) { 
        gameObject.transform.LookAt(new Vector3 (target.x, gameObject.transform.position.y,target.z)); 
    }
    void Destinate(Vector3 target) { agent.destination = target; }
    void Destinate(Transform target) { agent.destination = target.position; }
    float Distance(Transform target) { return Distance(target.position);}
    float Distance (Vector3 target) { return Vector3.Distance(gameObject.transform.position, target);}
}
