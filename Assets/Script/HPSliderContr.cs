using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPSliderContr : MonoBehaviour
{
    CharaController chara;
    Slider MySlider;
    TMP_Text MaxHPtext;
    TMP_Text CurrentHPtext;

    public CharaController Chara { get => chara; set => chara = value; }
    void Awake() 
    {
        MySlider = gameObject.GetComponent<Slider>();
        MaxHPtext = transform.Find("MaxHP").gameObject.GetComponent<TMP_Text>();
        CurrentHPtext = transform.Find("CurrentHP").gameObject.GetComponent<TMP_Text>();
    }
    void Start() 
    {
        MySlider.value = (float)chara.CurrentHp/chara.MaxHp;
        MaxHPtext.text = chara.MaxHp.ToString();
        CurrentHPtext.text = chara.CurrentHp.ToString();
    }
    // void Update()
    // {
    //     MySlider.value = (float)chara.CurrentHp/chara.MaxHp;
    //     MaxHPtext.text = chara.MaxHp.ToString();
    //     CurrentHPtext.text = chara.CurrentHp.ToString();
    // }
    
}
