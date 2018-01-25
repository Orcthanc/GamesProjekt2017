using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    public float shotCooldown = 1f;
    public float standardShotCooldown = 1f;

    public bool agro = false;

    protected int hp = 100;

    protected CharacterController charController;

<<<<<<< .merge_file_zLOWRa
=======
    /// <summary>
    /// Used to get the following hp or remove a certain amount of hp with the setter (hp -= value)
    /// </summary>
>>>>>>> .merge_file_rhR4GY
    public int Damage
    {
        get{
            return hp;
        }

        set{
            hp -= value;
<<<<<<< .merge_file_zLOWRa
        }
    }

    public void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    public void Update()
=======
            if (hp <= 0)
                Kill();
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    public void Start()
    {
    }

    public virtual void Update()
>>>>>>> .merge_file_rhR4GY
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

<<<<<<< .merge_file_zLOWRa
    public abstract bool CheckAgro();

    public abstract void Move();

=======
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
>>>>>>> .merge_file_rhR4GY
    public abstract void Shoot();

}
