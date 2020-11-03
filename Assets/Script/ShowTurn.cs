using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTurn : MonoBehaviour
{
    public GameObject PlayerTrunUI;
    public GameObject EnemyTrunUI;
    GameObject MyCanvas;
    private void Awake() 
    {
        MyCanvas = GameObject.Find("Canvas");
    }
    public IEnumerator PlayerTrun()
    {
        GameObject PlayerUI = Instantiate(PlayerTrunUI, new Vector3(20, -4, 0), Quaternion.identity) as GameObject;
        PlayerUI.transform.SetParent(MyCanvas.transform, false);
        yield return new WaitForSeconds(1.9f);
        Destroy(PlayerUI);
    }

    public IEnumerator EnemyTrun()
    {
        GameObject EnemyUI = Instantiate(EnemyTrunUI, new Vector3(20, -4, 0), Quaternion.identity) as GameObject;
        EnemyUI.transform.SetParent(MyCanvas.transform, false);
        yield return new WaitForSeconds(1.4f);
        Destroy(EnemyUI);
    }
}
