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
    GameObject[,] gameMatrixObjects;
    int[,] gameMatrix;
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
        gameMatrix = new int[shape.x,shape.y];
        gameMatrixObjects = new GameObject[shape.x,shape.y];
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
        CheckForNulls(gameMatrixObjects);
        Camera.main.orthographicSize = Mathf.Max(shape.x, shape.x);
    }

    private void AddLine(Vector3 position, int i, int j, bool vertical)
    {
        if ((i + 1 >= gameMatrixObjects.GetLength(0) && !vertical) || j >= gameMatrixObjects.GetLength(1))
        {
            return;
        }
        int verticalMultiplier = vertical ? 1 : 0;
        int xGridPosition = 1 - verticalMultiplier + i;
        int yGridPosition = j - verticalMultiplier;
        Vector3 offset = (1-verticalMultiplier)*Vector3.right + verticalMultiplier*Vector3.up;
        GameObject line = Instantiate(linePrefab, position + offset, Quaternion.Euler(0, 0, 90 * (1-verticalMultiplier)));
        gameMatrixObjects[xGridPosition, yGridPosition] = line;
        Line lineScript = line.GetComponent<Line>();
        lineScript.SetGridPosition(xGridPosition, yGridPosition);
    }

    public void ChangeLineOwnership(GameObject line)
    {
        Renderer lineRenderer = line.GetComponent<Renderer>();
        Line lineScript = line.GetComponent<Line>();
        lineRenderer.material.SetColor("_BaseColor", playerColors[turnPlayer]);
        Vector2Int position = lineScript.GetGridPosition();
        gameMatrix[position.x,position.y] = turnPlayer;
        Debug.Log(position);
        Debug.Log(gameMatrix[position.x,position.y]);
        CheckForNewBox(position.x, position.y);
    }

    private void CheckForNewBox(int i, int j)
    {
        if (i < shape.x && j < shape.y)
        {
            CheckBox(i,j,true,true);
        }
        if (i > 0 && j < shape.y)
        {
            CheckBox(i,j,false,true);
        }
        if (i < shape.x && j > 0)
        {
            CheckBox(i,j,true,false);
        }
        if (i > 0 && j > 0)
        {
            CheckBox(i,j,false,false);
        }
    }

    private void CheckBox(int i, int j, bool left, bool up)
    {
        int xOffset = left ? 1 : -1;
        int yOffset = up ? 1 : -1;
        if (IsSquared(i,j,xOffset,yOffset) && !IsClaimed(i,j,xOffset,yOffset))
        {
            ClaimBox(i,j,xOffset,yOffset);
        }
    }

    private bool IsSquared(int i, int j, int xOffset, int yOffset)
    {
        return gameMatrix[i+xOffset,j+yOffset] != 0 && gameMatrix[i+xOffset,j] != 0 && gameMatrix[i,j+yOffset] != 0;
    }

    private bool IsClaimed(int i, int j, int xOffset, int yOffset)
    {
        return claimedBoxes[i+xOffset,j+yOffset] && claimedBoxes[i+xOffset,j+yOffset] && claimedBoxes[i+xOffset,j+yOffset];
    }

    private void ClaimBox(int i, int j, int xOffset, int yOffset)
    {
        claimedBoxes[i,j] = true;
        claimedBoxes[i+xOffset,j] = true;
        claimedBoxes[i,j+yOffset] = true;
        claimedBoxes[i+xOffset,j+yOffset] = true;
        Debug.Log("!");
    }

    private void CheckForNulls(GameObject[,] matrix)
    {
        for(int i=0; i<matrix.GetLength(0); i++)
        {
            for(int j=0; j<matrix.GetLength(1); j++)
            {
                if (matrix[i,j] == null)
                {
                    Debug.Log(new Vector2Int(i,j));
                }
            }
        }
    }
}
