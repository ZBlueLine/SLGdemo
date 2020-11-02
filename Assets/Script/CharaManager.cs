using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

public class CharaManager : MonoBehaviour
{
    public List<Vector3Int> AllyGridIndex;
    public List<Object> Prefab;
    public GameObject MyCanvas;
    GameObject HPui;
    List<GameObject> AllChara;
    List<GameObject> AllEnemy;
    public List<int> TeamTag;
    Map UseFunction;
    int choseMark;

    public GameObject HPUI { get => HPui; set => HPui = value; }
    public int ChoseMark { get => choseMark; set => choseMark = value; }

    void Awake()
    {
        ChoseMark = 0;
        MyCanvas = GameObject.Find("Canvas");
        AllChara = new List<GameObject>();
        AllEnemy = new List<GameObject>();
    }
    void Start()
    {
        //创建角色
        UseFunction = transform.parent.gameObject.GetComponent<Map>();
        int charenumber = AllyGridIndex.Count;
        
        for(int i = 0, j = 0; i < charenumber&&j < Prefab.Count; ++i,++j)
        {
            if(TeamTag[i] == GlobalVar.IsChara)
                UseFunction.SetGridValue(new Vector2Int(AllyGridIndex[i].x, AllyGridIndex[i].z), IsCharaStatus.getInstance());
            else
                UseFunction.SetGridValue(new Vector2Int(AllyGridIndex[i].x, AllyGridIndex[i].z), IsEnemyStatus.getInstance());
            Vector3 TmpPosition = new Vector3();
            TmpPosition = new Vector3Int(AllyGridIndex[i].x * UseFunction.CellSize + UseFunction.CellSize/2, 0, AllyGridIndex[i].z * UseFunction.CellSize+UseFunction.CellSize/2);
            TmpPosition.y -= 0.05f;
            //create new chara
            GameObject NewAlly = Instantiate(Prefab[j], TmpPosition,  Quaternion.identity) as GameObject;
            CharaController m_Controller = NewAlly.GetComponent<CharaController>();
            m_Controller.InMap1 = transform.parent.gameObject;


            m_Controller.SetIndex(AllyGridIndex[i].x, AllyGridIndex[i].z);
            m_Controller.MyCanvas1 = MyCanvas;
            m_Controller.TeamTag = TeamTag[i];
            NewAlly.transform.SetParent(transform, false);
            if(TeamTag[i] == GlobalVar.IsChara)
            {
                ++UseFunction.CharaNumber;
                AllChara.Add(NewAlly);
            }
            else
            {
                ++UseFunction.EnemyNumber;
                AllEnemy.Add(NewAlly);
            }
            UseFunction.AddObject(new Vector2Int(AllyGridIndex[i].x, AllyGridIndex[i].z), NewAlly);
        }
        M_EnemyNumber = UseFunction.EnemyNumber;
        UseFunction.ShowValue();
    }

    void ReSetCharaStatus()
    {
        foreach(GameObject c in AllChara)
        {
            CharaController Contro = c.GetComponent<CharaController>();
            Contro.Status = GlobalVar.BeginRound;
        }
    }
    int m_EnemyNumber;
    public int M_EnemyNumber { get => m_EnemyNumber; set => m_EnemyNumber = value; }
    public GameObject GetAEnemy()
    {
        if(ChoseMark >= M_EnemyNumber)
            return null;
        while(!AllEnemy[ChoseMark])
        {
            ++ChoseMark;
            if(ChoseMark >= M_EnemyNumber)
                return null;
        }
        return AllEnemy[ChoseMark++];
    }

    public GameObject GetNearestChara(Vector2Int Index)
    {
        if(AllChara.Count == 0)
            return null;
        CharaController Mychara;
        GameObject res = null;
        int Mindis = 0x3f3f3f;
        foreach(GameObject c in AllChara)
        {   
            if(!c)
                continue;
            Mychara = c.GetComponent<CharaController>();
            int Dis = Mathf.Abs(Index.x - Mychara.GetIndex().x) + Mathf.Abs(Index.y - Mychara.GetIndex().y);
            if(Dis < Mindis)
            {
                Mindis = Dis;
                res = c;
            }
        }
        return res;
    }

    public void NewRound()
    {
        foreach(var c in AllChara)
        {
            if(c)
                c.GetComponent<CharaController>().Status = GlobalVar.BeginRound;
        }
        foreach(var c in AllEnemy)
        {
            if(c)
                c.GetComponent<CharaController>().Status = GlobalVar.BeginRound;
        }
    }
}
