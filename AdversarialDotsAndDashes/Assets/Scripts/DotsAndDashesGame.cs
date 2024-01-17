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
    int[,] gameMatrix;
    GameObject[,] gameMatrixObjects;
    int turnPlayer;

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
        if ((i + 1 >= gameMatrixObjects.GetLength(0) && !vertical) || j >= gameMatrixObjects.GetLength(1))
        {
            return;
        }
        int verticalMultiplier = vertical ? 1 : 0;
        Vector3 offset = (1-verticalMultiplier)*Vector3.right + verticalMultiplier*Vector3.up;
        GameObject line = Instantiate(linePrefab, position + offset, Quaternion.Euler(0, 0, 90 * (1-verticalMultiplier)));
        //gameMatrixObjects[i,j] = line;
    }

    public void ChangeLineOwnership(GameObject line)
    {
        Renderer lineRenderer = line.GetComponent<Renderer>();
        lineRenderer.material.SetColor("_BaseColor", playerColors[turnPlayer]);
    }
}
