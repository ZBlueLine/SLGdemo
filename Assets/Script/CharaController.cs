using UnityEngine;
using Utils;
using Global;
using System.Collections.Generic;

public class CharaController : MonoBehaviour
{
    //Property
    int WaitStatus;
    float LastActTime;
    public int MoveRange;
    public int AttackRange;
    public float speed;
    public float turnSpeed;
    public int MaxHp;
    public int CurrentHp;
    public int TeamTag;

    //记录本回合是否已经完成了动作
    int status;
    public int Status{get => status; set => status = value;}

    //Mapinfo
    GameObject InMap;
    Map MyMap;
    Stack<Vector2Int> DirList;

    Animator  m_Animator;
    public Material RangeMaterial;
    public Material AttackMaterial;
    Vector2Int Index;
    
    //Action
    Vector3 NextPosition;
    bool IsWalking;
    bool BeDamaged;
    bool Death;
    public int ATK;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    //UI
    public Object PropertyUI;
    GameObject MyCanvas;
    GameObject UI1;
    


    public GameObject InMap1 { get => InMap; set => InMap = value; }
    public GameObject MyCanvas1 { get => MyCanvas; set => MyCanvas = value; }

    void Start()
    {
        Death = false;
        DirList = new Stack<Vector2Int>();
        MyMap = InMap1.gameObject.GetComponent<Map>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        WaitStatus = 0;
        LastActTime = Time.time;
    }

    public void SetIndex(int x, int y)
    {   
        Index.x = x;
        Index.y = y;
    }
    public void SetIndex(Vector2Int pos)
    {   
        int x = pos.x;
        int y = pos.y;
        Index.x = x;
        Index.y = y;
    }

    public Vector2Int GetIndex()
    {
        return new Vector2Int(Index.x, Index.y);
    }
    void OnAnimatorMove ()
    {
        m_Rigidbody.MoveRotation (m_Rotation);
    }

    void FixedUpdate ()
    {
        float NowTime = Time.time;
        m_Animator.SetInteger("WaitStatus", WaitStatus);
        m_Animator.SetBool ("IsWalking", IsWalking);
        m_Animator.SetBool("BeDamaged", BeDamaged);
        m_Animator.SetBool("Death", Death);
        if(Death)
        {
            AnimatorStateInfo m_Animainfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            if(m_Animainfo.normalizedTime >= 1f)
            {
                Debug.Log("death！！！");
                MyMap.SetGridValue(gameObject.GetComponent<CharaController>().GetIndex(), EmptyLcationstatus.getInstance());
                Destroy(gameObject);
                MyMap.InAttack = false;
                if(TeamTag == GlobalVar.IsEnemy)
                    --MyMap.EnemyNumber;
                else
                    --MyMap.CharaNumber;
                MyMap.CheckTurn();
            }
            return;
        }
        if(IsWalking)
        {
            WaitStatus = 0;
            float step = speed * Time.deltaTime;  
            m_Movement.Normalize();
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, NextPosition, step);
            Debug.Log("gameObject.transform.position  "+gameObject.transform.position);
            Debug.Log("NextPosition  "+NextPosition);
            if(gameObject.transform.position.Equals(NextPosition))
            {
                Debug.Log("next");
                Move();
            }
        }
        else if(BeDamaged)
        {
            AnimatorStateInfo m_Animainfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            if(m_Animainfo.normalizedTime >= 1f)
            {
                BeDamaged = false;
                WaitStatus = 0;
                MyMap.CheckTurn();
                MyMap.InAttack = false;
            }
        }
        else if(NowTime - LastActTime > (Random.Range(4, 7)))
        {
            AnimatorStateInfo m_Animainfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            if(WaitStatus == 0)
            {
                WaitStatus = Random.Range(1, 4);
                LastActTime = NowTime;
            }
            else if(m_Animainfo.normalizedTime >= 1f)
            {
                LastActTime = NowTime;
                WaitStatus = 0;
            }
        }
    }

    public void Damaged(int Value)
    {
        BeDamaged = true;
        CurrentHp -= Value;
        Debug.Log(CurrentHp+ "!!!!!!!");
        Debug.Log(Value+"???????????");
        if(CurrentHp <= 0)
            Death = true;
    }

    public void AddMoveAction(Vector2Int Position)
    {
        DirList.Push(Position);
    }
    public void Move()
    {
        IsWalking = true;
        if(DirList.Count!=0)
        {
            Vector2Int NextPos = DirList.Peek();
            DirList.Pop();
            NextPosition = MyMap.GridIndexToWorld(new Vector3(NextPos.x, 0, NextPos.y));
            m_Movement = (NextPosition - transform.position);
            m_Movement.y = 0;
        }
        else 
        {
            MyMap.Moveing = false;
            //原位置标记为空位置
            MyMap.SetGridValue(new Vector2Int(Index.x, Index.y), EmptyLcationstatus.getInstance());
            Vector3 tmp = MyMap.WorldToGridIndex(gameObject.transform.position);
            Status = GlobalVar.Moved;
            GameObject Tmp = MyMap.ObjectArry[Index.x][Index.y];
            MyMap.ObjectArry[Index.x][Index.y] =  MyMap.ObjectArry[(int)tmp.x][(int)tmp.z];
            MyMap.ObjectArry[(int)tmp.x][(int)tmp.z] = Tmp;
            
            SetIndex((int)tmp.x, (int)tmp.z);
            IsWalking = false;
            if(TeamTag == GlobalVar.IsChara)
                MyMap.SetGridValue(new Vector2Int((int)tmp.x, (int)tmp.z), IsCharaStatus.getInstance());
            else 
                MyMap.SetGridValue(new Vector2Int((int)tmp.x, (int)tmp.z), IsEnemyStatus.getInstance());
        }
    }

    private bool InRange(Vector3 s, Vector2Int e, int Hei, int Wid, int MinRange, int MaxRange)
    {
        if(s.x < 0||s.z < 0||s.x >= Wid||s.z >= Hei)return false;
        int len = Mathf.Abs((int)s.x - e.x);
        len += Mathf.Abs((int)s.z - e.y);
        if(len <= MaxRange&&len > MinRange)
            return true;
        return false;
    }

    private void ShowRange(int MinRange, int MaxRange, GridStatus Mark, bool SetValue)
    {
        int Height = MyMap.Height;
        int Width = MyMap.Width;
        int CellSize = MyMap.CellSize;
        Vector3 StartIndex = new Vector3(Index.x - MaxRange, 0, Index.y - MaxRange);
        Vector3 NowIndex = new Vector3();
        for(int i = 0; i < MaxRange*2 + 1; ++i)
        {
            for(int j = 0; j < MaxRange*2 + 1; ++j)
            {
                NowIndex.x = StartIndex.x + i;
                NowIndex.z = StartIndex.z + j;
                if(!InRange(NowIndex, Index, Height, Width, MinRange, MaxRange))continue;
                GridStatus StatusofChose = MyMap.GetGridValue(new Vector2Int((int)NowIndex.x, (int)NowIndex.z));
                int Statuscode = StatusofChose.statucode;
                
                int visValue = UtilsTool.GetVis((int)NowIndex.x, (int)NowIndex.z, MyMap.Width);
                // 只有显示移动范围才需要判是否可达
                if(Mark.statucode != GlobalVar.Attack)
                {
                    if(visValue == 0)continue;
                    if(visValue == 2)
                        MyMap.AddClickMark(UtilsTool.CreatePlane(NowIndex, CellSize, InMap1.transform, AttackMaterial));
                    else
                        MyMap.AddClickMark(UtilsTool.CreatePlane(NowIndex, CellSize, InMap1.transform, RangeMaterial));

                }
                else
                    MyMap.AddClickMark(UtilsTool.CreatePlane(NowIndex, CellSize, InMap1.transform, AttackMaterial));
                
                //敌人角色在这里直接下一轮循环,不需要对地图数值进行设置
                if(!SetValue)continue;

                //如果是攻击选择只修改攻击表格
                if(Mark.statucode == GlobalVar.Attack)
                    if(TeamTag == GlobalVar.IsChara&&Statuscode == GlobalVar.IsEnemy)
                        MyMap.SetAttackValue(new Vector2Int((int)NowIndex.x, (int)NowIndex.z), Mark);
                    else
                        MyMap.SetAttackValue(new Vector2Int((int)NowIndex.x, (int)NowIndex.z), StatusofChose);
                //如果该位置显示的是可攻击位置，则填充准备攻击状态
                else if(StatusofChose.statucode > GlobalVar.CannotMove&&visValue != 2)
                    MyMap.SetGridValue(new Vector2Int((int)NowIndex.x, (int)NowIndex.z), Mark);
                else if(StatusofChose.statucode > GlobalVar.CannotMove)
                    MyMap.SetGridValue(new Vector2Int((int)NowIndex.x, (int)NowIndex.z), PrepareAttackststus.getInstance());
            }
        }
        MyMap.ShowValue();
    }

    public void ShowMoveRange(bool SetValue = true)
    {
        UtilsTool.BFS(MyMap.Height,MyMap.Width,  MyMap.gridArry, Index, new Vector2Int(-1, -1), MoveRange+AttackRange, AttackRange);
        ShowRange(0, MoveRange+AttackRange, PrepareMoveStatus.getInstance(), SetValue);
    }
    public void ShowAttackRange()
    {
        ShowRange(0, AttackRange, Attackststus.getInstance(), true);
        MyMap.SetAttackValue(new Vector2Int((int)Index.x, (int)Index.y), IsCharaStatus.getInstance());
    }
    public void Showproperties()
    {
        if(UI1)
            GameObject.Destroy(UI1);
        UI1 = Instantiate(PropertyUI, new Vector3(100, 90, 0),  Quaternion.identity) as GameObject;
        UI1.GetComponent<HPSliderContr>().Chara = this;
        UI1.transform.SetParent(MyCanvas.transform);
    }
    public void Closeproperties()
    {
        if(UI1)
            GameObject.Destroy(UI1);
    }
}
