using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

public class StandbyButtom : MonoBehaviour
{
    Map MyMap;
    private void Awake() 
    {
        MyMap = GameObject.Find("Map").GetComponent<Map>();
    }
    public void Click()
    {
        GameObject Selectchara = GridStatus.SelectedChara;
        Debug.Log("StandbyButtom!!");
        Selectchara.GetComponent<CharaController>().Status = GlobalVar.EndRound;
        MyMap.ReSetGridValue();
        ++MyMap.ActionEnd;
        MyMap.CheckTurn();
    }
}
