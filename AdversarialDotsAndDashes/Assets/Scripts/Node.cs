using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private CompactBoard board;
    private DotsAndDashesMove recentMove;
    private List<Node> children;
    private Node parent;
    private DotsAndDashesMove chosenMove;
    private int changeInScore;
    private int score;
    private int depth;
    private bool isMax;

    public void AddChild(Node child)
    {
        children.Add(child);
    }
    
    public int Evaluate()
    {
        if (recentMove == null)
        {
            return 0;
        }
        int sign = isMax ? 1 : -1;
        return changeInScore + sign * (recentMove.GetMove().Count - 1);
    }

    public void SimulateMove(DotsAndDashesMove move)
    {
        board.SimulateMove(move);
    }

    public int GetScore()
    {
        return score;
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
        return chosenMove;
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
        changeInScore = parentScore;
    }

    public void SetParent(Node parent_)
    {
        parent = parent_;
    }

    public void SetChosenMove(DotsAndDashesMove move)
    {
        chosenMove = move;
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
