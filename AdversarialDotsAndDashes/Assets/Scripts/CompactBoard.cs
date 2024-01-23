using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompactBoard
{
    DotsAndDashesGame game;
    bool[,] horizontalLines;
    bool[,] verticalLines;
    bool finished = false;

    public void InitializeRepresentation(bool[,] horizontal, bool[,] vertical, DotsAndDashesGame gameReference)
    {
        horizontalLines = horizontal;
        verticalLines = vertical;
        game = gameReference;
    }

    public void SimulateMove(int i, int j, bool vertical)
    {
        bool[,] matrix = vertical ? horizontalLines : verticalLines;
        matrix[i,j] = true;
        if (!CheckForNewBox(i,j,vertical))
        {
            finished = true;
        }
    }

    public bool CheckForNewBox(int i, int j, bool vertical)
    {
        bool foundBox1 = CheckBox(i,j,true, vertical);
        bool foundBox2 = CheckBox(i,j,false, vertical);
        return !(foundBox1 || foundBox2);
    }

    private bool CheckBox(int i, int j, bool positveOffset, bool vertical)
    {
        int offset = positveOffset ? 1 : -1;
        Vector2Int deltaAxis = vertical ? new Vector2Int(offset, 0) : new Vector2Int(0, offset);
        bool[,] majorAxis = vertical ? verticalLines : horizontalLines;
        bool[,] minorAxis = !vertical ? verticalLines : horizontalLines;
        
        Vector2Int oppositeLineCoordinates = new Vector2Int(i,j) + deltaAxis;
        Vector2Int minorLine1Coordinates = new Vector2Int(i,j) - deltaAxis * new Vector2Int((offset - 1) / 2, (offset - 1) / 2);
        Vector2Int minorLine2Coordinates = minorLine1Coordinates + new Vector2Int(Mathf.Abs(deltaAxis.y), Mathf.Abs(deltaAxis.x));

        if (!game.IndicesInBounds(oppositeLineCoordinates, minorLine1Coordinates, minorLine2Coordinates, vertical))
        {
            return false;
        }

        bool oppositeLine = majorAxis[oppositeLineCoordinates.x, oppositeLineCoordinates.y];
        bool minorLine1 = minorAxis[minorLine1Coordinates.x, minorLine1Coordinates.y];
        bool minorLine2 = minorAxis[minorLine2Coordinates.x, minorLine2Coordinates.y];

        if (oppositeLine && minorLine1 && minorLine2)
        {
            return true;
        }
        return false;
    }

    public DotsAndDashesMove GetRandomMove(bool restricted=false)
    {
        DotsAndDashesMove move = new DotsAndDashesMove();
        while(!finished)
        {
            AddRandomMove(move, restricted);
            (int, int, bool) recent = move.GetLastMove();
            SimulateMove(recent.Item1, recent.Item2, recent.Item3);
        }
        return move;
    }

    private void AddRandomMove(DotsAndDashesMove move, bool restricted=false)
    {
        List<(int, int)> verticalMoves = GetNeighborsByMatrix(true, restricted);
        List<(int, int)> horizontalMoves = GetNeighborsByMatrix(false, restricted);
        int index = Random.Range(0, verticalMoves.Count + horizontalMoves.Count - 1);
        bool vertical = index < verticalMoves.Count;
        int indexOffset = vertical ? 0 : verticalMoves.Count;
        List<(int, int)> moveSet = vertical ? verticalMoves : horizontalMoves;
        move.AddMove((moveSet[index - indexOffset].Item1, moveSet[index - indexOffset].Item2, vertical));
    }

    public List<(int, int)> GetNeighborsByMatrix(bool vertical, bool restricted)
    {
        bool[,] matrix = vertical ? verticalLines : horizontalLines;
        List<(int,int)> availableSpaces = new List<(int, int)>();
        for(int i =0; i<matrix.GetLength(0); i++)
        {
            for(int j = 0; j<matrix.GetLength(1); j++)
            {
                if(SpaceAvailable(i,j,vertical,restricted))
                {
                    availableSpaces.Add((i,j));
                }
            }
        }
        return availableSpaces;
    }

    private bool SpaceAvailable(int i, int j, bool vertical, bool restricted)
    {
        bool[,] majorAxis = vertical ? verticalLines : horizontalLines;
        bool[,] minorAxis = !vertical ? verticalLines : horizontalLines;
        if (majorAxis[i,j])
        {
            return false;
        }
        if (!restricted)
        {
            return true;
        }
        Vector2Int coordinates = new Vector2Int(i,j);
        Vector2Int majorOffset = vertical ? Vector2Int.right : Vector2Int.up;
        Vector2Int minorOffset = !vertical ? Vector2Int.right : Vector2Int.up;
        bool lineParallelTo = NextToClaimed(coordinates+majorOffset, majorAxis) || NextToClaimed(coordinates-majorOffset, majorAxis);
        bool lineOrthogonalTo = NextToClaimed(coordinates+minorOffset, minorAxis) || NextToClaimed(coordinates-minorOffset, minorAxis) || NextToClaimed(coordinates+minorOffset+majorOffset, minorAxis) || NextToClaimed(coordinates-minorOffset-majorOffset, minorAxis);
        return lineParallelTo || lineOrthogonalTo; 
    }

    private bool NextToClaimed(Vector2Int coordinates, bool[,] matrix)
    {
        int i = coordinates.x;
        int j = coordinates.y;
        if (i < 0 || j < 0 || i < matrix.GetLength(0) || j < matrix.GetLength(1))
        {
            return false;
        }
        return matrix[i,j];
    }
}
