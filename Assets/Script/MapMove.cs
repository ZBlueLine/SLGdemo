using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMove : MonoBehaviour
{
    private bool onMove;
    private Vector3 lastPosition;

    void Awake()
    {
        onMove = false;
        lastPosition = new Vector3(0, 0, 0);
    }

    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            if(Input.GetMouseButtonDown(1))
            {
                onMove = true;
                lastPosition = Input.mousePosition;
            }
            if(Input.GetMouseButtonDown(0))
            {
                onMove = false;
            }
            // Camera.main
        }
        else 
        {

        }
    }
}
