using System.Collections;
using System.Collections.Generic;

public class DotsAndDashesMove
{
    private List<(int,int,bool)> moveList = new List<(int,int,bool)>();

    public List<(int,int,bool)> GetMove()
    {
        return moveList;
    }

    public (int,int,bool) PopMove()
    {
        (int,int,bool) move = moveList[0];
        moveList.RemoveAt(0);
        return move;
    }

    public (int,int,bool) GetLastMove()
    {
        return moveList[moveList.Count - 1];
    }

    public void AddMove((int,int,bool) move)
    {
        moveList.Add(move);
    }

    public bool IsEmpty()
    {
        return moveList.Count == 0;
    }

    public void Concatenate(DotsAndDashesMove other, bool reverse=false)
    {
        List<(int,int,bool)> front = reverse ? other.GetMove() : moveList;
        List<(int,int,bool)> back = !reverse ? other.GetMove() : moveList;
        foreach((int,int,bool) element in back)
        {
            front.Add(element);
        }
        moveList = front;
    }
}
