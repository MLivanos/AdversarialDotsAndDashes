using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject inGameUI;
    DotsAndDashesGame dotsAndDashesScript;
    GameObject dotsAndDashesObject;

    private void Start()
    {
        SwitchScreens(true);
    }
    public void Play()
    {
        SwitchScreens(false);
        /*dotsAndDashesObject = Instantiate(new GameObject);
        dotsAndDashesObject.AddComponent(typeof(DotsAndDashesGame)) as DotsAndDashesGame;
        dotsAndDashesObject.*/
    }

    public void Exit()
    {
        SwitchScreens(true);
    }

    private void SwitchScreens(bool title)
    {
        titleScreen.SetActive(title);
        inGameUI.SetActive(!title);
    }
    
}
