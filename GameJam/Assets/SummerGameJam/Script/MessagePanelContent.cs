using TMPro;
using UnityEngine;

/// <summary>
/// MessagePanel内の結果情報を、役割ごとのTextへ個別に表示する。
/// </summary>
public class MessagePanelContent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI commentText;
    [SerializeField] private TextMeshProUGUI stressChangeText;
    [SerializeField] private TextMeshProUGUI workEfficiencyChangeText;

    public void SetContent(
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
}
