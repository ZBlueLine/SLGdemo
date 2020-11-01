using UnityEngine;
using Global;

public class MoveButtom : MonoBehaviour
{
    Map MyMap;
    private void Awake() 
    {
        MyMap = GameObject.Find("Map").GetComponent<Map>();
    }
    public void Click()
    {
        GameObject Selectchara = GridStatus.SelectedChara;
        if(Selectchara&&Selectchara.GetComponent<CharaController>().Status < GlobalVar.Moved)
        {
            MyMap.ReSetGridValue();
            MyMap.ClearText();
            CharaController MyCont = Selectchara.GetComponent<CharaController>();
            MyCont.ShowMoveRange();
        }
    }

}
