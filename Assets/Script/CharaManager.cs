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

    public GameObject HPUI { get => HPui; set => HPui = value; }

    void Awake()
    {
        MyCanvas = GameObject.Find("Canvas");
    }
    void Start()
    {
        //创建角色
        // Prefab.Add(Resources.Load("unitychan"));
        Map UseFunction = transform.parent.gameObject.GetComponent<Map>();
        for(int i = 0, j = 0; i < AllyGridIndex.Count&&j < Prefab.Count; ++i,++j)
        {
            UseFunction.SetGridValue(new Vector2Int(AllyGridIndex[i].x, AllyGridIndex[i].z), GlobalVar.IsChara);
            Vector3 TmpPosition = new Vector3();
            TmpPosition = UseFunction.GridIndexToWorld(AllyGridIndex[i]);
            TmpPosition.y -= 0.05f;
            //create new chara
            GameObject NewAlly = Instantiate(Prefab[j], TmpPosition,  Quaternion.identity) as GameObject;
            CharaController m_Controller = NewAlly.gameObject.GetComponent<CharaController>();
            m_Controller.InMap1 = transform.parent.gameObject;
            transform.forward = new Vector3(1, 0, -1);
            m_Controller.SetIndex(AllyGridIndex[i].x, AllyGridIndex[i].z);
            m_Controller.MyCanvas1 = MyCanvas;
            UseFunction.AddObject(new Vector2Int(AllyGridIndex[i].x, AllyGridIndex[i].z), NewAlly);
        }
        UseFunction.ShowValue();
    }
}
