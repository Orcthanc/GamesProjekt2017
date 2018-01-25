using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDInterface : MonoBehaviour {
    public  Image currentHealth;
    public Image currentTime;
    public Image currentHeat;
    public Text ratioText;
    private static float maxHP = 100;
    private static float ratioHealth;
    private static float maxHeat = 100;


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
        ratioText.text = Math.Round(ratioHealth * 100).ToString();
        //Heat Stuff
        currentHeat.fillAmount = (otherScript.Heat / maxHeat)/2;
        Debug.Log(currentHeat.fillAmount);
        //TimeStuff
        currentTime.fillAmount = otherScript.getRemainingPCT()/2;
    }

    
}
