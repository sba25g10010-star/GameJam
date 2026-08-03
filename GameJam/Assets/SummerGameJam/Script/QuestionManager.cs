using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    private List<QuestionSO> questionDatabase = new List<QuestionSO>();
    private QuestionSO currentQuestion;
    private bool isGameOver = false;

    [Header("UIクラスの参照")]
    [SerializeField] private QuestionUI gameUI;

    void Awake()
    {
        QuestionSO[] loadedQuestions = Resources.LoadAll<QuestionSO>("Questions");
        questionDatabase.AddRange(loadedQuestions);
    }

    void Start()
    {
        NextTurn();
    }

    /// <summary>
    /// 次の問題をセットして表示する
    /// </summary>
    public void NextTurn()
    {
        if (isGameOver) return;
        if (questionDatabase.Count == 0) return;

        int randomIndex = Random.Range(0, questionDatabase.Count);
        currentQuestion = questionDatabase[randomIndex];

        gameUI.UpdateUI(currentQuestion);
    }

    /// <summary>
    /// 食べるボタンが押された（成否に関わらず次は進む）
    /// </summary>
    public void OnEatButton()
    {
        if (isGameOver) return;

        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= currentQuestion.failureChance)
        {
            Debug.Log($"失敗！確率 {currentQuestion.failureChance}% に対し {randomValue:F1} を引いた");
            isGameOver = true;
            gameUI.ShowGameOverUI();
        }
        else
        {
            Debug.Log($"成功！確率 {currentQuestion.failureChance}% に対し {randomValue:F1} で回避");
            NextTurn();
        }
    }

    /// <summary>
    /// スルーボタンが押された
    /// </summary>
    public void OnPassButton()
    {
        if (isGameOver) return;

        Debug.Log("スルーして次の問題へ");
        NextTurn();
    }
}