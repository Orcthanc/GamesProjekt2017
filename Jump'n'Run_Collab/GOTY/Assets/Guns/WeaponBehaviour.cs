using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviour : MonoBehaviour {

    int bulletDamage;           //Damage the bullet deals on impact
    int fireRate;               //How much delay is added after every shot
    int fireDelay;              //Current delay. Weapon only fires when delay = 0
    float minimumAccuracy;      //Minimum size of hipfire
    float maximumAccuracy;      //Maximum size of hipfire
    float currentAccuracy;      //Current size of hipfire
    float accuracy;             //How much spread is added to hipfire after every shot
    float stability;            //How much spread is reduced when not firing
    float heatbuildup;          //How much heat is added after every shot
    float heat;                 //Current amount of heat. if heat reaches 100, weapons are disabled until heat falls to atleast 30
    float coolingRate = 1;      //How quick the gun cools. Currently UNUSED
    bool overheat;              //States if weapon is overheated or not
    float maxRange = 1000;      //Range after which raycast stops
    float recoilRight;          //Maximum recoil to the right
    float recoilLeft;           //Maximum recoil to the left
    float recoilUp;             //Maximum recoil upwards
    bool initialized = false;

    void Call()
    {
        Debug.Log("Heatlevel: "+heat);
        if (initialized == false)
        {
            {
                currentAccuracy = 0;
                heat = 0;
                setLMG();
            }
        }
        if (Input.GetButton("Mouse 1") && overheat == false && fireDelay == 0)
        {
            Debug.Log("Attempting to shoot");
            heat += heatbuildup;
            Vector3 direction = generateRandomImpact();
            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, direction, out hitInfo, maxRange, 10))
            {
                if (hitInfo.rigidbody.gameObject.GetComponent<Enemy>() != null)
                {
                    hitInfo.rigidbody.gameObject.GetComponent<Enemy>().Damage = bulletDamage;
                    Debug.Log("Hit something");
                }
            }
            getNewAccuracy();
        }
        else if (Input.GetButton("Mouse 1") == false)
        {
            currentAccuracy -= accuracy;
            if (currentAccuracy < minimumAccuracy)
            {
                currentAccuracy = minimumAccuracy;
            }
            heat -= coolingRate;
        }
        if (overheat && heat <= 30)
        {
            overheat = false;
        }
    }

    Vector3 generateRandomImpact()
    {
        float xCoordinate = Random.value * currentAccuracy;
        float yCoordinate = Random.value * currentAccuracy;
        if (Random.value > 0.5)
        {
            xCoordinate *= -1;
        }
        if (Random.value <= 0.5)
        {
            yCoordinate *= -1;
        }
        Vector3 direction = new Vector3(xCoordinate, yCoordinate, 1);
        return direction;
    }

    void getNewAccuracy()
    {
        currentAccuracy += accuracy;
        if (currentAccuracy > maximumAccuracy)
        {
            currentAccuracy = maximumAccuracy;
        }
    }

    public void setLMG()
    {
        bulletDamage = 30;
        minimumAccuracy = 0.05f;
        maximumAccuracy = 0.20f;
        accuracy = 0.01f;
        if (currentAccuracy > maximumAccuracy)
        {
            currentAccuracy = maximumAccuracy;
        }
        else if (currentAccuracy < minimumAccuracy)
        {
            currentAccuracy = minimumAccuracy;
        }
        heatbuildup = 2;
    }
}

