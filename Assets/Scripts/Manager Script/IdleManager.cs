using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Object = UnityEngine.Object;

public class IdleManager : MonoBehaviour
{
    [HideInInspector] // hide it in inspector
    public int length;

    [HideInInspector] public int strength;

    [HideInInspector] public int offlineEarnings;

    [HideInInspector] public int lengthCost;

    [HideInInspector] public int strengthCost;

    [HideInInspector] public int offlineEarningsCost;

    [HideInInspector] public int wallet;

    [HideInInspector] public int totalGain;

    private int[] costs = new[]
        {120, 151, 197, 250, 324, 414, 537, 687, 892, 1145, 1484, 1911, 2479, 3196, 4148, 5359, 6954, 9000, 11687};

    public static IdleManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (IdleManager.instance)
        {
            Object.Destroy(gameObject);
        }
        else
        {
            IdleManager.instance = this;
        }

        length = -PlayerPrefs.GetInt("Length", 30);
        strength = PlayerPrefs.GetInt("Strength", 3);
        offlineEarnings = PlayerPrefs.GetInt("OfflineEarnings", 3);
        lengthCost = costs[-length / 10 - 3]; // 30 / 10 - 3 = 0 for initial val
        strengthCost = costs[strength - 3]; // 30 / 10 - 3 = 0 for initial val
        offlineEarningsCost = costs[offlineEarnings - 3];
        wallet = PlayerPrefs.GetInt("Wallet", 0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // если на паузе
        if (pauseStatus)
        {
            DateTime now = DateTime.Now;
            PlayerPrefs.SetString("Date", now.ToString());
            MonoBehaviour.print(now.ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            string date = PlayerPrefs.GetString("Date", string.Empty);
            if (date != string.Empty)
            {
                DateTime dateTime = DateTime.Parse(date);
                totalGain = (int) ((DateTime.Now - dateTime).Minutes * offlineEarnings + 1.0);
                // Screen manager return
                ScreenManager.instance.ChangeScreens(Screens.RETURN);
            }
        }
    }

    private void OnApplicationQuit()
    {
        // ловим момент когда вышли и начнаем считать офлайн бабосики
        OnApplicationPause(true);
    }

    public void BuyLength()
    {
        length -= 10;
        wallet -= lengthCost;
        lengthCost = costs[-length / 10 - 3];
        PlayerPrefs.SetInt("Length", -length);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreenManager.instance.ChangeScreens(Screens.MAIN);
    }
    
    public void BuyStrength()
    {
        strength++;
        wallet -= strengthCost;
        strengthCost = costs[strength - 3];
        PlayerPrefs.SetInt("Strength", strength);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreenManager.instance.ChangeScreens(Screens.MAIN);
    }  
    
    public void BuyOfflineEarnings()
    {
        offlineEarnings++;
        wallet -= offlineEarningsCost;
        offlineEarningsCost = costs[offlineEarnings - 3];
        PlayerPrefs.SetInt("OfflineEarnings", offlineEarnings);
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreenManager.instance.ChangeScreens(Screens.MAIN);
    }    
    
    public void CollectMoney()
    {
        wallet += totalGain;
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreenManager.instance.ChangeScreens(Screens.MAIN);
    }   
    
    public void CollectDoubleMoney()
    {
        wallet += totalGain * 2;
        PlayerPrefs.SetInt("Wallet", wallet);
        ScreenManager.instance.ChangeScreens(Screens.MAIN);
    }
}