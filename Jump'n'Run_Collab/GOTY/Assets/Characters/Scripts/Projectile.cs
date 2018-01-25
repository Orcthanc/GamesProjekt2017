using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {

    public float speed = 2;
    public int stoptime;

    void Start() {
        stoptime = Environment.TickCount + 15000;
    }

    public virtual void Update()
    {
        Vector3 movement = transform.forward * speed * Time.deltaTime;
        transform.position += movement;

        if(Environment.TickCount > stoptime) {
            Destroy(gameObject);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        NewPlayerMovement temp;
        if ((temp = other.gameObject.GetComponent<NewPlayerMovement>()) != null)
            temp.Damage = 10;
        Destroy(gameObject);
    }

}
