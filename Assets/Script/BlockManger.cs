using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManger : MonoBehaviour
{
    public GameObject Block;
    public int Numbers;
    public List<Vector2Int> Index;
    Map MyMap;
    private void Awake() 
    {
        MyMap = transform.parent.gameObject.GetComponent<Map>();
    }
    void Start()
    {
        foreach(var c in Index)
        {
            Vector3 TmpLocation = MyMap.GridIndexToWorld(new Vector3(c.x, 0, c.y));
            Instantiate(Block, TmpLocation, Quaternion.identity);
            MyMap.SetGridValue(c, Blockstatus.getInstance());
        }
    }
}
