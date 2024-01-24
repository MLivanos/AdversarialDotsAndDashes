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
    
    public int Evaluate()
    {
        return changeInScore + (isMax ? 1 : -1) * board.GetDeltaScore();
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
