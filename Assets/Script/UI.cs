using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject EscUI;
    GameObject MyEsc;
    public GameObject MyCanvas;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(MyEsc)
            {
                Destroy(MyEsc);
            }
            else 
            {
                MyEsc = Instantiate(EscUI, new Vector3(20, -4, 0),  Quaternion.identity) as GameObject;
                MyEsc.transform.SetParent(MyCanvas.transform, false);
            }
            
        }
    }

}
