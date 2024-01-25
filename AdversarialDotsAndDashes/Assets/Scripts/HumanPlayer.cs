using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : DotsAndDashesPlayer
{
    bool isPlaying = false;

    private void CheckForLine()
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 10f))
        {
            if (raycastHit.transform.tag == "line")
            {
                Line lineScript = raycastHit.transform.gameObject.GetComponent<Line>();
                Vector2Int position = lineScript.GetGridPosition();
                if (board.SpaceAvailable(position.x, position.y, lineScript.IsVertical()))
                {
                    move.AddMove((position.x, position.y, lineScript.IsVertical()));
                    isPlaying = false;
                    game.RecieveMove(move);
                }
            }
        }
    }

    private void Update()
    {
        if (isPlaying && Input.GetMouseButtonDown(0))
        {
            CheckForLine();
        }
    }
    public override void Play(CompactBoard representation)
    {
        isPlaying = true;
        move = new DotsAndDashesMove();
        board = representation;
        return;
    }
}
