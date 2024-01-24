using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompactBoard
{
    DotsAndDashesGame game;
    bool[,] horizontalLines;
    bool[,] verticalLines;
    bool finished = false;
    int numberOfMoves;

    public void InitializeRepresentation(bool[,] vertical, bool[,] horizontal, DotsAndDashesGame gameReference, int linesClaimed)
    {
        horizontalLines = horizontal;
        verticalLines = vertical;
        game = gameReference;
        numberOfMoves = linesClaimed;
    }

    public CompactBoard GetBoardCopy()
    {
        CompactBoard newBoard = new CompactBoard();
        newBoard.InitializeRepresentation(verticalLines, horizontalLines, game, numberOfMoves);
        return newBoard;
    }

    public void SimulateMove(int i, int j, bool vertical)
    {
        bool[,] matrix = vertical ? verticalLines : horizontalLines;
        matrix[i,j] = true;
        if (!CheckForNewBox(i,j,vertical))
        {
            finished = true;
        }
        numberOfMoves++;
    }

    public void SimulateMove(DotsAndDashesMove move)
    {
        while(!move.IsEmpty())
        {
            (int, int, bool) nextMove = move.PopMove();
            SimulateMove(nextMove.Item1, nextMove.Item2, nextMove.Item3);
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
        AddRandomMove(move, restricted);
        return move;
    }

    private void AddRandomMove(DotsAndDashesMove move, bool restricted=false)
    {
        List<DotsAndDashesMove> verticalMoves = GetNeighborsByMatrix(true, restricted);
        List<DotsAndDashesMove> horizontalMoves = GetNeighborsByMatrix(false, restricted);
        int index = Random.Range(0, verticalMoves.Count + horizontalMoves.Count - 1);
        bool vertical = index < verticalMoves.Count;
        int indexOffset = vertical ? 0 : verticalMoves.Count;
        List<DotsAndDashesMove> moveSet = vertical ? verticalMoves : horizontalMoves;
        move.Concatenate(moveSet[index - indexOffset]);
    }

    public List<DotsAndDashesMove> GetNeighborsByMatrix(bool vertical, bool restricted=false, bool recursive=false)
    {
        bool[,] matrix = vertical ? verticalLines : horizontalLines;
        List<DotsAndDashesMove> availableSpaces = new List<DotsAndDashesMove>();
        for(int i =0; i<matrix.GetLength(0); i++)
        {
            for(int j = 0; j<matrix.GetLength(1); j++)
            {
                if(SpaceAvailable(i,j,vertical,restricted))
                {
                    AddToNeighbors(i, j, vertical, recursive, restricted, availableSpaces);
                }
            }
        }
        return availableSpaces;
    }

    private void AddToNeighbors(int i, int j, bool vertical, bool recursive, bool restricted, List<DotsAndDashesMove> availableSpaces)
    {
        DotsAndDashesMove newMove = CreateNewMove(i,j,vertical);
        if(recursive && CheckForNewBox(i,j,vertical))
        {
            CompactBoard newBoard = new CompactBoard();
            newBoard.InitializeRepresentation(verticalLines, horizontalLines, game, numberOfMoves);
            newBoard.SimulateMove(i,j,vertical);
            List<DotsAndDashesMove> additionalRoutesVertical = newBoard.GetNeighborsByMatrix(true, restricted, recursive);
            List<DotsAndDashesMove> additionalRoutesHorizontal = newBoard.GetNeighborsByMatrix(false, restricted, recursive);
            foreach(DotsAndDashesMove route in additionalRoutesVertical)
            {
                newMove = CreateNewMove(i,j,true);
                newMove.Concatenate(route);
                availableSpaces.Add(newMove);
            }
            foreach(DotsAndDashesMove route in additionalRoutesHorizontal)
            {
                newMove = CreateNewMove(i,j,false);
                newMove.Concatenate(route);
                availableSpaces.Add(newMove);
            }
        }
        else
        {
            availableSpaces.Add(newMove);
        }
    }

    private DotsAndDashesMove CreateNewMove(int i, int j, bool vertical)
    {
        DotsAndDashesMove newMove = new DotsAndDashesMove();
        newMove.AddMove((i,j,vertical));
        return newMove;
    }

    public bool SpaceAvailable(int i, int j, bool vertical, bool restricted=false)
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
        if (i < 0 || j < 0 || i >= matrix.GetLength(0) || j >= matrix.GetLength(1))
        {
            return false;
        }
        return matrix[i,j];
    }

    public bool IsFinished()
    {
        return finished;
    }

    public bool IsGameOver()
    {
        return numberOfMoves == horizontalLines.GetLength(0) * horizontalLines.GetLength(1) + verticalLines.GetLength(0) * verticalLines.GetLength(1);
    }
}
