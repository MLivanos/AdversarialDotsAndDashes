using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsAndDashesGame : MonoBehaviour
{
    [SerializeField] private Color[] playerColors = new Color[2];
    [SerializeField] private Color unclaimedColor;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject boxPrefab;
    private float spaceBetweenDots=2.0f;
    private GameObject[] playersPrefab = new GameObject[2];
    private Vector2Int shape;
    private DotsAndDashesPlayer[] players = new DotsAndDashesPlayer[2];
    private GameObject[,] gameMatrixObjectsHorizontal;
    private GameObject[,] gameMatrixObjectsVertical;
    private List<GameObject> dots;
    private List<DotsAndDashesMove> linesOnThePath;
    private List<GameObject> instantiatedBoxes = new List<GameObject>();
    bool[,] claimedBoxes;
    private Vector2Int playerScores = Vector2Int.zero;
    int turnPlayer = 1;
    int nLinesClaimed;
    bool switchPlayer = true;

    private void Update()
    {
        if (switchPlayer)
        {
            ChangeTurnPlayer();
        }
        if (Input.GetKeyDown("r"))
        {
            Restart();
        }
    }

    public void Configure(Vector2Int gameShape, GameObject player1, GameObject player2)
    {
        shape = gameShape;
        playersPrefab[0] = player1;
        playersPrefab[1] = player2;
    }

    public void HighLightPath()
    {
        DotsAndDashesPlayer agent = players[0] is MinimaxPlayer ? players[0] : players[1]; 
        int agentPlayerID = agent.GetPosition();
        linesOnThePath = agent.GetHighlightedPath();
        int totalNumberOfMoves = linesOnThePath.Count - 1;
        float currentOpacity = 1;
        float decrement = (float)totalNumberOfMoves / (float)(totalNumberOfMoves + 1);
        int currentPlayer = agentPlayerID;
        foreach(DotsAndDashesMove moveSet in linesOnThePath.GetRange(1, linesOnThePath.Count-1))
        {
            currentOpacity *= (decrement*decrement);
            Debug.Log((byte)(255*currentOpacity));
            currentPlayer = 1 - currentPlayer;
            foreach((int, int, bool) individualMove in moveSet.GetMove())
            {
                GameObject[,] matrix = individualMove.Item3 ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
                GameObject line = matrix[individualMove.Item1, individualMove.Item2];
                Renderer lineRenderer = line.GetComponent<Renderer>();
                Color32 initialColor = playerColors[currentPlayer];
                Color32 fadedColor = new Color32(initialColor.r, initialColor.g, initialColor.b, (byte)(255*currentOpacity));
                lineRenderer.material.SetColor("_BaseColor", fadedColor);
            }
        }
    }

    public void UnHighlightPath()
    {
        DotsAndDashesPlayer agent = players[0] is MinimaxPlayer ? players[0] : players[1]; 
        linesOnThePath = agent.GetHighlightedPath();
        int totalNumberOfMoves = linesOnThePath.Count;
        foreach(DotsAndDashesMove moveSet in linesOnThePath.GetRange(1, linesOnThePath.Count-1))
        {
            foreach((int, int, bool) individualMove in moveSet.GetMove())
            {
                GameObject[,] matrix = individualMove.Item3 ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
                GameObject line = matrix[individualMove.Item1, individualMove.Item2];
                Renderer lineRenderer = line.GetComponent<Renderer>();
                lineRenderer.material.SetColor("_BaseColor", unclaimedColor);
            }
        }
    }

    public void RecieveMove(DotsAndDashesMove move)
    {
        while (!switchPlayer && !move.IsEmpty())
        {
            (int, int, bool) lineClaimed = move.PopMove();
            GameObject[,] matrix = lineClaimed.Item3 ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
            GameObject line = matrix[lineClaimed.Item1,lineClaimed.Item2];
            ChangeLineOwnership(line);
        }
        if (!switchPlayer)
        {
            players[turnPlayer].Play(GetCompactRepresentation());
        }
    }

    private void ChangeTurnPlayer()
    {
        if (!IsGameOver())
        {
            turnPlayer = 1 - turnPlayer;
            switchPlayer = false;
            players[turnPlayer].Play(GetCompactRepresentation());
        }
    }

    public void Initialize()
    {
        CreatePlayers();
        nLinesClaimed = 0;
        dots = new List<GameObject>();
        gameMatrixObjectsHorizontal = new GameObject[shape.x - 1,shape.y];
        gameMatrixObjectsVertical = new GameObject[shape.x,shape.y - 1];
        claimedBoxes = new bool[shape.x,shape.y];
        float xOffset = (shape.x - 1) / 2.0f * spaceBetweenDots;
        float yOffset = (shape.y - 1) / 2.0f * spaceBetweenDots;
        for (int i=0; i < shape.x; i++)
        {
            for (int j=0; j < shape.y; j++)
            {
                Vector3 position = new Vector3(i*spaceBetweenDots - xOffset, j*spaceBetweenDots - yOffset, 0);
                AddLine(position,i,j,false);
                AddLine(position,i,j+1,true);
                GameObject newDot = Instantiate(dotPrefab, position + Vector3.back, Quaternion.identity);
                dots.Add(newDot);
            }
        }
        ClearLines(gameMatrixObjectsHorizontal);
        ClearLines(gameMatrixObjectsVertical);
        Camera.main.orthographicSize = Mathf.Max(shape.x, shape.y);
    }

    public void CreatePlayers()
    {
        players = new DotsAndDashesPlayer[2];
        GameObject player1Object = Instantiate(playersPrefab[0]);
        GameObject player2Object = Instantiate(playersPrefab[1]);
        players[0] = player1Object.GetComponent<DotsAndDashesPlayer>();
        players[1] = player2Object.GetComponent<DotsAndDashesPlayer>();
        players[0].Initialize(0,this);
        players[1].Initialize(1,this);
    }

    public void Restart()
    {
        nLinesClaimed = 0;
        turnPlayer = 1;
        switchPlayer = true;
        playerScores = Vector2Int.zero;
        foreach(GameObject box in instantiatedBoxes)
        {
            Destroy(box);
        }
        ClearLines(gameMatrixObjectsHorizontal);
        ClearLines(gameMatrixObjectsVertical);
    }

    private void ClearLines(GameObject[,] lineMatrix)
    {
        for (int i=0; i < lineMatrix.GetLength(0); i++)
        {
            for (int j=0; j < lineMatrix.GetLength(1); j++)
            {
                Renderer lineRenderer = lineMatrix[i,j].GetComponent<Renderer>();
                lineRenderer.material.SetColor("_BaseColor", unclaimedColor);
                Line lineScript = lineMatrix[i,j].GetComponent<Line>();
                lineScript.SetClaimed(false);
            }
        }
    }

    private void AddLine(Vector3 position, int i, int j, bool vertical)
    {
        if ((i + 1 >= shape.x && !vertical) || j >= shape.y)
        {
            return;
        }
        int verticalMultiplier = vertical ? 1 : 0;
        int xGridPosition = 1 - verticalMultiplier + i;
        int yGridPosition = j - verticalMultiplier;
        Vector3 offset = (1-verticalMultiplier)*Vector3.right + verticalMultiplier*Vector3.up;
        GameObject line = Instantiate(linePrefab, position + offset, Quaternion.Euler(0, 0, 90 * (1-verticalMultiplier)));
        GameObject[,] axis = vertical ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
        int axisOffset = vertical ? -1 : 0;
        axis[i,j+axisOffset] = line;
        Line lineScript = line.GetComponent<Line>();
        lineScript.SetGridPosition(i, j - verticalMultiplier);
        lineScript.SetVetical(vertical);
    }

    public void ChangeLineOwnership(GameObject line)
    {
        Line lineScript = line.GetComponent<Line>();
        if (lineScript.IsClaimed())
        {
            return;
        }
        Vector2Int position = lineScript.GetGridPosition();
        Renderer lineRenderer = line.GetComponent<Renderer>();
        lineRenderer.material.SetColor("_BaseColor", playerColors[turnPlayer]);
        lineScript.SetClaimed(true);
        nLinesClaimed += 1;
        CheckForNewBox(position.x, position.y, lineScript.IsVertical());
        if (IsGameOver())
        {
            Debug.Log("Game Over! Final Score: ");
            Debug.Log(playerScores);
        }
    }

    private void CheckForNewBox(int i, int j, bool vertical)
    {
        bool foundBox1 = CheckBox(i,j,true, vertical);
        bool foundBox2 = CheckBox(i,j,false, vertical);
        switchPlayer = !(foundBox1 || foundBox2);
    }

    private bool CheckBox(int i, int j, bool positveOffset, bool vertical)
    {
        int offset = positveOffset ? 1 : -1;
        Vector2Int deltaAxis = vertical ? new Vector2Int(offset, 0) : new Vector2Int(0, offset);
        GameObject[,] majorAxis = vertical ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
        GameObject[,] minorAxis = !vertical ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
        
        Vector2Int oppositeLineCoordinates = new Vector2Int(i,j) + deltaAxis;
        Vector2Int minorLine1Coordinates = new Vector2Int(i,j) - deltaAxis * new Vector2Int((offset - 1) / 2, (offset - 1) / 2);
        Vector2Int minorLine2Coordinates = minorLine1Coordinates + new Vector2Int(Mathf.Abs(deltaAxis.y), Mathf.Abs(deltaAxis.x));

        if (!IndicesInBounds(oppositeLineCoordinates, minorLine1Coordinates, minorLine2Coordinates, majorAxis, minorAxis))
        {
            return false;
        }

        Line oppositeLine = majorAxis[oppositeLineCoordinates.x, oppositeLineCoordinates.y].GetComponent<Line>();
        Line minorLine1 = minorAxis[minorLine1Coordinates.x, minorLine1Coordinates.y].GetComponent<Line>();
        Line minorLine2 = minorAxis[minorLine2Coordinates.x, minorLine2Coordinates.y].GetComponent<Line>();

        if (oppositeLine.IsClaimed() && minorLine1.IsClaimed() && minorLine2.IsClaimed())
        {
            ClaimBox((minorLine1.transform.position + minorLine2.transform.position + Vector3.forward) / 2);
            return true;
        }
        return false;
    }

    public bool IndicesInBounds(Vector2Int majorIndices, Vector2Int minorIndices1, Vector2Int minorIndices2, bool vertical)
    {
        GameObject[,] majorAxis = vertical ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
        GameObject[,] minorAxis = !vertical ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
        return IndicesInBounds(majorIndices, minorIndices1, minorIndices2, majorAxis, minorAxis);
    }

    public bool IndicesInBounds(Vector2Int majorIndices, Vector2Int minorIndices1, Vector2Int minorIndices2, GameObject[,] majorAxis, GameObject[,] minorAxis)
    {
        bool majorOutOfBounds = majorIndices.x >= 0 && majorIndices.y >= 0 && majorIndices.x < majorAxis.GetLength(0) && majorIndices.y < majorAxis.GetLength(1);
        bool minor1OutOfBounds = minorIndices1.x >= 0 && minorIndices1.y >= 0 && minorIndices1.x < minorAxis.GetLength(0) && minorIndices1.y < minorAxis.GetLength(1);
        bool minor2OutOfBounds = minorIndices2.x >= 0 && minorIndices2.y >= 0 && minorIndices2.x < minorAxis.GetLength(0) && minorIndices2.y < minorAxis.GetLength(1);
        return majorOutOfBounds && minor1OutOfBounds && minor2OutOfBounds;
    }

    private void ClaimBox(Vector3 boxPosition)
    {
        playerScores[turnPlayer] += 1;
        Debug.Log(playerScores);
        GameObject newBox = Instantiate(boxPrefab);
        instantiatedBoxes.Add(newBox);
        newBox.transform.localScale = new Vector3(spaceBetweenDots, spaceBetweenDots, 0);
        newBox.transform.position = boxPosition;
        Renderer boxRenderer = newBox.GetComponent<Renderer>();
        Color32 semiTransparentColor = playerColors[turnPlayer];
        semiTransparentColor.a = 50;
        boxRenderer.material.SetColor("_Color", semiTransparentColor);
    }

    private bool IsGameOver()
    {
        return nLinesClaimed == gameMatrixObjectsHorizontal.GetLength(0) * gameMatrixObjectsHorizontal.GetLength(1) + gameMatrixObjectsVertical.GetLength(0) * gameMatrixObjectsVertical.GetLength(1);
    }

    public CompactBoard GetCompactRepresentation()
    {
        CompactBoard representation = new CompactBoard();
        bool[,] claimedVertical = GetClaimedMatrix(true);
        bool[,] claimedHorizontal = GetClaimedMatrix(false);
        representation.InitializeRepresentation(claimedVertical, claimedHorizontal, this, nLinesClaimed);
        return representation;
    }

    private bool[,] GetClaimedMatrix(bool vertical)
    {
        GameObject[,] gameObjectMatrix = vertical ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
        bool[,] matrix = new bool[gameObjectMatrix.GetLength(0),gameObjectMatrix.GetLength(1)];
        for (int i=0; i<matrix.GetLength(0); i++)
        {
            for(int j=0; j<matrix.GetLength(1); j++)
            {
                Line lineScript = gameObjectMatrix[i,j].GetComponent<Line>();
                if (lineScript.IsClaimed())
                {
                    matrix[i,j] = true;
                }
            }
        }
        return matrix;
    }

    public void DestroyAllObjects()
    {
        Restart();
        players[0].Destruct();
        players[1].Destruct();
        DestroyMatrix(true);
        DestroyMatrix(false);
        foreach(GameObject dot in dots)
        {
            Destroy(dot);
        }
    }

    private void DestroyMatrix(bool vertical)
    {
        GameObject[,] matrix = vertical ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
        for(int i=0; i<matrix.GetLength(0); i++)
        {
            for(int j=0; j<matrix.GetLength(1); j++)
            {
                Destroy(matrix[i,j]);
            }
        }
    }
}
