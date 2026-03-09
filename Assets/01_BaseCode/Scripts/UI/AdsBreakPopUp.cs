using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdsBreakPopUp : MonoBehaviour
{
    private static AdsBreakPopUp instance;
    private UnityAction actionShowAds;
    private bool isCoolDownShowAds;
    private float timer;
    public static AdsBreakPopUp Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<AdsBreakPopUp>(PathPrefabs.ADS_BREAK_POP_UP));
        }
        return instance;
    }


    public void Show(UnityAction actionShowAds)
    {
        instance.gameObject.SetActive(true);
        this.actionShowAds = actionShowAds;
        timer = 0;
        isCoolDownShowAds = true;
    }


    private void Update()
    {
        if (isCoolDownShowAds)
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                isCoolDownShowAds = false;
                this.actionShowAds?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
