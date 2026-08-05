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
        int workEfficiencyChange)
    {
        SetText(resultText, result);
        SetText(commentText, comment);
        SetText(stressChangeText, $"ストレス {FormatChange(stressChange)}%");
        SetText(workEfficiencyChangeText, $"作業効率 {FormatChange(workEfficiencyChange)}%");
    }
}