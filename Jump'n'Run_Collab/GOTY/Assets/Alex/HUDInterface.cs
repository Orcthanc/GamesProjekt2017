using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDInterface : MonoBehaviour {
    public  Image currentHealth;
    public Text ratioText;
    private static float maxHP = 100;
    private static float ratioHealth;
    public GameObject player;
    static NewPlayerMovement otherScript;

	void Start () {
        otherScript = player.GetComponent<NewPlayerMovement>();
	}
	
	public  void updateHUD()
    {
        ratioHealth = maxHP / otherScript.Damage;
        currentHealth.rectTransform.localScale = new Vector3(ratioHealth, 1, 1);
    }

    
}
