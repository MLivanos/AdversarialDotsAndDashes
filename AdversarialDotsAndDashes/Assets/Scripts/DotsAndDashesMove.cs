using System.Collections;
using System.Collections.Generic;

public class DotsAndDashesMove
{
    private List<(int,int,bool)> moveList = new List<(int,int,bool)>();

    public List<(int,int,bool)> GetMove()
    {
        return moveList;
    }

    public (int,int,bool) GetLastMove()
    {
        return moveList[moveList.Count - 1];
    }

    public void AddMove((int,int,bool) move)
    {
        moveList.Add(move);
    }
}
