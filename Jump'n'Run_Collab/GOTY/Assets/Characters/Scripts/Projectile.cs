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
        Destroy(gameObject);
    }

}
