using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaManager : MonoBehaviour
{
    private List<GameObject> Ally;
    public List<Vector3Int> AllyGridIndex;
    void Awake()
    {
        Ally = new List<GameObject>();
    }
    void Start()
    {
        //创建角色
        Object Prefab = Resources.Load("unitychan");
        CreateGrid UseFunction = transform.parent.gameObject.GetComponent<CreateGrid>();
        foreach(Vector3Int c in AllyGridIndex)
        {
            UseFunction.SetGridValue(new Vector2Int(c.x, c.z), 1);
            Vector3 TmpPosition = new Vector3();
            TmpPosition = UseFunction.GridIndexToWorld(c);
            TmpPosition.y -= 0.05f;
            GameObject NewAlly = Instantiate(Prefab, TmpPosition,  Quaternion.identity) as GameObject;
            Ally.Add(NewAlly);
        }
        UseFunction.ShowValue();
    }
}
