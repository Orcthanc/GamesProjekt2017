using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    public float shotCooldown = 1f;
    public float standardShotCooldown = 1f;

    public bool agro = false;

    protected int hp = 100;

    protected CharacterController charController;

    /// <summary>
    /// Used to get the following hp or remove a certain amount of hp with the setter (hp -= value)
    /// </summary>
    public int Damage
    {
        get{
            return hp;
        }

        set{
            hp -= value;
        }
    }

    public void Start()
    {
    }

    public void Update()
    {
        if (agro)
        {
            Move();
            shotCooldown -= Time.deltaTime;
            if(shotCooldown < 0)
            {
                shotCooldown += standardShotCooldown;
                Shoot();
            }
        }
        else
        {
            agro = CheckAgro();
        }
    }

    /// <summary>
    /// Used to check agro while not agroed (is player in sight?)
    /// </summary>
    /// <returns>Returns true if the enemy should start attacking the player</returns>
    public abstract bool CheckAgro();

    /// <summary>
    /// Moves the robot
    /// </summary>
    public abstract void Move();

    /// <summary>
    /// Spawns a new projectile
    /// </summary>
    public abstract void Shoot();

}
