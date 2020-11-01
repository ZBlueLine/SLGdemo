using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Global;

public class AttackButtom : MonoBehaviour
{
    Map MyMap;
    private void Awake() 
    {
        MyMap = GameObject.Find("Map").GetComponent<Map>();
    }
    public void Click()
    {
        GameObject Selectchara = GridStatus.SelectedChara;
        if(Selectchara&&Selectchara.GetComponent<CharaController>().Status < GlobalVar.Attacked)
        {
            MyMap.ReSetGridValue();
            MyMap.ClearText();
            CharaController MyCont = Selectchara.GetComponent<CharaController>();
            MyMap.PrepareAttack = true;
            MyCont.ShowAttackRange();
        }
    }
}
