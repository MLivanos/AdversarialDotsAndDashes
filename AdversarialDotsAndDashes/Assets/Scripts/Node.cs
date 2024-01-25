using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private CompactBoard board;
    private DotsAndDashesMove recentMove;
    private List<Node> children;
    private Node parent;
    private Node chosenChild;
    private int changeInScore;
    private int score;
    private int depth;
    private bool isMax;

    public void AddChild(Node child)
    {
        children.Add(child);
    }
    
    // ISSUE: changeInScore is wrong
    public int Evaluate()
    {
        /*if (depth == 0 && (isMax ? -1 : 1) * board.GetDeltaScore() != 0)
        {
            Debug.Log("======");
            Debug.Log(changeInScore);
            Debug.Log((isMax ? -1 : 1) * board.GetDeltaScore());
        }*/
        return changeInScore + (isMax ? -1 : 1) * board.GetDeltaScore();
        //return (isMax ? -1 : 1) * board.GetDeltaScore();
    }

    public void SimulateMove(DotsAndDashesMove move)
    {
        board.SimulateMove(move);
    }

    public int GetScore()
    {
        return changeInScore;
    }

    public int GetDepth()
    {
        return depth;
    }

    public CompactBoard GetBoardCopy()
    {
        return board.GetBoardCopy();
    }

    public CompactBoard GetBoard()
    {
        return board;
    }

    public DotsAndDashesMove GetMove()
    {
        return recentMove;
    }

    public DotsAndDashesMove GetBestMove()
    {
        return chosenChild.GetMove();
    }

    public bool IsMax()
    {
        return isMax;
    }

    public void SetBoard(CompactBoard board_)
    {
        board = board_;
    }

    public void SetChangeInScore(int parentScore)
    {
        if (parentScore > 0)
        {
            Debug.Log(depth += 1);
        }
        changeInScore = parentScore;
    }

    public void SetParent(Node parent_)
    {
        parent = parent_;
    }

    public void SetChosenChild(Node thisChildNode)
    {
        chosenChild = thisChildNode;
    }

    public void SetDepth(int depth_)
    {
        depth = depth_;
    }

    public void SetMove(DotsAndDashesMove move_)
    {
        recentMove = move_;
        SimulateMove(recentMove);
    }

    public void SetMax(bool maximizing)
    {
        isMax = maximizing;
    }
}
