using UnityEngine;

[CreateAssetMenu(fileName ="Teacher" ,menuName = "Game/CreateTeacher")]
public class TeacherSO : ScriptableObject
{
    public string teacherName; //先生の名前
    public string hobby; // 得意なこと
    public Sprite teacherImage; //先生の画像

    [Header("苦手な問題")]
    [Tooltip("この先生が必ず失敗するQuestionを登録する")]
    [SerializeField] private QuestionSO[] weakQuestions;

    [Header("ストレス増加量")]
    [Min(0)]
    public int correctStlessUp; //成功時ストレスアップ
    [Min(0)]
    public int missStlessUp; //失敗時ストレスアップ
    public int efficiencyUp; //効率アップ
    public int efficiencyDown; //効率ダウン
    public int intimacy; //親密度

    public bool IsWeakQuestion(QuestionSO question)
    {
        if (question == null || weakQuestions == null) return false;

        foreach (QuestionSO weakQuestion in weakQuestions)
        {
            if (weakQuestion == question) return true;
        }

        return false;
    }
}
