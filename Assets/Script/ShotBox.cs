using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBox : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Bullet;
    public float Speed;
    public int AttackRange;
    GameObject ABullet;
    Vector3 AimPos;
    int SellSize;
    private void Start() 
    {
        SellSize = transform.parent.parent.GetComponent<Map>().CellSize;
        AimPos = new Vector3(0, 1, AttackRange*SellSize);
    }
    void Update()
    {
        if(ABullet)
        {
            Vector3 NowPos = ABullet.transform.localPosition;
            if(NowPos.Equals(AimPos))
                Destroy(ABullet);
            float step = Time.deltaTime*Speed;
            ABullet.transform.localPosition = Vector3.MoveTowards(NowPos,  AimPos, step);
        }
    }

    public void Shot(Vector3 dir)
    {
        ABullet = Instantiate(Bullet, 0.5f*transform.forward, Quaternion.identity);
        Vector3 adjustment = new Vector3(0, 1, 0);
        ABullet.transform.localPosition += adjustment;
        ABullet.transform.SetParent(transform, false);
    }
}
