using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {

    public float speed = 2;

    public virtual void Update()
    {
        Vector3 movement = transform.forward * speed * Time.deltaTime;
        transform.position += movement;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        NewPlayerMovement temp;
        if ((temp = other.gameObject.GetComponent<NewPlayerMovement>()) != null)
            temp.Damage = 10;
        Destroy(gameObject);
    }

}
