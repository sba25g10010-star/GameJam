using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class QuestionManager : MonoBehaviour
{
    [System.Flags]
    private enum DeathCause
    {
        None = 0,
        Stress = 1,
        WorkEfficiency = 2
    }

    private List<QuestionSO> questionDatabase = new List<QuestionSO>();
    private QuestionSO currentQuestion;

    private bool teacherFailed = false;
    private bool isGameOver = false;
    private bool isDeathMessagePanelOpen = false;
    [Header("先生管理クラスの参照")]
    [SerializeField] private TeacherManager teacherManager;
    [Header("問題数")]
    [SerializeField] private TextMeshProUGUI questionNumberText;
    private int currentQuestionNumber = 0; // 表示用（第○問）
    private int solvedQuestionCount = 0;     // 成功した問題数

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
    [SerializeField] private MessagePanelContent messagePanelContent;
    [SerializeField] private TextMeshProUGUI stressPercentageText;
    [SerializeField] private TextMeshProUGUI workPercentageText;

    [Header("死亡パネル")]
    [Tooltip("未設定の場合は、生存時のMessagePanelを実行時に複製します。")]
    [SerializeField] private GameObject deathMessagePanel;
    [SerializeField] private MessagePanelContent deathMessagePanelContent;
    [SerializeField] private Image deathMessagePanelImage;
    [SerializeField] private string deathResultMessage = "死亡";
    [SerializeField] private Color deathPanelColor = new Color32(90, 20, 20, 242);

    [Header("ストレスによる死亡メッセージ候補")]
    [SerializeField, TextArea]
    private string[] stressDeathMessages =
    {
        "ストレスを溜めすぎて頭が爆発した",
        "耐えきれなくなった"
    };

    [Header("作業効率による死亡メッセージ候補")]
    [SerializeField, TextArea]
    private string[] workEfficiencyDeathMessages =
    {
        "全然作業が進まなかった",
        "作業が進まなかったので、逃げることにした"
    };

    [Header("両方による死亡メッセージ候補")]
    [SerializeField, TextArea]
    private string[] bothDeathMessages =
    {
        "何もかもが嫌になった",
        "もう全てを諦めることにした"
    };

    [Header("ストレスゲージ")]
    [SerializeField] private StressGauge stressGauge;
    [SerializeField, Min(1)] private int maxStress = 100;
    [Tooltip("AIが失敗したときに増えるストレス値")]
    [SerializeField, Min(0)] private int aiFailureStress = 50;
    [Tooltip("AIが成功したときに減るストレス値")]
    [SerializeField, Min(0)] private int aiSuccessRecovery = 20;

    [Header("作業効率ゲージ")]
    [SerializeField] private WorkGauge workGauge;
    [SerializeField, Range(0, 100)] private int initialWorkEfficiency = 75;
    [SerializeField, Min(0)] private int aiSuccessEfficiency = 25;
    [SerializeField, Min(0)] private int aiFailureEfficiency = 25;

    [Header("AI結果メッセージ")]
    [SerializeField] private string FailureMessage = "失敗！";
    [SerializeField] private string SuccessMessage = "成功！";
    [SerializeField] private Color aiFailurePanelColor = new Color32(217, 51, 51, 242);
    [SerializeField] private Color aiSuccessPanelColor = new Color32(51, 191, 77, 242);
    private int currentStress;
    private int currentWorkEfficiency;

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
        if (messagePanelContent == null && messagePanel != null)
        {
            messagePanelContent = messagePanel.GetComponentInChildren<MessagePanelContent>(true);
        }

        InitializeDeathMessagePanel();

        if (stressGauge == null)
        {
            stressGauge = FindAnyObjectByType<StressGauge>();
        }

        if (workGauge == null)
        {
            workGauge = FindAnyObjectByType<WorkGauge>();
        }

        currentStress = 0;
        UpdateStressGauge();
        currentWorkEfficiency = Mathf.Clamp(initialWorkEfficiency, 0, 100);
        UpdateWorkGauge();

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
        currentQuestionNumber = 0;

        if (questionNumberText != null)
        {
            questionNumberText.text = "";
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
        currentQuestionNumber++;
        if (questionNumberText != null)
        {
            questionNumberText.text = $"第{currentQuestionNumber}問";
        }
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
        if (!aiFailed)
        {
            solvedQuestionCount++;
        }
        int stressChange = aiFailed ? aiFailureStress : -aiSuccessRecovery;
        int workEfficiencyChange = aiFailed ? -aiFailureEfficiency : aiSuccessEfficiency;

        int prevStress = currentStress;
        int prevEfficiency = currentWorkEfficiency;


        ChangeWorkEfficiency(workEfficiencyChange);
        bool reachedMax = ChangeStress(stressChange);

        DeathCause deathCause = GetDeathCause();
        if (deathCause != DeathCause.None)
        {
            OpenDeathMessagePanel(deathCause, stressChange, workEfficiencyChange);
            return;
        }


        OpenMessagePanel();
        SetAIResultMessage(aiFailed, stressChange, workEfficiencyChange, prevStress, prevEfficiency);

        if (reachedMax) return;

        teacherManager.ChengeRandomTeacher();
        teacherManager.ShowRandomTeachers();
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
        if (!teacherFailed)
        {
            solvedQuestionCount++;
        }

        int stressIncrease = Mathf.Max(0,
            teacherFailed ? selectedTeacher.missStlessUp : selectedTeacher.correctStlessUp);

        int workEfficiencyChange = teacherFailed
            ? -Mathf.Max(0, selectedTeacher.efficiencyDown)
            : Mathf.Max(0, selectedTeacher.efficiencyUp);

        int prevStress = currentStress;
        int prevEfficiency = currentWorkEfficiency;

        ChangeWorkEfficiency(workEfficiencyChange);

        bool reachedMaxStress = ChangeStress(stressIncrease);

        DeathCause deathCause = GetDeathCause();
        if (deathCause != DeathCause.None)
        {
            OpenDeathMessagePanel(deathCause, stressIncrease, workEfficiencyChange);
            return;
        }



        OpenMessagePanel();
        SetTeacherResultMessage(
                    teacherFailed,
                    stressIncrease,
                    workEfficiencyChange,
                    prevStress,
                    prevEfficiency);

        // 判定後、次のターンで使う先生へ切り替える。
        teacherManager.ChengeRandomTeacher();
        teacherManager.ShowRandomTeachers();

        OpenMessagePanel();
        if (reachedMaxStress) return;

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

        if (isDeathMessagePanelOpen)
        {
            TriggerGameOver();
            return;
        }

        messagePanel.SetActive(false);
        isMessagePanelOpen = false;
        NextTurn();
    }
    /// <summary>
    /// ストレスを増減する。
    /// </summary>
    private bool ChangeStress(int amount)
    {
        currentStress = Mathf.Clamp(currentStress + amount, 0, maxStress);
        UpdateStressGauge();
        Debug.Log($"ストレス: {currentStress}/{maxStress}");
        if (currentStress < maxStress) return false;
        ResultData.SolvedQuestionCount =solvedQuestionCount;
        ResultData.CurrentQuestionNumber =currentQuestionNumber;
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

    /// <summary>
    /// 作業効率を増減する。
    /// </summary>
    private void ChangeWorkEfficiency(int amount)
    {
        currentWorkEfficiency = Mathf.Clamp(currentWorkEfficiency + amount, 0, 100);
        UpdateWorkGauge();

        Debug.Log($"作業効率: {currentWorkEfficiency}%");
    }

    private DeathCause GetDeathCause()
    {
        DeathCause cause = DeathCause.None;

        if (currentStress >= maxStress)
        {
            cause |= DeathCause.Stress;
        }

        if (currentWorkEfficiency <= 0)
        {
            cause |= DeathCause.WorkEfficiency;
        }

        return cause;
    }

    /// <summary>
    /// 専用パネルが未設定なら、生存時のパネルを複製して同じ配置を引き継ぐ。
    /// </summary>
    private void InitializeDeathMessagePanel()
    {
        if (deathMessagePanel == null && messagePanel != null)
        {
            deathMessagePanel = Instantiate(
                messagePanel,
                messagePanel.transform.parent,
                false);
            deathMessagePanel.name = "DeathMessagePanel";
            deathMessagePanel.transform.SetSiblingIndex(
                messagePanel.transform.GetSiblingIndex() + 1);
        }

        if (deathMessagePanel == null)
        {
            Debug.LogError("死亡パネルを作成できません。MessagePanelを設定してください。", this);
            return;
        }

        if (deathMessagePanelContent == null)
        {
            deathMessagePanelContent =
                deathMessagePanel.GetComponentInChildren<MessagePanelContent>(true);
        }

        if (deathMessagePanelImage == null)
        {
            deathMessagePanelImage = deathMessagePanel.GetComponent<Image>();
        }

        deathMessagePanel.SetActive(false);
    }

    private void OpenDeathMessagePanel(
        DeathCause cause,
        int stressChange,
        int workEfficiencyChange)
    {
        if (deathMessagePanel == null || deathMessagePanelContent == null)
        {
            Debug.LogError("死亡パネルの表示先が設定されていません。", this);
            TriggerGameOver();
            return;
        }

        string deathMessage = GetRandomDeathMessage(cause);
        deathMessagePanelContent.DeathSetContent(
            deathResultMessage,
            deathMessage,
            stressChange,
            workEfficiencyChange);

        if (deathMessagePanelImage != null)
        {
            deathMessagePanelImage.color = deathPanelColor;
        }

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        isDeathMessagePanelOpen = true;
        isMessagePanelOpen = true;
        deathMessagePanel.SetActive(true);
    }

    private string GetRandomDeathMessage(DeathCause cause)
    {
        if (cause == (DeathCause.Stress | DeathCause.WorkEfficiency))
        {
            return PickRandomMessage(
                bothDeathMessages,
                "ストレスと作業効率が最大になった。"
            );
        }

        if (cause == DeathCause.Stress)
        {
            return PickRandomMessage(
                stressDeathMessages,
                "ストレスが最大になった。"
            );
        }

        return PickRandomMessage(
            workEfficiencyDeathMessages,
            "作業効率が0になった。"
        );
    }

    private static string PickRandomMessage(string[] messages, string fallbackMessage)
    {
        if (messages == null || messages.Length == 0)
        {
            return fallbackMessage;
        }

        int validMessageCount = 0;
        for (int index = 0; index < messages.Length; index++)
        {
            if (!string.IsNullOrWhiteSpace(messages[index]))
            {
                validMessageCount++;
            }
        }

        if (validMessageCount == 0)
        {
            return fallbackMessage;
        }

        int selectedMessageIndex = Random.Range(0, validMessageCount);
        for (int index = 0; index < messages.Length; index++)
        {
            if (string.IsNullOrWhiteSpace(messages[index]))
            {
                continue;
            }

            if (selectedMessageIndex == 0)
            {
                return messages[index];
            }

            selectedMessageIndex--;
        }

        return fallbackMessage;
    }

    private bool TriggerGameOver()
    {
        if (isGameOver) return false;
        ResultData.CurrentQuestionNumber =currentQuestionNumber;
        ResultData.SolvedQuestionCount = solvedQuestionCount;
        isGameOver = true;
        SceneManager.LoadScene(resultSceneName);
        return true;
    }

    private void UpdateWorkGauge()
    {
        if (workGauge == null)
        {
            Debug.LogError("WorkGaugeが見つかりません。", this);
        }
        else
        {
            workGauge.SetEfficiency(currentWorkEfficiency);
        }

        if (workPercentageText != null)
        {
            workPercentageText.text = $"{currentWorkEfficiency}%";
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

    private void SetAIResultMessage(bool aiFailed, int stressChange, int workEfficiencyChange, int prevStress,
        int prevEfficiency)
    {
        if (messagePanelContent != null)
        {
            string panelResult = aiFailed ? FailureMessage : SuccessMessage;
            string panelComment = commentMaager != null
                ? commentMaager.GetAIComment(currentQuestion, aiFailed)
                : string.Empty;

            UpdateMessagePanelContent(
                            panelResult,
                            panelComment,
                            stressChange,
                            workEfficiencyChange,
                            prevStress,
                            currentStress,
                            prevEfficiency,
                            currentWorkEfficiency);

            if (messagePanelImage != null)
            {
                messagePanelImage.color = aiFailed ? aiFailurePanelColor : aiSuccessPanelColor;
            }

            return;
        }
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

        messageText.text += $"\n作業効率 {workEfficiencyChange:+#;-#;0}%";

        if (messagePanelImage != null)
        {
            messagePanelImage.color = aiFailed ? aiFailurePanelColor : aiSuccessPanelColor;
        }
    }

    private void SetTeacherResultMessage(
            bool teacherFailed,
            int stressIncrease,
            int workEfficiencyChange,
            int prevStress,
            int prevEfficiency)
    {
        if (messagePanelContent != null)
        {
            string panelResult = teacherFailed ? FailureMessage : SuccessMessage;
            string panelComment = commentMaager != null
                ? commentMaager.GetTeacherComment(currentQuestion, teacherFailed)
                : string.Empty;

            UpdateMessagePanelContent(
                panelResult,
                panelComment,
                stressIncrease,
                workEfficiencyChange,
                prevStress,
                currentStress,
                prevEfficiency,
                currentWorkEfficiency);

            if (messagePanelImage != null)
            {
                messagePanelImage.color = teacherFailed ? aiFailurePanelColor : aiSuccessPanelColor;
            }

            return;
        }
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
            messageText.text = $"{resultMessage}\n{comment}\nストレス {stressIncrease:+#;-#;0}%";
        }
        else
        {
            messageText.text = $"{resultMessage}\nストレス {stressIncrease:+#;-#;0}%";
        }

        messageText.text += $"\n作業効率 {workEfficiencyChange:+#;-#;0}%";

        if (messagePanelImage != null)
        {
            messagePanelImage.color = teacherFailed ? aiFailurePanelColor : aiSuccessPanelColor;
        }
    }

    private void UpdateMessagePanelContent(
string resultMessage,
        string comment,
        int stressChange,
        int workEfficiencyChange,
        int previousStress,
        int currentStress,
        int previousEfficiency,
        int currentEfficiency)
    {
        if (messagePanelContent == null)
        {
            Debug.LogError("MessagePanelContentが設定されていません。", this);
            return;
        }

        messagePanelContent.SetContent(
                    resultMessage,
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
