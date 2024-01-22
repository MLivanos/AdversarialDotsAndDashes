using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private int xIndex;
    private int yIndex;
    private bool isVertical;
    private bool isClaimed;

    public void SetVetical(bool vertical)
    {
        isVertical = vertical;
    }

    public void SetGridPosition(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public void SetClaimed(bool claimed)
    {
        isClaimed = claimed;
    }

    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(xIndex,yIndex);
    }

    public bool IsVertical()
    {
        return isVertical;
    }

    public bool IsClaimed()
    {
        return isClaimed;
    }
}
