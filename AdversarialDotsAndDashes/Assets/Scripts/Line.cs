using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private int xIndex;
    private int yIndex;

    public void SetGridPosition(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(xIndex,yIndex);
    }
}
