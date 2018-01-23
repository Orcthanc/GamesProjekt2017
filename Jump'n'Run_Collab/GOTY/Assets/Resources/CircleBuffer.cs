using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBuffer {

    Vector3[] posBuffer;
    Quaternion[] dirBuffer;

    int lastEl = 0;
    int firstEl = 0;
    int size;

    public CircleBuffer (int size)
    {
        posBuffer = new Vector3[size];
        dirBuffer = new Quaternion[size];
        this.size = size;
    }

    public void Push(Vector3 vector3, Quaternion quaternion)
    {
        lastEl = (++lastEl % size);
        posBuffer[lastEl] = vector3;
        dirBuffer[lastEl] = quaternion;
        if(lastEl == firstEl)
        {
            firstEl = ++firstEl % size;
        }
    }

    public bool Pop(out Vector3 vector3, out Quaternion quaternion)
    {
        if(lastEl == firstEl)
        {
            vector3 = Vector3.forward;
            quaternion = Quaternion.identity;
            return false;
        }
        else
        {
            Debug.Log(lastEl);
            vector3 = posBuffer[lastEl];
            quaternion = dirBuffer[lastEl];
            lastEl = (lastEl + size - 1) % size;
            return true;
        }
    }

}
