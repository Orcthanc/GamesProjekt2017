using UnityEngine;
public class SpawnButton : ButtonScript{

    public GameObject prefab;
    public GameObject posEmpty;

    public override void Action(){
        Instantiate(prefab,posEmpty.transform);
    }
}