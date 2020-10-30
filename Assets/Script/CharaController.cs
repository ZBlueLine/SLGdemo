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
    public float speed;
    public float turnSpeed;
    public int MaxHp;
    public int CurrentHp;

    //Mapinfo
    GameObject InMap;
    Map MyMap;
    Stack<Vector2Int> DirList;

    Animator  m_Animator;
    public Material RangeMaterial;
    Vector2Int Index;
    
    //Move
    Vector3 NextPosition;
    bool isWalking;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;
    
    //UI
    public Object PropertyUI;
    GameObject MyCanvas;
    


    public GameObject InMap1 { get => InMap; set => InMap = value; }
    public GameObject MyCanvas1 { get => MyCanvas; set => MyCanvas = value; }

    void Start()
    {
        // CurrentHp = MaxHp;
        DirList = new Stack<Vector2Int>();
        // DirList.Push(new Vector2Int(0, 1));
        // DirList.Push(new Vector2Int(2, 2));
        // DirList.Push(new Vector2Int(4, 0));
        // DirList.Push(new Vector2Int(4, 6));
        // DirList.Push(new Vector2Int(1, 1));
        MyMap = InMap1.gameObject.GetComponent<Map>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        WaitStatus = 0;
        LastActTime = Time.time;
        Move();
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
        m_Animator.SetBool ("IsWalking", isWalking);
        if(isWalking)
        {
            WaitStatus = 0;
            float step = speed * Time.deltaTime;  
             
            m_Movement.Normalize();
            Debug.Log(transform.forward+"transform.forward");
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            Debug.Log(desiredForward);
            m_Rotation = Quaternion.LookRotation(desiredForward);
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, NextPosition, step);
            if(gameObject.transform.position.Equals(NextPosition))
            {
                Move();
            }
        }
        else if(NowTime - LastActTime > (Random.Range(4, 7)))
        {
            AnimatorStateInfo m_Animainfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            if(WaitStatus == 0)
            {
                WaitStatus = Random.Range(1, 4);
                LastActTime = NowTime;
                m_Animator.SetInteger("WaitStatus", WaitStatus);
            }
            else if(m_Animainfo.normalizedTime >= 1f)
            {
                LastActTime = NowTime;
                WaitStatus = 0;
                m_Animator.SetInteger("WaitStatus", WaitStatus);
            }
        }
    }

    public void AddMoveAction(Vector2Int Position)
    {
        DirList.Push(Position);
    }
    public void Move()
    {
        isWalking = true;
        if(DirList.Count!=0)
        {
            Vector2Int NextPos = DirList.Peek();
            DirList.Pop();
            NextPosition = MyMap.GridIndexToWorld(new Vector3(NextPos.x, 0, NextPos.y));
            m_Movement = (NextPosition - transform.position);
            m_Movement.y = 0;
            Debug.Log(m_Movement+"m_Movement");
        }
        else 
        {
            Vector3 tmp = MyMap.WorldToGridIndex(gameObject.transform.position);
            SetIndex((int)tmp.x, (int)tmp.z);
            isWalking = false;
            MyMap.SetGridValue(new Vector2Int((int)tmp.x, (int)tmp.z), GlobalVar.IsChara);
        }
    }

    private bool InRange(Vector3 s, Vector2Int e, int Hei, int Wid)
    {
        if(s.x < 0||s.z < 0||s.x >= Wid||s.z >= Hei)return false;
        int len = Mathf.Abs((int)s.x-e.x);
        len += Mathf.Abs((int)s.z - e.y);
        if(len < MoveRange)
            return true;
        return false;
    }

    public void ShowMoveRange()
    {
        int Height = MyMap.Height;
        int Width = MyMap.Width;
        int CellSize = MyMap.CellSize;
        Vector3 StartIndex = new Vector3(Index.x - MoveRange + 1, 0, Index.y - MoveRange + 1);
        Vector3 NowIndex = new Vector3();
        for(int i = 0; i < MoveRange*2; ++i)
        {
            for(int j = 0; j < MoveRange*2; ++j)
            {
                NowIndex.x = StartIndex.x + i;
                NowIndex.z = StartIndex.z + j;
                if(!InRange(NowIndex, Index, Height, Width))continue;
                MyMap.AddClickMark(UtilsTool.CreatePlane(NowIndex, CellSize, InMap1.transform, RangeMaterial));
                if(NowIndex.x == Index.x&&NowIndex.z == Index.y)continue;
                if(MyMap.GetGridValue(new Vector2Int((int)NowIndex.x, (int)NowIndex.z)) == GlobalVar.IsChara)continue;
                MyMap.SetGridValue(new Vector2Int((int)NowIndex.x, (int)NowIndex.z), GlobalVar.PrepareMove);
            }
        }
        MyMap.ShowValue();
    }

    public void Showproperties()
    {
        CharaManager UIMan =  MyMap.GetComponentInChildren<CharaManager>();
        GameObject UI1 =  MyMap.GetComponentInChildren<CharaManager>().HPUI;
        if(UIMan.HPUI)
            GameObject.Destroy(UIMan.HPUI);
        UIMan.HPUI = Instantiate(PropertyUI, new Vector3(100, 30, 0),  Quaternion.identity) as GameObject;
        UIMan.HPUI.GetComponent<HPSliderContr>().Chara = this;
        UIMan.HPUI.transform.SetParent(MyCanvas.transform);
    }
}
