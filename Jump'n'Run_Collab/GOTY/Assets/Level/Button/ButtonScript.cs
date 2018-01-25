
using UnityEngine;
using System;

public abstract class ButtonScript : Enemy
{

    public new int Damage
    {
        get
        {
            return hp;
        }
        set
        {
            push();
        }
    }

    Animator animator;
    bool pushedState;
    protected bool pushTrigger;

    public bool timed;
    public float duration;
    int timedDuration;
    bool ticking;

    void Start()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        animator.Update(Time.deltaTime);
        checkPushed();
        actionTrigger();

        if(timed && ticking && Environment.TickCount > timedDuration){
            unpush();
            ticking = false;
        }
    }

    void actionTrigger(){
        if(pushTrigger){
            Debug.Log("Trigger");
            pushTrigger = false;
            this.Action();
        }
    }

    void checkPushed(){
        if(Input.GetButtonDown("Submit")){
            if(Physics.OverlapSphere(gameObject.transform.position, 1f, 1 << 9).Length != 0){
                push();
            }
        }
    }

    public abstract void Action(); 

    public bool pushed(){
        return pushedState;
    }

    private void pushTimed(float _duration)
    {
        ticking = true;
        timedDuration = Environment.TickCount * (int)(_duration * 1000);
    }

    private void unpush(){
        pushedState = false;
        animator.SetTrigger("unpush");
    }

    public void push(){
        if(timed){
            pushTimed(duration);
        }

        pushTrigger = true;
        pushedState = true;
        animator.SetTrigger("push");
    }

    /// <summary>
    /// Used to check agro while not agroed (is player in sight?)
    /// </summary>
    /// <returns>Returns true if the enemy should start attacking the player</returns>
    public override bool CheckAgro() { return true; }

    /// <summary>
    /// Moves the robot
    /// </summary>
    public override void Move(){}

    /// <summary>
    /// Spawns a new projectile
    /// </summary>
    public override void Shoot(){}
}
