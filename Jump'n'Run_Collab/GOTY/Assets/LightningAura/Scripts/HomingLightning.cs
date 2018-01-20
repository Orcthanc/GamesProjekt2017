using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(Collider))]
public class HomingLightning : MonoBehaviour {

    ParticleSystem particles;
    List<GameObject> objectsInRange;
    ParticleSystem.Particle[] particleArr;

	// Use this for initialization
	void Start () {
        objectsInRange = new List<GameObject>();
        particles = GetComponent<ParticleSystem>();
        particleArr = new ParticleSystem.Particle[particles.main.maxParticles];
	}

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject o in objectsInRange)
        {
            Debug.Log(o.name);
        }
        if (objectsInRange.Count > 0)
        {

            int length = particles.GetParticles(particleArr);

            for (int i = 0; i < length; i++)
            {
                Vector3 target = Closest((particleArr[i].position + GetComponentInParent<Transform>().position), objectsInRange).transform.position;
                Debug.Log(" ");
                Debug.Log(target);
                Debug.Log(particleArr[i].position);
                Debug.Log(transform.position);
                Debug.Log(transform.position + particleArr[i].position);
                Debug.Log(transform.position - target);
                Debug.Log(transform.position + particleArr[i].position - target);
                Debug.DrawRay(transform.position - particleArr[i].position, transform.position + particleArr[i].position - target);
                particleArr[i].position -= (target - transform.position - particleArr[i].position).normalized;
            }
            particles.SetParticles(particleArr, length);
        }
	}

    /// <summary>
    /// Used to get the closest object to a position
    /// </summary>
    /// <param name="pos">Position to base calculation of</param>
    /// <param name="objs">All objects to consider</param>
    /// <returns>Closest GameObject to position</returns>
    GameObject Closest(Vector3 pos, List<GameObject> objs)
    {
        GameObject tempGO = null;
        float tempF = float.MaxValue;

        foreach(GameObject o in objs)
        {
            if((pos - o.transform.position).magnitude < tempF)
            {
                tempF = (pos - o.transform.position).magnitude;
                tempGO = o;
            }
        }

        return tempGO;
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log(collider.gameObject.name);
        objectsInRange.Add(collider.gameObject);
    }

    private void OnTriggerExit(Collider collider)
    {
        objectsInRange.Remove(collider.gameObject);
    }
}
