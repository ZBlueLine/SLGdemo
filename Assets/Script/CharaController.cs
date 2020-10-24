using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaController : MonoBehaviour
{
    Animator  m_Animator;
    int WaitStatus;
    float LastActTime;
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        WaitStatus = 0;
        LastActTime = Time.time;
    }

    void Update()
    {
        float NowTime = Time.time;
        if(NowTime - LastActTime > 4 )
        {
            AnimatorStateInfo m_Animainfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            if(WaitStatus == 0)
            {
                WaitStatus = Random.Range(1, 4);
                LastActTime = NowTime;
                Debug.Log(WaitStatus);
                m_Animator.SetInteger("WaitStatus", WaitStatus);
            }
            else if(m_Animainfo.normalizedTime >= 1f)
            {
                LastActTime = NowTime;
                WaitStatus = 0;
                m_Animator.SetInteger("WaitStatus", WaitStatus);
            }
        }
    }
}
