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
    int Enemynumbers;

    public GameObject HPUI { get => HPui; set => HPui = value; }

    void Awake()
    {
        choseMark = 0;
        Enemynumbers = 0;
        MyCanvas = GameObject.Find("Canvas");
        AllChara = new List<GameObject>();
        AllEnemy = new List<GameObject>();
    }
    void Start()
    {
        //创建角色
        Map UseFunction = transform.parent.gameObject.GetComponent<Map>();
        int charenumber = AllyGridIndex.Count;
        
        for(int i = 0, j = 0; i < charenumber&&j < Prefab.Count; ++i,++j)
        {
            if(TeamTag[i] == GlobalVar.IsChara)
                UseFunction.SetGridValue(new Vector2Int(AllyGridIndex[i].x, AllyGridIndex[i].z), IsCharaStatus.getInstance());
            else
                UseFunction.SetGridValue(new Vector2Int(AllyGridIndex[i].x, AllyGridIndex[i].z), IsEnemyStatus.getInstance());
            Vector3 TmpPosition = new Vector3();
            TmpPosition = UseFunction.GridIndexToWorld(AllyGridIndex[i]);
            TmpPosition.y = UseFunction.transform.position.y;
            TmpPosition.y -= 0.05f;
            //create new chara
            GameObject NewAlly = Instantiate(Prefab[j], TmpPosition,  Quaternion.identity) as GameObject;
            CharaController m_Controller = NewAlly.gameObject.GetComponent<CharaController>();
            m_Controller.InMap1 = transform.parent.gameObject;
            transform.forward = new Vector3(1, 0, -1);
            m_Controller.SetIndex(AllyGridIndex[i].x, AllyGridIndex[i].z);
            m_Controller.MyCanvas1 = MyCanvas;
            m_Controller.TeamTag = TeamTag[i];
            if(TeamTag[i] == GlobalVar.IsChara)
            {
                ++UseFunction.CharaNumber;
                AllChara.Add(NewAlly);
            }
            else
            {
                ++Enemynumbers;
                AllEnemy.Add(NewAlly);
            }
            UseFunction.AddObject(new Vector2Int(AllyGridIndex[i].x, AllyGridIndex[i].z), NewAlly);
        }
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
    public GameObject GetAEnemy()
    {
        if(choseMark >= Enemynumbers)  
        {
            choseMark = 0;
            return null;
        }
        return AllEnemy[choseMark++];
    }

    public GameObject GetNearestChara(Vector2Int Index)
    {
        if(AllChara.Count == 0)
            return null;
        CharaController Mychara = AllChara[0].GetComponent<CharaController>();
        GameObject res = AllChara[0];
        int Mindis = Mathf.Abs(Index.x - Mychara.GetIndex().x) + Mathf.Abs(Index.y - Mychara.GetIndex().y);
        foreach(GameObject c in AllChara)
        {   
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
            c.GetComponent<CharaController>().Status = GlobalVar.BeginRound;
        }
        foreach(var c in AllEnemy)
        {
            c.GetComponent<CharaController>().Status = GlobalVar.BeginRound;
        }
    }
}
