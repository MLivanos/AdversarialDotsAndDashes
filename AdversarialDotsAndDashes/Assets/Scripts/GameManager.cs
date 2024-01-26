using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject inGameUI;
    [SerializeField] private GameObject dotsAndDashesPrefab;
    [SerializeField] private GameObject[] agents;
    private bool first;
    private GameObject opponent;
    private int[] shape = new int[2];
    private int depth;
    private DotsAndDashesGame dotsAndDashesGame;
    private GameObject dotsAndDashesGameObject;

    private void Start()
    {
        opponent = agents[0];
        SwitchScreens(true);
    }
    public void Play()
    {
        SwitchScreens(false);
        //dotsAndDashesGameObject = Instantiate(dotsAndDashesPrefab);
        //dotsAndDashesGame = dotsAndDashesGameObject.GetComponent<DotsAndDashesGame>();
        Debug.Log(shape[0]);
        Debug.Log(shape[1]);
        Debug.Log(opponent);
        Debug.Log(depth);
        Debug.Log(first);

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

    public void SetX(string value)
    {
        SetShape(Int32.Parse(value),true);
    }

    public void SetY(string value)
    {
        SetShape(Int32.Parse(value),false);
    }

    public void SetShape(int value, bool xPosition)
    {
        int index = xPosition ? 0 : 1;
        shape[index] = (int)Mathf.Max(value,2);
    }
    
    public void SetDepth(string value)
    {
        depth = (int)Mathf.Max(Int32.Parse(value), 1);
    }

    public void SetOpponent(int index)
    {
        opponent = agents[index];
    }

    public void SetPosition(bool position)
    {
        first = position;
    }
}
