using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class QuestionManager : MonoBehaviour
{
    private List<QuestionSO> questionDatabase = new List<QuestionSO>();
    private QuestionSO currentQuestion;

    private bool isGameOver = false;
    [Header("先生管理クラスの参照")]
    [SerializeField] private TeacherManager teacherManager;

    private bool isMessagePanelOpen = false;

    [Header("UIクラスの参照")]
    [SerializeField] private QuestionUI gameUI;
    [SerializeField] private GameObject messagePanel;

    [Header("画面遷移")]
    [SerializeField] private string resultSceneName = "Result";

    void Awake()
    {
        QuestionSO[] loadedQuestions = Resources.LoadAll<QuestionSO>("Questions");
        questionDatabase.AddRange(loadedQuestions);
    }

    void Start()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        NextTurn();
    }

    /// <summary>
    /// 次の問題をセットして表示する
    /// </summary>
    public void NextTurn()
    {
        if (isGameOver || isMessagePanelOpen) return;
        if (questionDatabase.Count == 0) return;

        int randomIndex = Random.Range(0, questionDatabase.Count);
        currentQuestion = questionDatabase[randomIndex];

        gameUI.UpdateUI(currentQuestion);
    }

    /// <summary>
    /// AIに聞くボタンが押された
    /// </summary>
    public void OnAIButton()
    {
        if (isGameOver || isMessagePanelOpen || currentQuestion == null) return;


        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= currentQuestion.failureChance)
        {
            Debug.Log(
                $"失敗！確率 {currentQuestion.failureChance}% に対し " +
                $"{randomValue:F1} を引いた"
            );

            isGameOver = true;
            SceneManager.LoadScene(resultSceneName);
        }
        else
        {
            Debug.Log($"成功！確率 {currentQuestion.failureChance}% に対し {randomValue:F1} で回避");

            if (messagePanel == null)
            {
                Debug.LogError("MessagePanelが設定されていません。次の問題には進みません。");
                return;
            }

            isMessagePanelOpen = true;
            messagePanel.SetActive(true);
        }
    }
    /// <summary>
    /// 現在の先生に問題を解いてもらう
    /// </summary>
    public void OnTeacherButton()
    {
        if (isGameOver) return;

        if (teacherManager.IsCorrectTeacher(currentQuestion))
        {
            Debug.Log("先生の得意な問題なので成功！");
            NextTurn();
            return;
        }

        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= currentQuestion.failureChance)
        {
            Debug.Log(
                $"先生が失敗！確率 {currentQuestion.failureChance}% に対し " +
                $"{randomValue:F1} を引いた"
            );

            isGameOver = true;
            gameUI.ShowGameOverUI();
        }
        else
        {
            Debug.Log(
                $"先生の不得意な問題だが成功！確率 " +
                $"{currentQuestion.failureChance}% に対し " +
                $"{randomValue:F1} で回避"
            );

            NextTurn();

        }
    }

    /// <summary>
    /// 一言パネルの「次へ」ボタンが押された
    /// </summary>
    public void OnNextButton()
    {
        if (isGameOver || !isMessagePanelOpen) return;

        messagePanel.SetActive(false);
        isMessagePanelOpen = false;
        NextTurn();
    }


}

