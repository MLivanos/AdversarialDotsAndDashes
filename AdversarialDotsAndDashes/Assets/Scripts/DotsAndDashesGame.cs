using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsAndDashesGame : MonoBehaviour
{
    [SerializeField] private Color[] playerColors = new Color[2];
    [SerializeField] private Color unclaimedColor;
    [SerializeField] private Vector2Int shape;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private float spaceBetweenDots;
    private GameObject[,] gameMatrixObjectsHorizontal;
    private GameObject[,] gameMatrixObjectsVertical;
    private List<GameObject> instantiatedBoxes = new List<GameObject>();
    bool[,] claimedBoxes;
    private Vector2Int playerScores = Vector2Int.zero;
    int turnPlayer = 1;
    int nLinesClaimed;
    bool switchPlayer = false;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckForLine();
        }
        if (switchPlayer)
        {
            ChangeTurnPlayer();
        }
        if (Input.GetKeyDown("r"))
        {
            Restart();
        }
    }

    private void CheckForLine()
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, 10f))
        {
            if (raycastHit.transform.tag == "line")
            {
                ChangeLineOwnership(raycastHit.transform.gameObject);
            }
        }
    }

    private void ChangeTurnPlayer()
    {
        turnPlayer = 1 - turnPlayer;
        switchPlayer = false;
    }

    public void Initialize()
    {
        nLinesClaimed = 0;
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
            }
        }
        ClearLines(gameMatrixObjectsHorizontal);
        ClearLines(gameMatrixObjectsVertical);
        Camera.main.orthographicSize = Mathf.Max(shape.x, shape.y);
    }

    private void Restart()
    {
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
            ClaimBox((minorLine1.transform.position + minorLine2.transform.position) / 2);
            return true;
        }
        return false;
    }

    private bool IndicesInBounds(Vector2Int majorIndices, Vector2Int minorIndices1, Vector2Int minorIndices2, GameObject[,] majorAxis, GameObject[,] minorAxis)
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
}
