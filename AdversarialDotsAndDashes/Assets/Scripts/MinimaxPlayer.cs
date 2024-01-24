using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimaxPlayer : DotsAndDashesPlayer
{
    DotsAndDashesMove bestMove = new DotsAndDashesMove();
    int maxDepth = 2;
    Node initialNode;
    public override void Play(CompactBoard representation)
    {
        board = representation;
        initialNode = new Node();
        initialNode.SetBoard(representation);
        initialNode.SetDepth(maxDepth);
        initialNode.SetMax(true);
        Debug.Log(Max(initialNode, maxDepth));
        game.RecieveMove(initialNode.GetBestMove());
    }

    private int Max(Node node, int depth)
    {
        if (depth == 0 || board.IsGameOver())
        {
            return node.Evaluate();
        }
        int bestValueFound = Int32.MinValue;
        List<DotsAndDashesMove> moves = GetMoves(node);
        Debug.Log(moves.Count);
        foreach (DotsAndDashesMove move in moves)
        {
            Node childNode = CreateChild(node, move);
            int newValue = Min(childNode, depth-1);
            if (newValue > bestValueFound)
            {
                node.SetChosenChild(childNode);
                bestValueFound = newValue;
            }
        }
        return bestValueFound;
    }

    private int Min(Node node, int depth)
    {
        if (depth == 0 || board.IsGameOver())
        {
            return node.Evaluate();
        }
        int bestValueFound = Int32.MaxValue;
        List<DotsAndDashesMove> moves = GetMoves(node);
        Debug.Log(moves.Count);
        foreach (DotsAndDashesMove move in moves)
        {
            Node childNode = CreateChild(node, move);
            int newValue = Max(childNode, depth-1);
            if (newValue < bestValueFound)
            {
                node.SetChosenChild(childNode);
                bestValueFound = newValue;
            }
        }
        return bestValueFound;
    }

    private List<DotsAndDashesMove> GetMoves(Node node)
    {
        List<DotsAndDashesMove> verticalMoves = node.GetBoard().GetNeighborsByMatrix(true, true, false);
        List<DotsAndDashesMove> horizontalMoves = node.GetBoard().GetNeighborsByMatrix(false, true, false);
        return verticalMoves.Concat(horizontalMoves).ToList();
    }

    private Node CreateChild(Node parent, DotsAndDashesMove move)
    {
        Node childNode = new Node();
        childNode.SetBoard(parent.GetBoardCopy());
        childNode.SetMove(move);
        childNode.SetParent(parent);
        childNode.SetDepth(parent.GetDepth() - 1);
        childNode.SetMax(!parent.IsMax());
        childNode.SetChangeInScore(parent.Evaluate());
        return childNode;
    }
}
