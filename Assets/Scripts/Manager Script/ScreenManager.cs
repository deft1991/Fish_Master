using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;

    private GameObject _currentScreen;

    [Header("Screens")] public GameObject endScreen;
    public GameObject gameScreen;
    public GameObject mainScreen;
    public GameObject returnScreen;

    [Header("Buttons")] public Button lengthButton;
    public Button strengthButton;
    public Button offlineEarningsButton;

    [Header("Texts")] public Text gameScreenMoneyText;
    public Text lengthCostText;
    public Text lengthValueText;
    public Text strengthCostText;
    public Text strengthValueText;
    public Text offlineEarningsCostText;
    public Text offlineEarningsCostValueText;
    public Text endScreenMoneyText;
    public Text returnScreenMoneyText;

    private int _gameCount;

    // Start is called before the first frame update
    void Awake()
    {
        if (ScreenManager.instance)
        {
            Destroy(base.gameObject);
        }
        else
        {
            ScreenManager.instance = this;
        }
        _currentScreen = mainScreen;
    }

    // Update is called once per frame
    void Start()
    {
        CheckIdles();
        UpdateTexts();
    }

    public void ChangeScreens(Screens screen)
    {
        _currentScreen.SetActive(false);
        switch (screen)
        {
            case Screens.MAIN:
                _currentScreen = mainScreen;
                UpdateTexts();
                CheckIdles();
                break;
            case Screens.GAME:
                _currentScreen = gameScreen;
                _gameCount++;
                break;
            case Screens.END:
                _currentScreen = endScreen;
                SetEndScreenMoney();
                break;
            case Screens.RETURN:
                _currentScreen = returnScreen;
                SetReturnScreenMoney();
                break;
        }

        _currentScreen.SetActive(true);
    }

    public void SetEndScreenMoney()
    {
        endScreenMoneyText.text = "$ " + IdleManager.instance.totalGain;
    }

    public void SetReturnScreenMoney()
    {
        returnScreenMoneyText.text = "$ " + IdleManager.instance.totalGain + " gained while waiting!";
    }

    private void UpdateTexts()
    {
        gameScreenMoneyText.text = "$ " + IdleManager.instance.wallet;
        lengthCostText.text = "$ " + IdleManager.instance.lengthCost;
        lengthValueText.text = -IdleManager.instance.length + " m";
        strengthCostText.text = "$ " + IdleManager.instance.strengthCost;
        strengthValueText.text = IdleManager.instance.strength + " fishes.";
        offlineEarningsCostText.text = "$ " + IdleManager.instance.offlineEarningsCost;
        offlineEarningsCostValueText.text = "$ " + IdleManager.instance.offlineEarnings + "/min";
    }

    private void CheckIdles()
    {
        int lengthCost = IdleManager.instance.lengthCost;
        int strengthCost = IdleManager.instance.strengthCost;
        int offlineEarningsCost = IdleManager.instance.offlineEarningsCost;
        int wallet = IdleManager.instance.wallet;

        lengthButton.interactable = wallet > lengthCost;
        strengthButton.interactable = wallet > strengthCost;
        offlineEarningsButton.interactable = wallet > offlineEarningsCost;
    }
}