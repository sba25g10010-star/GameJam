using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 増減演出用の作業効率ゲージクラス。
/// </summary>
public class AnimatedWorkGauge : MonoBehaviour
{
    [SerializeField] private Image frontImage;
    [Header("アニメーション設定")]
    [SerializeField] private float changeSpeed = 1.5f;

    private Coroutine animateCoroutine;

    private void Awake()
    {
        if (frontImage != null)
        {
            frontImage.type = Image.Type.Filled;
        }
    }

    public void AnimateTo(int newEfficiency)
    {
        if (frontImage == null) return;

        float targetFill = Mathf.Clamp01(newEfficiency / 100f);
        StartAnimateCoroutine(frontImage.fillAmount, targetFill);
    }

    public void StartAnimation(int startEfficiency, int targetEfficiency)
    {
        if (frontImage == null) return;

        float startFill = Mathf.Clamp01(startEfficiency / 100f);
        float targetFill = Mathf.Clamp01(targetEfficiency / 100f);

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