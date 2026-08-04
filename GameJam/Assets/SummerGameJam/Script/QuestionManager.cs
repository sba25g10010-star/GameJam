using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class QuestionManager : MonoBehaviour
{
    private List<QuestionSO> questionDatabase = new List<QuestionSO>();
    private QuestionSO currentQuestion;

    private bool teacherFailed = false;
    private bool isGameOver = false;
    [Header("先生管理クラスの参照")]
    [SerializeField] private TeacherManager teacherManager;

    [Header("コメント管理クラスの参照")]
    [SerializeField] private CommentMaager commentMaager;
    private bool isMessagePanelOpen = false;
    [Header("先生変更ボタン")]
    [SerializeField] private Button changeTeacherButton;

    [Range(0f, 1f)]
    [SerializeField] private float disabledButtonAlpha = 0.5f;

    private bool hasChangedTeacher;

    [Header("UIクラスの参照")]
    [SerializeField] private QuestionUI gameUI;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private Image messagePanelImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI stressPercentageText;

    [Header("ストレスゲージ")]
    [SerializeField] private StressGauge stressGauge;
    [SerializeField, Min(1)] private int maxStress = 100;
    [Tooltip("AIが失敗したときに増えるストレス値")]
    [SerializeField, Min(0)] private int aiFailureStress = 50;
    [Tooltip("AIが成功したときに減るストレス値")]
    [SerializeField, Min(0)] private int aiSuccessRecovery = 20;

    [Header("AI結果メッセージ")]
    [SerializeField] private string FailureMessage = "失敗！";
    [SerializeField] private string SuccessMessage = "成功！";
    [SerializeField] private Color aiFailurePanelColor = new Color32(217, 51, 51, 242);
    [SerializeField] private Color aiSuccessPanelColor = new Color32(51, 191, 77, 242);
    private int currentStress;

    [Header("画面遷移")]
    [SerializeField] private string resultSceneName = "Result";
    void Awake()
    {
        QuestionSO[] loadedQuestions = Resources.LoadAll<QuestionSO>("Questions");
        questionDatabase.AddRange(loadedQuestions);
    }
    void Start()
    {
        if (messagePanelImage == null && messagePanel != null)
        {
            messagePanelImage = messagePanel.GetComponent<Image>();
        }
        if (messageText == null && messagePanel != null)
        {
            messageText = messagePanel.GetComponentInChildren<TextMeshProUGUI>(true);
        }
        if (stressGauge == null)
        {
            stressGauge = FindAnyObjectByType<StressGauge>();
        }
        currentStress = 0;
        UpdateStressGauge();
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
        ResetChangeTeacherButton();
    }
    /// <summary>
    /// AIに聞くボタンが押された
    /// </summary>
    public void OnAIButton()
    {
        if (isGameOver || isMessagePanelOpen || currentQuestion == null) return;
        int randomValue = Random.Range(0, 100);
        bool aiFailed = randomValue < currentQuestion.failureChance;
        if (aiFailed)
        {
            Debug.Log(
                $"失敗！確率 {currentQuestion.failureChance}% に対し " +
                $"{randomValue} を引いた"
            );
        }
        else
        {
            Debug.Log($"成功！確率 {currentQuestion.failureChance}% に対し {randomValue} で回避");
        }
        int stressChange = aiFailed ? aiFailureStress : -aiSuccessRecovery;
        SetAIResultMessage(aiFailed, stressChange);
        if (ChangeStress(stressChange)) return;
        teacherManager.ChengeRandomTeacher();
        teacherManager.ShowRandomTeachers();
        OpenMessagePanel();
    }
    /// <summary>
    /// 現在の先生に問題を解いてもらう
    /// </summary>
    public void OnTeacherButton()
    {
        if (isGameOver || isMessagePanelOpen || currentQuestion == null) return;

        if (teacherManager == null)
        {
            Debug.LogError("TeacherManagerが設定されていません。", this);
            return;
        }

        TeacherSO selectedTeacher = teacherManager.GetCurrentTeacher();
        if (selectedTeacher == null)
        {
            Debug.LogError("現在の先生が設定されていません。", this);
            return;
        }

        // QuestionSOのCorrect Teacherに登録された先生だけが成功する。
        teacherFailed = !teacherManager.IsCorrectTeacher(currentQuestion);

        if (teacherFailed)
        {
            Debug.Log($"{selectedTeacher.teacherName}先生は正解できる先生ではないので失敗！");
        }
        else
        {
            Debug.Log($"{selectedTeacher.teacherName}先生は正解できる先生なので必ず成功！");
        }

        int stressIncrease = Mathf.Max(0,
            teacherFailed ? selectedTeacher.missStlessUp : selectedTeacher.correctStlessUp);

        // string resultMessage = teacherFailed
        //     ? currentQuestion.missTeacherComment
        //     : currentQuestion.correctTeacherComment;

        // if (string.IsNullOrWhiteSpace(resultMessage))
        // {
        //     resultMessage = teacherFailed
        //         ? $"{selectedTeacher.teacherName}先生が失敗した！"
        //         : $"{selectedTeacher.teacherName}先生が成功した！";
        // }

        bool reachedMaxStress = ChangeStress(stressIncrease);
        SetTeacherResultMessage(teacherFailed, stressIncrease);

        // 判定後、次のターンで使う先生へ切り替える。
        teacherManager.ChengeRandomTeacher();
        teacherManager.ShowRandomTeachers();

        if (reachedMaxStress) return;

        OpenMessagePanel();
    }
    /// <summary>
    /// 下3人から先生をランダムで1人選ぶ
    /// 各問題につき1回だけ使用できる
    /// </summary>
    public void OnChangeTeacherButton()
    {
        if (isGameOver || isMessagePanelOpen) return;
        if (hasChangedTeacher) return;
        teacherManager.SelectRandomSlotTeacher();
        hasChangedTeacher = true;
        changeTeacherButton.interactable = false;
        return;
    }
    /// <summary>
    /// 次の問題になったとき、先生変更ボタンを再使用可能にする
    /// </summary>
    private void ResetChangeTeacherButton()
    {
        hasChangedTeacher = false;
        if (changeTeacherButton != null)
        {
            changeTeacherButton.interactable = true;
        }
        return;
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
    /// <summary>
    /// ストレスを増減し、最大値に達したらリザルトへ移動する。
    /// </summary>
    private bool ChangeStress(int amount)
    {
        currentStress = Mathf.Clamp(currentStress + amount, 0, maxStress);
        UpdateStressGauge();
        Debug.Log($"ストレス: {currentStress}/{maxStress}");
        if (currentStress < maxStress) return false;
        isGameOver = true;
        SceneManager.LoadScene(resultSceneName);
        return true;
    }
    private void UpdateStressGauge()
    {
        if (stressGauge == null)
        {
            Debug.LogError("StressGaugeが見つかりません。", this);
        }
        else
        {
            stressGauge.SetStress(currentStress, maxStress);
        }
        if (stressPercentageText != null)
        {
            int stressPercentage = Mathf.RoundToInt((float)currentStress / maxStress * 100f);
            stressPercentageText.text = $"{stressPercentage}%";
        }
    }
    private void OpenMessagePanel()
    {
        if (messagePanel == null)
        {
            Debug.LogError("MessagePanelが設定されていません。次の問題には進みません。", this);
            return;
        }
        isMessagePanelOpen = true;
        messagePanel.SetActive(true);
    }
    private void SetAIResultMessage(bool aiFailed, int stressChange)
    {
        if (messageText == null)
        {
            Debug.LogError("MessageTextが設定されていません。", this);
            return;
        }

        string resultMessage = aiFailed ? FailureMessage : SuccessMessage;

        string comment = "";

        if (commentMaager != null)
        {
            comment = commentMaager.GetAIComment(currentQuestion, aiFailed);
        }

        if (!string.IsNullOrEmpty(comment))
        {
            messageText.text = $"{resultMessage}\n{comment}\nストレス {stressChange:+#;-#;0}%";
        }
        else
        {
            messageText.text = $"{resultMessage}\nストレス {stressChange:+#;-#;0}%";
        }

        if (messagePanelImage != null)
        {
            messagePanelImage.color = aiFailed ? aiFailurePanelColor : aiSuccessPanelColor;
        }
    }

    private void SetTeacherResultMessage(bool teacherFailed, int stressChange)
    {
        if (messageText == null)
        {
            Debug.Log("messageTextがnullです");
            return;
        }

        string resultMessage = teacherFailed ? FailureMessage : SuccessMessage;

        string comment = "";
        if (commentMaager != null)
        {
            comment = commentMaager.GetTeacherComment(currentQuestion, teacherFailed);
        }

        Debug.Log(comment);
        if (!string.IsNullOrEmpty(comment))
        {
            messageText.text = $"{resultMessage}\n{comment}\nストレス {stressChange:+#;-#;0}%";
        }
        else
        {
            messageText.text = $"{resultMessage}\nストレス {stressChange:+#;-#;0}%";
        }

        if (messagePanelImage != null)
        {
            messagePanelImage.color = teacherFailed ? aiFailurePanelColor : aiSuccessPanelColor;
        }
    }

}
