﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (CharacterController))]
public abstract class Enemy : MonoBehaviour {

    public float shotCooldown = 1f;
    public readonly float standardShotCooldown = 1f;

    public bool agro = false;

    private int hp = 100;

    private CharacterController charController;

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
        charController = GetComponent<CharacterController>();
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

    public abstract bool CheckAgro();

    public abstract void Move();

    public abstract void Shoot();

}
