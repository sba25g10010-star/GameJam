using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.UI;

public class DamageSystem : MonoBehaviour
{
    //HP表示用テキスト
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private HPSystem hpSystem;

    //最大HP
    [SerializeField] private int maxHP = 100;

    //現在のHP
    [SerializeField] private float currentHP;

    private float workHP;    

    private void Start()
    {
        workHP = currentHP;
    }

    private void Update()
    {
        if(workHP < currentHP)
        {
            workHP += Time.deltaTime * 150;
            workHP = Mathf.Min(workHP, currentHP);
        }
        else
        if(workHP > currentHP)
        {
            workHP -= Time.deltaTime * 150;
            workHP = Mathf.Max(workHP, currentHP);
        }

        //TextのTextコンポーネントにアクセス
        //(int)はfloatを整数で表示するためのもの
        text.text = ((int)workHP).ToString();

        //HPSystemのスクリプトのHPDown関数に2つの数値を送る
        hpSystem.HPDown(workHP, maxHP);
   }
}
