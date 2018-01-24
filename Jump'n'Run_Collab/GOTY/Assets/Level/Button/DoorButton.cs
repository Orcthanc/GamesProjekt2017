using UnityEngine;
public class DoorButton : ButtonScript
{
    public GameObject door;

    public override void Action(){
        Debug.Log("DESTROYING DOOR");
        Destroy(door);
    }

    void actionTrigger()
    {
        if (pushTrigger)
        {
            Debug.Log("Trigger");
            pushTrigger = false;
            Action();
        }
    }
}

