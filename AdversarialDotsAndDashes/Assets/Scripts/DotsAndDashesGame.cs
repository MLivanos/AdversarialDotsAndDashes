using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsAndDashesGame : MonoBehaviour
{
    [SerializeField] private Color[] playerColors = new Color[2];
    [SerializeField] private Vector2Int shape;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private float spaceBetweenDots;
    GameObject[,] gameMatrixObjectsHorizontal;
    GameObject[,] gameMatrixObjectsVertical;
    int[,] gameMatrixHorizontal;
    int[,] gameMatrixVertical;
    bool[,] claimedBoxes;
    int turnPlayer = 1;

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
                turnPlayer = 1 - turnPlayer;
            }
        }
    }

    public void Initialize()
    {
        gameMatrixHorizontal = new int[shape.x - 1,shape.y];
        gameMatrixVertical = new int[shape.x,shape.y - 1];
        gameMatrixObjectsHorizontal = new GameObject[shape.x - 1,shape.y];
        gameMatrixObjectsVertical = new GameObject[shape.x,shape.y - 1];
        //gameMatrixObjects = new GameObject[shape.x,shape.y];
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
        Camera.main.orthographicSize = Mathf.Max(shape.x, shape.x);
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
        if (vertical)
        {
            gameMatrixObjectsVertical[i,j-1] = line;
        }
        else
        {
            gameMatrixObjectsHorizontal[i,j] = line;
        }
        Line lineScript = line.GetComponent<Line>();
        lineScript.SetGridPosition(i, j - verticalMultiplier);
        lineScript.SetVetical(vertical);
    }

    public void ChangeLineOwnership(GameObject line)
    {
        Renderer lineRenderer = line.GetComponent<Renderer>();
        Line lineScript = line.GetComponent<Line>();
        lineRenderer.material.SetColor("_BaseColor", playerColors[turnPlayer]);
        Vector2Int position = lineScript.GetGridPosition();
        if (lineScript.IsVertical())
        {
            gameMatrixVertical[position.x,position.y] = turnPlayer;
        }
        else
        {
            gameMatrixHorizontal[position.x,position.y] = turnPlayer;
        }
        lineScript.SetClaimed(true);
        //Debug.Log(position);
        //Debug.Log(gameMatrix[position.x,position.y]);
        CheckForNewBox(position.x, position.y, lineScript.IsVertical());
    }

    private void CheckForNewBox(int i, int j, bool vertical)
    {
        if (i < shape.x && j < shape.y)
        {
            CheckBox(i,j,true, vertical);
        }
        if (i > 0 && j < shape.y)
        {
            CheckBox(i,j,false, vertical);
        }
    }

    private void CheckBox(int i, int j, bool positveOffset, bool vertical)
    {
        int offset = positveOffset ? 1 : -1;
        Vector2Int deltaAxis = vertical ? new Vector2Int(offset, 0) : new Vector2Int(0, offset);
        GameObject[,] majorAxis = vertical ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
        GameObject[,] minorAxis = !vertical ? gameMatrixObjectsVertical : gameMatrixObjectsHorizontal;
        
        Vector2Int oppositeLineCoordinates = new Vector2Int(i,j) + deltaAxis;
        Vector2Int minorLine1Coordinates = new Vector2Int(i,j) - deltaAxis * new Vector2Int((offset - 1) / 2, (offset - 1) / 2);
        Vector2Int minorLine2Coordinates = minorLine1Coordinates + new Vector2Int(Mathf.Abs(deltaAxis.y), Mathf.Abs(deltaAxis.x));

        Line oppositeLine = majorAxis[oppositeLineCoordinates.x, oppositeLineCoordinates.y].GetComponent<Line>();
        Line minorLine1 = minorAxis[minorLine1Coordinates.x, minorLine1Coordinates.y].GetComponent<Line>();
        Line minorLine2 = minorAxis[minorLine2Coordinates.x, minorLine2Coordinates.y].GetComponent<Line>();

        if (oppositeLine.IsClaimed() && minorLine1.IsClaimed() && minorLine2.IsClaimed())
        {
            ClaimBox(i,j,offset,offset);
        }
    }

    private bool IsSquared(int i, int j, int xOffset, int yOffset, bool vertical)
    {
        if (vertical)
        {
            return gameMatrixHorizontal[i+xOffset,j] != 0 && gameMatrixHorizontal[i,j] != 0 && gameMatrixVertical[i,j+yOffset] != 0;
        }
        return gameMatrixVertical[i,j] != 0 && gameMatrixVertical[i,j+yOffset] != 0 && gameMatrixHorizontal[i+xOffset,j] != 0;
    }

    private bool IsClaimed(int i, int j, int xOffset, int yOffset)
    {
        //return claimedBoxes[i+xOffset,j+yOffset] && claimedBoxes[i+xOffset,j+yOffset] && claimedBoxes[i+xOffset,j+yOffset];
        return false;
    }

    private void ClaimBox(int i, int j, int xOffset, int yOffset)
    {
        Debug.Log("claimed");
    }

    private void CheckForNulls(GameObject[,] matrix)
    {
        for(int i=0; i<matrix.GetLength(0); i++)
        {
            for(int j=0; j<matrix.GetLength(1); j++)
            {
                Line line = matrix[i,j].GetComponent<Line>();
                Debug.Log(line.GetGridPosition());
            }
        }
    }
}
