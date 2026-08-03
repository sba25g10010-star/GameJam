using UnityEngine;
using TMPro;

public class QuestionUI : MonoBehaviour
{
    [Header("UIコンポーネントの紐付け")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI chanceText;

    /// <summary>
    /// 引数でもらったSOのデータを画面のUIに反映する
    /// </summary>
    public void UpdateUI(QuestionSO data)
    {
        nameText.text = data.questionName;
        
        chanceText.text = $"死亡確率: {data.failureChance}%";
    }

    /// <summary>
    /// ゲームオーバー時の画面表示
    /// </summary>
    public void ShowGameOverUI()
    {
        nameText.text = "ゲームオーバー";
    }
}