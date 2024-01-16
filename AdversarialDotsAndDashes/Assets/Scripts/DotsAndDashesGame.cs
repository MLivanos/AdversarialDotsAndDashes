using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsAndDashesGame : MonoBehaviour
{
    [SerializeField] private Vector2Int shape;
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private float spaceBetweenDots;
    int[,] gameMatrix;
    GameObject[,] gameMatrixObjects;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        gameMatrix = new int[shape.x,shape.y];
        float xOffset = (shape.x - 1) / 2.0f * spaceBetweenDots;
        float yOffset = (shape.y - 1) / 2.0f * spaceBetweenDots;
        for (int i=0; i < shape.x; i++)
        {
            for (int j=0; j < shape.y; j++)
            {
                Vector3 position = new Vector3(i*spaceBetweenDots - xOffset, j*spaceBetweenDots - yOffset, 0);
                GameObject newDot = Instantiate(dotPrefab, position, Quaternion.identity);
                AddNeighborDashes(position);
            }
        }
        Camera.main.orthographicSize = Mathf.Max(shape.x, shape.x);
    }

    private void AddNeighborDashes(Vector3 position)
    {
        if (position.x > 0)
        {
            Instantiate(linePrefab, position + Vector3.left, Quaternion.Euler(0, 0, 90));
        }
        if (position.x < shape.x - 1)
        {
            Instantiate(linePrefab, position + Vector3.right, Quaternion.Euler(0, 0, 90));
        }
        if (position.y > 0)
        {
            Instantiate(linePrefab, position + Vector3.down, Quaternion.identity);
        }
        if (position.y < shape.y - 1)
        {
            Instantiate(linePrefab, position + Vector3.up, Quaternion.identity);
        }
    }
}
