using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBuffer {

    Vector3[] posBuffer;
    Quaternion[] dirBuffer;
    Quaternion[] camDirBuffer;

    int lastEl = 0;
    int firstEl = 0;
    int size;

    public int Size
    {
        get
        {
            return size;
        }
    }

    public CircleBuffer (int size)
    {
        posBuffer = new Vector3[size];
        dirBuffer = new Quaternion[size];
        camDirBuffer = new Quaternion[size];
        this.size = size;
    }

    public void Push(Vector3 vector3, Quaternion quaternion, Quaternion cam)
    {
        lastEl = (++lastEl % size);
        posBuffer[lastEl] = vector3;
        dirBuffer[lastEl] = quaternion;
        camDirBuffer[lastEl] = cam;
        if(lastEl == firstEl)
        {
            firstEl = ++firstEl % size;
        }
    }

    public bool Pop(out Vector3 vector3, out Quaternion quaternion, out Quaternion camDir)
    {
        if(lastEl == firstEl)
        {
            vector3 = Vector3.forward;
            quaternion = Quaternion.identity;
            camDir = Quaternion.identity;
            return false;
        }
        else
        {
            vector3 = posBuffer[lastEl];
            quaternion = dirBuffer[lastEl];
            camDir = camDirBuffer[lastEl];
            lastEl = (lastEl + size - 1) % size;
            return true;
        }
    }

    public float getRemainingPct()
    {
        return ((float)(getRemainingSize())) / ((float)(size));
    }

    public int getRemainingSize()
    {
        return (lastEl - size + 500) % size;
    }

}
