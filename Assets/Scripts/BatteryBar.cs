using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryBar : MonoBehaviour 
{
    private Image imgComp;
    [SerializeField]
    private int currentBattery;
    [SerializeField]
    private bool charging;


    private const int maxBattery = 100;
    private const int chargeRate = 1;
    private const int lowBatteryThreshold = 40;

	void Awake () 
    {
        imgComp = GetComponent<Image>();
        currentBattery = maxBattery;
        charging = true;
        UpdateGraphic();

        InvokeRepeating("ChargeIfInPosition", 1, 1);
	}

    void UpdateGraphic()
    {
        imgComp.fillAmount = (float)currentBattery / maxBattery;
    }
	
    public void StartCharging(){
        charging = true;
    }

    public void StopCharging(){
        charging = false;
    }

    public void DrainBattery(){
        currentBattery -= 1;
        UpdateGraphic();
    }

    public bool LowBattery(){
        return currentBattery <= lowBatteryThreshold;
    }

    private void ChargeIfInPosition()
    {
        if (charging && currentBattery < maxBattery)
        {
            currentBattery += chargeRate;
            UpdateGraphic(); 
        }
           
    }
}

