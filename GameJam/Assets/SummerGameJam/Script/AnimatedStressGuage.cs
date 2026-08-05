using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 増減演出用のストレスゲージクラス。
/// 変化前後の値を渡して、アニメーション再生を行います。
/// </summary>
public class AnimatedStressGauge : MonoBehaviour
{
    [SerializeField] private Image frontImage;
    [Header("アニメーション設定")]
    [SerializeField] private float changeSpeed = 1.5f; // 1秒間の変化割合

    private Coroutine animateCoroutine;

    private void Awake()
    {
        if (frontImage != null)
        {
            frontImage.type = Image.Type.Filled;
        }
    }

    /// <summary>
    /// 現在のゲージ割合から、新しい目標値までアニメーションを開始する
    /// </summary>
    public void AnimateTo(int newStress, int maxStress)
    {
        if (frontImage == null || maxStress <= 0) return;

        float targetFill = Mathf.Clamp01((float)newStress / maxStress);
        StartAnimateCoroutine(frontImage.fillAmount, targetFill);
    }

    /// <summary>
    /// 変化前後の値を明示してアニメーションを開始する（結果ポップアップ用）
    /// </summary>
    /// <param name="startStress">変化前のストレス値</param>
    /// <param name="targetStress">変化後のストレス値</param>
    /// <param name="maxStress">最大ストレス値</param>
    public void StartAnimation(int startStress, int targetStress, int maxStress)
    {
        if (frontImage == null || maxStress <= 0) return;

        float startFill = Mathf.Clamp01((float)startStress / maxStress);
        float targetFill = Mathf.Clamp01((float)targetStress / maxStress);

        frontImage.fillAmount = startFill;
        StartAnimateCoroutine(startFill, targetFill);
    }

    private void StartAnimateCoroutine(float startFill, float targetFill)
    {
        if (animateCoroutine != null)
        {
            StopCoroutine(animateCoroutine);
        }
        animateCoroutine = StartCoroutine(AnimateGaugeRoutine(targetFill));
    }

    private IEnumerator AnimateGaugeRoutine(float targetFill)
    {
        while (!Mathf.Approximately(frontImage.fillAmount, targetFill))
        {
            frontImage.fillAmount = Mathf.MoveTowards(
                frontImage.fillAmount,
                targetFill,
                changeSpeed * Time.deltaTime
            );
            yield return null;
        }
        frontImage.fillAmount = targetFill;
    }
}