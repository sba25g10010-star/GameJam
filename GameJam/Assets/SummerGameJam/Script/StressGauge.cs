using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ストレスゲージの見た目だけを管理する。
/// Source Imageには依存せず、ImageのFill Amountだけを更新する。
/// </summary>
public class StressGauge : MonoBehaviour
{
    [SerializeField] private Image frontImage;

    public void SetStress(int currentStress, int maxStress)
    {
        if (frontImage == null)
        {
            Debug.LogError("HeartGauge_frontが設定されていません。", this);
            return;
        }

        if (maxStress <= 0)
        {
            Debug.LogError("最大ストレス値は1以上にしてください。", this);
            return;
        }

        // Source Imageを変更しても、Filled設定とFill Amountで同じように動作する。
        frontImage.type = Image.Type.Filled;
        frontImage.fillAmount = Mathf.Clamp01((float)currentStress / maxStress);
    }
}
