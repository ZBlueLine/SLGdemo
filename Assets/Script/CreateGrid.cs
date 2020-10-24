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
        GameObject MygameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform Mytransform = MygameObject.transform;
        Mytransform.localPosition = Mtrans;
        Mytransform.localPosition += transform.localPosition;
        Mytransform.Rotate(new Vector3(90, 0, 0));
        TextMesh textmesh = MygameObject.GetComponent<TextMesh>();
        textmesh.anchor = TextAnchor.MiddleCenter;
        textmesh.alignment = TextAlignment.Left;
        textmesh.text = Mtext;
        textmesh.fontSize = Fontsize;
        textmesh.color = color;
        textmesh.GetComponent<MeshRenderer>().sortingOrder = 1;
        Mytransform.SetParent(this.transform);
    }
    public void Create(int wid, int hig, int cellsize)
    {
        // CreatePlane();
        Debug.Log(hig+"  creat "+wid);
        width = wid;
        height = hig;
        CellSize = cellsize;
        gridArry = new int [width, height];
        
    }

    public void ShowValue()
    {
        int c, r;
        r = gridArry.GetLength(1);
        c = gridArry.GetLength(0);
        for(int i = 0; i < r; ++i)
        {
            for(int j = 0; j < c; ++j)
            {
                CreateText(GetPosition(i, j) + new Vector3(CellSize, 0, CellSize)*0.5f, gridArry[j, i].ToString(), 5, Color.white);
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
        GridPosition -= transform.position;
        GridPosition.x =  Mathf.FloorToInt(GridPosition.x / CellSize);
        GridPosition.y = 0;
        GridPosition.z =  Mathf.FloorToInt(GridPosition.z / CellSize);
        return GridPosition;
    }

    public Vector3 GridIndexToWorld(Vector3 Position)
    {
        Vector3 WorldPosition = Position;
        WorldPosition *= CellSize;
        WorldPosition += transform.position;
        WorldPosition.x += CellSize/2f;
        WorldPosition.z += CellSize/2f;
        return WorldPosition;
    }

    public void SetGridValue(Vector2Int Index, int value)
    {
        int x, y;
        x = Index.x;
        y = Index.y;
        if(x < 0||x >= width||y < 0||y >= height)
        {
            Debug.Log("error Index in SetGridValue!!!!!!!");
            return;
        }
        Debug.Log(x+"  :   " + y);
        gridArry[x, y] = value;
    }

}
