using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsAndDashesRandomPlayer : DotsAndDashesPlayer
{
    public override void Play(CompactBoard representation)
    {
        DotsAndDashesMove move = representation.GetRandomMove();
        game.RecieveMove(move);
    }
}