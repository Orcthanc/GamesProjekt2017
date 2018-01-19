using UnityEngine;
using UnityEngine.AI;
using System.Security;

public class RobotBehaviour : MonoBehaviour{

    private Animator anim;
    private NavMeshAgent agent;
    private Transform player;

    private string[] names;
    int i;

    public void Start()
    {
        anim = gameObject.GetComponentInChildren<Animator>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
        agent.autoTraverseOffMeshLink = true;
    }

    // Update is called once per frame
    public void Update()
    {
        i++;
        anim.Update(Time.deltaTime);

        Destinate(player);
        LookAt(agent.nextPosition);

        if (agent.speed > 0.0f){
            setWalk();
        }
    

        if(Distance(player) < 5.0f){

            Destinate(gameObject.transform);
            setAim();
            LookAt(player);

            if ((i %= 200) == 0)
            {
                anim.SetTrigger("Shoot");
            }
        }

   	}

    public void setWalk(){
        anim.SetBool("Walk", true);
        anim.SetBool("Aim", false);
    }

    public void setAim(){
        anim.SetBool("Walk", false);
        anim.SetBool("Aim", true);
    }

  //  override
    public bool CheckAgro(){
        return false;
    }

   // override
    public void Move() {
        
    }

    //override
    public void Shoot() {
        
    }

    bool SeesPlayer(){
        return false;
    }

    void LookAt(Transform target) { LookAt(target.position); }
    void LookAt(Vector3 target) { 
        gameObject.transform.LookAt(new Vector3 (target.x, gameObject.transform.position.y,target.z)); 
    }
    void Destinate(Vector3 target) { agent.destination = target; }
    void Destinate(Transform target) { agent.destination = target.position; }
    float Distance(Transform target) { return Distance(target.position);}
    float Distance (Vector3 target) { return Vector3.Distance(gameObject.transform.position, target);}

}
