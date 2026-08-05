using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ストレスゲージの見た目だけを管理する。
/// Source Imageには依存せず、ImageのFill Amountをアニメーションさせて更新する。
/// </summary>
public class StressGauge : MonoBehaviour
{
    [SerializeField] private Image frontImage;
    [Header("アニメーション設定")]
    [SerializeField] private float changeSpeed = 1.5f; // 1秒間に変化する割合（例: 1.5なら約0.67秒で全開）

    private float targetFillAmount;

    private void Awake()
    {
        if (frontImage != null)
        {
            frontImage.type = Image.Type.Filled;
            targetFillAmount = frontImage.fillAmount;
        }
    }

    private void Update()
    {
        if (frontImage == null) return;

        // 現在の値から目標値へ一定速度で滑らかに変化させる
        if (!Mathf.Approximately(frontImage.fillAmount, targetFillAmount))
        {
            frontImage.fillAmount = Mathf.MoveTowards(
                frontImage.fillAmount, 
                targetFillAmount, 
                changeSpeed * Time.deltaTime
            );
        }
    }

    public void SetStress(int currentStress, int maxStress)
    {
        if (frontImage == null)
        {
            Debug.LogError("frontImageが設定されていません。", this);
            return;
        }

        if (maxStress <= 0)
        {
            Debug.LogError("最大ストレス値は1以上にしてください。", this);
            return;
        }

        // 直接fillAmountを変更せず、目標値だけを更新する
        targetFillAmount = Mathf.Clamp01((float)currentStress / maxStress);
    }
}