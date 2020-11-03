using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipRound : MonoBehaviour
{
    Map MyMap;
    private void Start() 
    {    
        MyMap = GameObject.Find("Map").GetComponent<Map>();
    }
    public void Skip()
    {
        MyMap.ActionEnd = MyMap.CharaNumber;
        return;
    }
}
