using System.Collections;
using System.Collections.Generic;
using System;
using Global;
using UnityEngine;
using Utils;
public class Map : MonoBehaviour
{
    public int Width;
    public int Height;
    public int CellSize;
    public int CamHeight;
    public Vector3 TargetPos;
    Vector3Int AimPoint;
    List<GameObject> ClickMark;
    public Material SelectBlockColor;
    int [,] gridArry;
    List<List<GameObject>> ObjectArry;
    List<GameObject> Texts;
    bool Press;
    Vector3 LastPressLocation;
    GameObject SelectedChara;

    void Awake()
    {
        ClickMark = new List<GameObject>();
        gridArry = new int [Width, Height];
        Texts = new List<GameObject>();
        LastPressLocation = new Vector3();
        ObjectArry = new List<List<GameObject>>();
        for(int i = 0; i < Width; ++i)
        {
            ObjectArry.Add(new List<GameObject>());
            for(int j = 0; j < Height; ++j)
            {
                ObjectArry[i].Add(null);
            }
        }
    }
    void Start()
    {
        
        Press = false;
        // float x = Width/2f;
        // float z = Height/2f;
        // float y = CamHeight + transform.localPosition.y;
        // x *= CellSize;
        // z *= CellSize;
        // x += transform.localPosition.x;
        // z += transform.localPosition.z;
        //camtrans.localPosition = new Vector3(x, y, z);
    }
    void Update()
    {   
        if(Input.GetMouseButtonUp(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray Mray = Camera.main.ScreenPointToRay(mousePosition);
            Debug.DrawRay(Mray.origin, Mray.direction * 1000, Color.white, 100, true);
            RaycastHit Mhit;
            if(Physics.Raycast(Mray, out Mhit))
            {
                if(Mhit.collider.gameObject.tag == "Map")
                {
                    //清空文字显示
                    TargetPos = Mhit.point;

                    //计算点击的格子左下角的index
                    AimPoint = WorldToGridIndex(TargetPos);
                    int x , y;
                    x = AimPoint.x;
                    y = AimPoint.z;
                    int value = GetGridValue(new Vector2Int(x, y));
                    ClearText();
                    ReSetGridValue();
                    if(value == GlobalVar.IsChara)
                    {
                        Debug.Log("have Chara");
                        SelectedChara = GetObject(new Vector2Int(x, y));
                        CharaController selectchara = SelectedChara.gameObject.GetComponent<CharaController>();
                        selectchara.ShowMoveRange();
                        selectchara.Showproperties();
                    }
                    else if(value == GlobalVar.PrepareMove)
                    {
                        Debug.Log("PrepareMove");
                        CharaController m_Controller = SelectedChara.GetComponent<CharaController>();
                        Vector2Int StartPos = m_Controller.GetIndex();
                        Queue<Vector2Int> Path =  UtilsTool.BFS(Height, Width, ref gridArry, StartPos, new Vector2Int(x, y));
                        while(Path.Count != 0)
                        {
                            m_Controller.AddMoveAction(Path.Dequeue());
                        }
                        Debug.Log("Move");
                        m_Controller.Move();
                        SetGridValue(new Vector2Int(StartPos.x, StartPos.y), GlobalVar.EmptyLcation);
                        GameObject Tmp = ObjectArry[StartPos.x][StartPos.y];
                        ObjectArry[StartPos.x][StartPos.y] =  ObjectArry[x][y];
                        ObjectArry[x][y] = Tmp;
                        ShowValue();
                    }
                    else if(value == -1)
                    {
                        Debug.Log("error Click range!!!");
                    }
                    else 
                    {
                        AddClickMark(UtilsTool.CreatePlane(AimPoint, CellSize, transform, SelectBlockColor));
                        ShowValue();
                    }
                }
            }
        }
    }

    void ReSetGridValue()
    {
        if(ClickMark.Count != 0)
        {
            foreach(GameObject c in ClickMark)
            {
                Vector3 Position = c.transform.position;
                Position = WorldToGridIndex(Position);
                Vector2Int CLocaiton =  new Vector2Int((int)Position.x, (int)Position.z);
                //重置格子的值
                if(GetGridValue(CLocaiton)== GlobalVar.PrepareMove)
                {
                    SetGridValue(CLocaiton, GlobalVar.EmptyLcation);
                }
                UnityEngine.Object.Destroy(c);
            }
            ClickMark.Clear();
        }
    }

    public bool AddClickMark(GameObject Mark)
    {
        ClickMark.Add(Mark);
        return true;
    }

    public bool AddObject(Vector2Int Index, GameObject NewObject)
    {
        int x, y;
        x = Index.x;
        y = Index.y; 
        if(x < 0||x >= Width||y < 0||y >= Height)
            return false;
        ObjectArry[x][y] = NewObject;
        return true;
    }
     
    public GameObject GetObject(Vector2Int Index)
    {
        return ObjectArry[Index.x][Index.y];
    }

    public bool SetGridValue(Vector2Int Index, int value)
    {
        int x, y;
        x = Index.x;
        y = Index.y;
        if(x < 0||x >= Width||y < 0||y >= Height)
        {
            Debug.Log("error Index in SetGridValue!!!!!!!");
            return false;
        }
        gridArry[x, y] = value;
        return true;
    }

    public int GetGridValue(Vector2Int Index)
    {
        int x, y;
        x = Index.x;
        y = Index.y;
        if(x < 0||x >= Width||y < 0||y >= Height)
        {
            Debug.Log("error Index in GetGridValue!!!!!!!");
            return -1;
        }
        return gridArry[x, y];
    }
    Vector3 GetPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * CellSize;
    }
    public void ShowValue()
    {
        int x, y;
        x = Width;
        y = Height;
        for(int i = 0; i < x; ++i)
        {
            for(int j = 0; j < y; ++j)
            {
                CreateText(GetPosition(i, j) + new Vector3(CellSize, 0, CellSize)*0.5f, GetGridValue(new Vector2Int(i, j)).ToString(), 5, Color.white);
                Debug.DrawLine(GetPosition(i, j), GetPosition(i, j+1), Color.red, 200f);
                Debug.DrawLine(GetPosition(i, j), GetPosition(i+1, j), Color.red, 200f);
            }
        }
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

    public Vector3Int WorldToGridIndex(Vector3 Position)
    {
        Vector3Int GridPosition = new Vector3Int();
        Position -= transform.position;
        GridPosition.x =  Mathf.FloorToInt(Position.x / CellSize);
        GridPosition.y = 0;
        GridPosition.z =  Mathf.FloorToInt(Position.z / CellSize);
        return GridPosition;
    }
    public void CreateText(Vector3 Mtrans, string Mtext, int Fontsize, Color color)
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
        Texts.Add(MygameObject);
    }

    public void ClearText()
    {
        foreach(GameObject c in Texts)
        {
            Destroy(c);
        }
        Texts.Clear();
    }
}
 