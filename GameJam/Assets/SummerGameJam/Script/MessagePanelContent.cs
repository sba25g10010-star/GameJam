using TMPro;
using UnityEngine;

/// <summary>
/// MessagePanel内の結果情報とアニメーションゲージをまとめて管理する。
/// </summary>
public class MessagePanelContent : MonoBehaviour
{
    [Header("テキスト参照")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI commentText;
    [SerializeField] private TextMeshProUGUI stressChangeText;
    [SerializeField] private TextMeshProUGUI workEfficiencyChangeText;

    [Header("アニメーションゲージ参照")]
    [SerializeField] private AnimatedStressGauge animatedStressGauge;
    [SerializeField] private AnimatedWorkGauge animatedWorkGauge;

    [Header("アニメーションテキスト参照")]
    [SerializeField] private AnimatedPercentage animatedStressText;
    [SerializeField] private AnimatedPercentage animatedWorkText;

    /// <summary>
    /// メッセージパネルの内容をセットし、ゲージのアニメーションを開始する
    /// </summary>
    public void SetContent(
        string result,
        string comment,
        int stressChange,
        int workEfficiencyChange,
        int previousStress,
        int currentStress,
        int maxStress,
        int previousEfficiency,
        int currentEfficiency)
    {
        SetText(resultText, result);
        SetText(commentText, comment);
        SetText(stressChangeText, $"ストレス {FormatChange(stressChange)}%");
        SetText(workEfficiencyChangeText, $"作業効率 {FormatChange(workEfficiencyChange)}%");

        // メッセージパネルが表示されるタイミングでゲージアニメーションを開始
        if (animatedStressGauge != null)
        {
            animatedStressGauge.StartAnimation(previousStress, currentStress, maxStress);
        }

        if (animatedWorkGauge != null)
        {
            animatedWorkGauge.StartAnimation(previousEfficiency, currentEfficiency);
        }
        
        if (animatedStressText != null)
        {
            int prevStressPercent = Mathf.RoundToInt((float)previousStress / maxStress * 100f);
            int currStressPercent = Mathf.RoundToInt((float)currentStress / maxStress * 100f);
            animatedStressText.StartAnimation(prevStressPercent, currStressPercent);
        }

        if (animatedWorkText != null)
        {
            animatedWorkText.StartAnimation(previousEfficiency, currentEfficiency);
        }
    }

    private static string FormatChange(int amount)
    {
        return amount.ToString("+#;-#;0");
    }

    private static void SetText(TextMeshProUGUI target, string value)
    {
        if (target != null)
        {
            target.text = value ?? string.Empty;
        }
    }

    public void DeathSetContent(
        string result,
        string comment,
        int stressChange,
        int workEfficiencyChange,
        int previousStress,
        int currentStress,
        int maxStress,
        int previousEfficiency,
        int currentEfficiency)
    {
        SetContent(
            result,
            comment,
            stressChange,
            workEfficiencyChange,
            previousStress,
            currentStress,
            maxStress,
            previousEfficiency,
            currentEfficiency);
    }
}
