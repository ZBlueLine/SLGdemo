using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharaColltroler : MonoBehaviour
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
                System.Random rd = new System.Random();
                WaitStatus = rd.Next(1,5);
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
