using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimaxPlayer : DotsAndDashesPlayer
{
    [SerializeField] bool alphaBeta;
    DotsAndDashesMove bestMove = new DotsAndDashesMove();
    int maxDepth = 3;
    Node initialNode;
    public override void Play(CompactBoard representation)
    {
        board = representation;
        initialNode = new Node();
        initialNode.SetBoard(representation);
        initialNode.SetDepth(maxDepth);
        initialNode.SetMax(true);
        Max(initialNode, maxDepth, Int32.MinValue, Int32.MaxValue);
        game.RecieveMove(initialNode.GetBestMove());
    }

    private int Max(Node node, int depth, int alpha, int beta)
    {
        if (depth == 0 || board.IsGameOver())
        {
            return node.Evaluate();
        }
        int bestValueFound = Int32.MinValue;
        List<DotsAndDashesMove> moves = GetMoves(node);
        foreach (DotsAndDashesMove move in moves)
        {
            Node childNode = CreateChild(node, move);
            int newValue = Min(childNode, depth-1, alpha, beta);
            if (newValue > bestValueFound)
            {
                node.SetChosenChild(childNode);
                bestValueFound = newValue;
            }
            alpha = (int)Mathf.Max(alpha, newValue);
            if (alphaBeta && bestValueFound >= beta)
            {
                break;
            }
        }
        return bestValueFound;
    }

    private int Min(Node node, int depth, int alpha, int beta)
    {
        if (depth == 0 || board.IsGameOver())
        {
            return node.Evaluate();
        }
        int bestValueFound = Int32.MaxValue;
        List<DotsAndDashesMove> moves = GetMoves(node);
        foreach (DotsAndDashesMove move in moves)
        {
            Node childNode = CreateChild(node, move);
            int newValue = Max(childNode, depth-1, alpha, beta);
            if (newValue < bestValueFound)
            {
                node.SetChosenChild(childNode);
                bestValueFound = newValue;
            }
            beta = (int)Mathf.Min(beta, newValue);
            if (alphaBeta && bestValueFound <= alpha)
            {
                break;
            }
        }
        return bestValueFound;
    }

    private List<DotsAndDashesMove> GetMoves(Node node)
    {
        List<DotsAndDashesMove> verticalMoves = node.GetBoard().GetNeighborsByMatrix(true, true, true);
        List<DotsAndDashesMove> horizontalMoves = node.GetBoard().GetNeighborsByMatrix(false, true, true);
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
