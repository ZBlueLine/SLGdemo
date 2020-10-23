using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    private int width;
    private int height;
    private int [,] gridArry;
    private int CellSize;
    void Awake()
    {
        width = 0;
        height = 0;
        CellSize = 1;
    }
    
    void CreateText(Vector3 Mtrans, string Mtext, int Fontsize, Color color)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(this.transform);
        transform.localPosition = Mtrans;
        transform.Rotate(new Vector3(90, 0, 0));
        TextMesh textmesh = gameObject.GetComponent<TextMesh>();
        textmesh.anchor = TextAnchor.MiddleCenter;
        textmesh.alignment = TextAlignment.Left;
        textmesh.text = Mtext;
        textmesh.fontSize = Fontsize;
        textmesh.color = color;
        textmesh.GetComponent<MeshRenderer>().sortingOrder = 1;
    }
    public void Create(int hig, int wid, int cellsize)
    {
        // CreatePlane();
        // Debug.Log(transform.localPosition);
        height = hig;
        width = wid;
        CellSize = cellsize;
        gridArry = new int [height, width];
        int c, r;
        r = gridArry.GetLength(0);
        c = gridArry.GetLength(1);
        for(int i = 0; i < r; ++i)
        {
            for(int j = 0; j < c; ++j)
            {
                CreateText(GetPosition(i, j) + new Vector3(CellSize, 0, CellSize)*0.5f, gridArry[i, j].ToString(), 26, Color.white);
                Debug.DrawLine(GetPosition(i, j), GetPosition(i, j+1), Color.red, 200f);
                Debug.DrawLine(GetPosition(i, j), GetPosition(i+1, j), Color.red, 200f);
            }
        }
    }

    Vector3 GetPosition(int z, int x)
    {
        return new Vector3(x, 0, z) * CellSize;
    }

    public Vector3 WorldToGridIndex(Vector3 Position)
    {
        Vector3 GridPosition = Position;
        GridPosition -= transform.localPosition;
        GridPosition.x =  Mathf.FloorToInt(GridPosition.x / CellSize);
        GridPosition.y = 0;
        GridPosition.z =  Mathf.FloorToInt(GridPosition.z / CellSize);
        return GridPosition;
    }

}
