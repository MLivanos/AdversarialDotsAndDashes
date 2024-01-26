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
    private bool first = true;
    private GameObject opponent;
    private int[] shape = new int[2];
    private int depth;
    private DotsAndDashesGame dotsAndDashesGame;
    private GameObject dotsAndDashesGameObject;

    private void Start()
    {
        shape[0] = 5;
        shape[1] = 6;
        depth = 3;
        opponent = agents[0];
        SwitchScreens(true);
    }
    public void Play()
    {
        SwitchScreens(false);
        dotsAndDashesGameObject = Instantiate(dotsAndDashesPrefab);
        dotsAndDashesGame = dotsAndDashesGameObject.GetComponent<DotsAndDashesGame>();
        GameObject player1Object = first ? agents[agents.GetLength(0)-1] : opponent;
        GameObject player2Object = !first ? agents[agents.GetLength(0)-1] : opponent;
        dotsAndDashesGame.Configure(new Vector2Int(shape[0], shape[1]), player1Object, player2Object, depth);
        dotsAndDashesGame.Initialize();
    }

    public void Exit()
    {
        dotsAndDashesGame.DestroyAllObjects();
        Destroy(dotsAndDashesGameObject);
        SwitchScreens(true);
    }

    public void Reset()
    {
        dotsAndDashesGame.Restart();
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

    public void HighLightPath()
    {
        dotsAndDashesGame.HighLightPath();
    }

    public void UnHighlightPath()
    {
        dotsAndDashesGame.UnHighlightPath();
    }
}
