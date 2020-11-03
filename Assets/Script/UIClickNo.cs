using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIClickNo : MonoBehaviour
{
    public void ClickNo()
    {
        Destroy(transform.parent.gameObject);
    }
}
