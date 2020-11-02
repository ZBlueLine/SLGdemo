using System.Collections;
using System.Collections.Generic;
using System;
using Global;
using UnityEngine;
using Utils;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class Map : MonoBehaviour
{
    //Property
    public int Width;
    public int Height;

    public int CellSize;
    int charanumber;
    int enemynumber;
    
    public int CharaNumber{get => charanumber; set => charanumber = value;}
    public int EnemyNumber{get => enemynumber; set => enemynumber = value;}

    public Vector3 TargetPos;
    Vector3Int aimpoint;
    List<GameObject> ClickMark;
    public Material SelectBlockColor;
    
    GridStatus [,] GridArry;
    public GridStatus[,] gridArry { get => GridArry; set => GridArry = value; }
    GridStatus [,] attackArry;
    public GridStatus[,] AttackArry { get => attackArry; set => attackArry = value; }
    List<List<GameObject>> objectarry;
    public List<List<GameObject>> ObjectArry { get => objectarry; set => objectarry = value; }
    public Vector3Int AimPoint { get => aimpoint; set => aimpoint = value; }

    List<GameObject> Texts;

    //Status
    bool preacttk;
    public bool PrepareAttack { get => preacttk; set => preacttk = value; }
    bool moveing;
    bool enemyTurn;
    bool playerTurn;
    bool inAttack;
    public bool InAttack { get => inAttack; set => inAttack = value; }
    public bool EnemyTurn { get => enemyTurn; set => enemyTurn = value; }
    public bool PlayerTurn { get => playerTurn; set => playerTurn = value; }

    //已结束行动的单位个数
    int actionend;
    int enemyactionend;
    public int ActionEnd{get => actionend; set => actionend = value;}
    public int Enemyactionend { get => enemyactionend; set => enemyactionend = value; }
    
    public bool Moveing { get => moveing; set => moveing = value; }
    bool canattack;

    CharaManager charaMange;

    Vector3 LastPressLocation;



    void Awake()
    {
        CanAttack = false;
        EnemyTurn = false;
        PlayerTurn = false;
        Moveing = false;
        GridStatus.MyMap = this;
        EnemyNumber = 0;
        CharaNumber = 0;
        ClickMark = new List<GameObject>();
        gridArry = new GridStatus [Width, Height];
        AttackArry = new GridStatus [Width, Height];
        for(int i = 0; i < Width; ++i)
            for(int j = 0; j < Height; ++j)
            {
                gridArry[i, j] = EmptyLcationstatus.getInstance();
                AttackArry[i, j] = EmptyLcationstatus.getInstance();
            }
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

    private void Start() => charaMange = transform.Find("CharaManager").gameObject.GetComponent<CharaManager>();

    GameObject ChosedEnemy;
    GameObject ChoseChara;
    CharaController EnemyCon;

    CharaController CharaCon;
    //记录上一次的ai移动以后是否可以攻击

    public bool CanAttack { get => canattack; set => canattack = value; }

    void Update()
    {   
        if(EnemyNumber == 0)
        {
            Debug.Log("Victory");
            SceneManager.LoadScene("Victory");
            return;
        }
        else if(CharaNumber == 0)
        {
            Debug.Log("GameOver");
            SceneManager.LoadScene("Lose");
            return;
        }
        Debug.Log("move  "+ Moveing);
        if(Moveing||InAttack)return;
        if(!PlayerTurn&&EnemyTurn)
        {
            if(ChosedEnemy&&CanAttack)
            {
                CharaCon.Damaged(EnemyCon.ATK);
                CanAttack = false;
            }
            ChosedEnemy = charaMange.GetAEnemy();
            if(!ChosedEnemy)
            {
                CheckTurn();
                charaMange.NewRound();
                ActionEnd = 0;
                Enemyactionend = 0;
            }
            else
            {
                ++Enemyactionend;
                EnemyCon =  ChosedEnemy.GetComponent<CharaController>();
                ChoseChara = charaMange.GetNearestChara(EnemyCon.GetIndex());
                if(ChoseChara)
                    CharaCon = ChoseChara.GetComponent<CharaController>();
                else 
                    return;

                Queue<Vector2Int> Path =  UtilsTool.BFS(Height, Width,  GridArry, EnemyCon.GetIndex(), CharaCon.GetIndex());

                int cnt = EnemyCon.MoveRange + EnemyCon.AttackRange;
                if(Path.Count < cnt)
                    CanAttack = true;
                if(Path.Count < EnemyCon.MinAttackRange)
                {
                    CanAttack = false;
                    Path = UtilsTool.UBFS(Height, Width, GridArry, EnemyCon.GetIndex(), CharaCon.GetIndex(), EnemyCon.MinAttackRange);
                }
                while(Path.Count > 0)
                {
                    if(Path.Count > EnemyCon.MoveRange)
                        Path.Dequeue();
                    else
                        EnemyCon.AddMoveAction(Path.Dequeue());
                }
                EnemyCon.AimPos = CharaCon.GetIndex();
                Moveing = true;
                EnemyCon.Move();
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            //判断有没有点到ui
            if(!EventSystem.current.IsPointerOverGameObject())
            {
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

                        //状态模式
                        GridStatus Nowstatus = GetGridValue(new Vector2Int(x, y), PrepareAttack);
                        // ClearText();
                        ReSetGridValue();
                        if(Nowstatus.statucode!=GlobalVar.FailLcation)
                            Nowstatus.Operation(x, y);
                    }
                }
            }
        }
    }

    public void CheckTurn()
    {
        if(ActionEnd == CharaNumber)
        {
            CanAttack = false;
            PlayerTurn = false;
            EnemyTurn = true;
        }
        if(Enemyactionend == EnemyNumber)
        {
            PlayerTurn = true;
            EnemyTurn = false;
        }
        charaMange.ChoseMark = 0;
    }

    public void ReSetGridValue()
    {
        PrepareAttack = false;
        if(ClickMark.Count != 0)
        {
            foreach(GameObject c in ClickMark)
            {
                Vector3 Position = c.transform.position;
                Position = WorldToGridIndex(Position);
                Vector2Int CLocaiton =  new Vector2Int((int)Position.x, (int)Position.z);
                //重置格子的值
                SetAttackValue(CLocaiton, EmptyLcationstatus.getInstance());
                if(GetGridValue(CLocaiton).statucode > GlobalVar.CannotMove)
                {
                    SetGridValue(CLocaiton, EmptyLcationstatus.getInstance());
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

    public bool SetGridValue(Vector2Int Index, GridStatus status)
    {
        int x, y;
        x = Index.x;
        y = Index.y;
        if(x < 0||x >= Width||y < 0||y >= Height)
        {
            Debug.Log("error Index in SetGridValue!!!!!!!");
            return false;
        }
        gridArry[x, y] = status;
        return true;
    }

    public bool SetAttackValue(Vector2Int Index, GridStatus status)
    {
        int x, y;
        x = Index.x;
        y = Index.y;
        if(x < 0||x >= Width||y < 0||y >= Height)
        {
            Debug.Log("error Index in SetAttackValue!!!!!!!");
            return false;
        }
        AttackArry[x, y] = status;
        return true;
    }

    public GridStatus GetGridValue(Vector2Int Index, bool Attack = false)
    {
        int x, y;
        x = Index.x;
        y = Index.y;
        if(x < 0||x >= Width||y < 0||y >= Height)
        {
            Debug.Log("error Index in GetGridValue!!!!!!!");
            return ErrorStatus.getInstance();
        }
        if(Attack)
            return AttackArry[x, y];
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
                // CreateText(GetPosition(i, j) + new Vector3(CellSize, 0, CellSize)*0.5f, GetGridValue(new Vector2Int(i, j)).statucode.ToString(), 5, Color.white);
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
 