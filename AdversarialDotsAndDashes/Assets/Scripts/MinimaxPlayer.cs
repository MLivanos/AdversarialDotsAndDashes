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
        board.ResetDeltaScore();
        initialNode = new Node();
        initialNode.SetBoard(representation);
        initialNode.SetDepth(maxDepth);
        initialNode.SetMax(true);
        initialNode.SetChangeInScore(0);
        Max(initialNode, maxDepth, Int32.MinValue, Int32.MaxValue);
        if (initialNode.GetChosenChild() == null)
        {
            Debug.Log("Warning: Invalid move");
            game.RecieveMove(representation.GetRandomMove());
            return;
        }
        game.RecieveMove(initialNode.GetBestMove());
    }

    private int Max(Node node, int depth, int alpha, int beta)
    {
        if (depth == 0)
        {
            return node.Evaluate();
        }
        List<DotsAndDashesMove> moves = GetMoves(node);
        if (moves.Count == 0)
        {
            return node.Evaluate();
        }
        int bestValueFound = Int32.MinValue;
        foreach (DotsAndDashesMove move_ in moves)
        {
            Node childNode = CreateChild(node, move_);
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
        if (depth == 0 || node.GetBoard().IsGameOver())
        {
            return node.Evaluate();
        }
        List<DotsAndDashesMove> moves = GetMoves(node);
        if (moves.Count == 0)
        {
            return node.Evaluate();
        }
        int bestValueFound = Int32.MaxValue;
        foreach (DotsAndDashesMove move_ in moves)
        {
            Node childNode = CreateChild(node, move_);
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
        List<DotsAndDashesMove> moves = verticalMoves.Concat(horizontalMoves).ToList().OrderByDescending(move_=>move_.GetCount()).ToList();
        return moves.Where(move_=>move_.GetCount() >= moves[0].GetCount() - 1).ToList();
    }

    private Node CreateChild(Node parent, DotsAndDashesMove thisMove)
    {
        Node childNode = new Node();
        childNode.SetBoard(parent.GetBoardCopy());
        childNode.SetMove(thisMove);
        childNode.SetParent(parent);
        childNode.SetDepth(parent.GetDepth() - 1);
        childNode.SetMax(!parent.IsMax());
        childNode.SetChangeInScore(parent.Evaluate());
        return childNode;
    }
}
