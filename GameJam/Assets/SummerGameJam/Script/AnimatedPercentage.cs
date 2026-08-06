using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 数値を指定した時間でカウントアップ/ダウン表示する演出用クラス。
/// </summary>
public class AnimatedPercentage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI percentageText;
    [Header("アニメーション設定")]
    [SerializeField] private float duration = 0.67f; // アニメーションにかかる時間（秒）

    private Coroutine animateCoroutine;

    private void Awake()
    {
        if (percentageText == null)
        {
            percentageText = GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// 指定した値から目標値までカウントアニメーションを開始する
    /// </summary>
    public void StartAnimation(int startValue, int targetValue)
    {
        if (percentageText == null) return;

        // 非アクティブ時はコルーチンを起動せず直接最終値を代入
        if (!gameObject.activeInHierarchy)
        {
            percentageText.text = $"{targetValue}%";
            return;
        }

        if (animateCoroutine != null)
        {
            StopCoroutine(animateCoroutine);
        }
        animateCoroutine = StartCoroutine(AnimateValueRoutine(startValue, targetValue));
    }

    private IEnumerator AnimateValueRoutine(int startValue, int targetValue)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            
            // イージング（SmoothStep）でスムーズな数値変動にする
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, Mathf.SmoothStep(0, 1, t)));
            percentageText.text = $"{currentValue}%";

            yield return null;
        }

        percentageText.text = $"{targetValue}%";
    }
}