using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    private List<QuestionSO> questionDatabase = new List<QuestionSO>();
    private QuestionSO currentQuestion;
    private bool isGameOver = false;

    [Header("別マネージャーへの参照")]
    [SerializeField] private TeacherManager teacherManager; // 🚀 先生マネージャーとの連携

    [Header("UIクラスの参照")]
    [SerializeField] private QuestionUI gameUI;

    void Awake()
    {
        // フォルダから問題を自動ロード
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

        // 1. 問題をランダムにセットして表示
        int randomIndex = Random.Range(0, questionDatabase.Count);
        currentQuestion = questionDatabase[randomIndex];
        gameUI.UpdateUI(currentQuestion);

        // 2. 🚀 先生マネージャーに命令して、今回の先生をランダムセットさせる
        if (teacherManager != null)
        {
            teacherManager.SetupInitialTeacherForTurn();
        }
    }

    /// <summary>
    /// 🧑‍🏫 「先生に聞く」ボタンが押されたときの処理
    /// </summary>
    public void OnAskTeacherButton()
    {
        if (isGameOver || teacherManager == null) return;

        // いま画面にいる先生のデータを取得
        TeacherSO activeTeacher = teacherManager.CurrentTeacher;
        if (activeTeacher == null) return;

        // ⚔️ 相性判定（選んだ先生が、問題SOに登録されている「正解の先生」と一致するか）
        if (activeTeacher == currentQuestion.correctTeacher)
        {
            // 一致した場合のログ
            Debug.Log($"⭕ 専門の 【{activeTeacher.teacherName}】 に聞いた！相性バッチリ！");
        }
        else
        {
            // 一致しなかった場合のログ
            string correctName = currentQuestion.correctTeacher != null ? currentQuestion.correctTeacher.teacherName : "なし";
            Debug.Log($"❌ 専門外の 【{activeTeacher.teacherName}】 に聞いてしまった...（この問題の正解は {correctName}）");
        }

        // どっちにせよ次の問題へ進む
        NextTurn();
    }

    /// <summary>
    /// 🤖 「AIに聞く」ボタンが押されたときの処理
    /// </summary>
    public void OnAskAIButton()
    {
        if (isGameOver) return;

        float randomValue = Random.Range(0f, 100f);

        // AIが見当違いなことを言った（失敗確率を引いた）
        if (randomValue <= currentQuestion.failureChance)
        {
            Debug.Log($"💀 AIが見当違いなことを言った！（確率 {currentQuestion.failureChance}% に対し {randomValue:F1} を引いた）");
        }
        // AIが正しいことを言った（成功）
        else
        {
            Debug.Log($"🤖 AIが正しい回答をくれた！（確率 {currentQuestion.failureChance}% に対し {randomValue:F1} で回避）");
        }

        // どっちにせよ次の問題へ進む
        NextTurn();
    }
}