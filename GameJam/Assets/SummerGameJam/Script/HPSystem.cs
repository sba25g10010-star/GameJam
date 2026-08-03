using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class HPSystem : MonoBehaviour
{
    [SerializeField] private Image image;
    
    //()の中身は引数、他のところから数値を得て{}の中で使う
	public void HPDown (float current, int max)
    {
        //ImageというコンポーネントのfillAmountを取得して操作する
        image.fillAmount = current / max;
    }
}
