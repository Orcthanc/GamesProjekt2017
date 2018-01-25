using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDInterface : MonoBehaviour {
    public  Image currentHealth;

    public Image currentHeat;
    private static float maxHP = 100;
    private static float ratioHealth;
    private static float maxHeat = 100;
    private static float ratioHeat;

    public GameObject player;
    static NewPlayerMovement otherScript;

	void Start () {
        otherScript = player.GetComponent<NewPlayerMovement>();
	}
	
	public  void UpdateHUD()
    {
        //Health Stuff
        ratioHealth = otherScript.Damage / maxHP;
        currentHealth.rectTransform.localScale = new Vector3(ratioHealth, 1, 1);

        //Heat Stuff
        //ratioHeat = maxHeat / otherScript.heat;
        //currentHeat.fillAmount = ratioHeat / 2;
        //TimeReverseStuff
    }

    
}
