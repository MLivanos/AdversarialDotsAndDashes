using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DotsAndDashesPlayer : MonoBehaviour
{
    [SerializeField] protected int seed;
    protected DotsAndDashesGame game;
    protected DotsAndDashesMove move;
    protected CompactBoard board;
    protected int position;
    protected int opponentPosition;

    public void Initialize(int playerPosition, DotsAndDashesGame game_, int randomSeed=-1)
    {
        game = game_;
        position = playerPosition;
        opponentPosition = 1 - position;
        if (randomSeed == -1)
        {
            return;
        }
        Random.InitState(seed);
    }

    public virtual void Play(CompactBoard representation)
    {
        return;
    }
}