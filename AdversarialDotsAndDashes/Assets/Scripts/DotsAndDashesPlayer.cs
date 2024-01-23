using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DotsAndDashesPlayer : MonoBehaviour
{
    [SerializeField] int seed;
    int position;
    int opponentPosition;

    private void Start()
    {

    }

    public void Initialize(int playerPosition, int randomSeed=-1)
    {
        position = playerPosition;
        opponentPosition = 1 - position;
        if (randomSeed == -1)
        {
            return;
        }
        Random.InitState(seed);
    }

    public virtual void Play()
    {
        
    }
}