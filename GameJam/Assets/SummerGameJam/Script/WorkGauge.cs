using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 作業効率ゲージの見た目を管理する。
/// Source Imageには依存せず、ImageのFill Amountだけを更新する。
/// </summary>
public class WorkGauge : MonoBehaviour
{
    [SerializeField] private Image frontImage;

    public void SetEfficiency(int efficiency)
    {
        if (frontImage == null)
        {
            Debug.LogError("WorkGauge_frontが設定されていません。", this);
            return;
        }

        frontImage.type = Image.Type.Filled;
        frontImage.fillAmount = Mathf.Clamp01(efficiency / 100f);
    }
}
