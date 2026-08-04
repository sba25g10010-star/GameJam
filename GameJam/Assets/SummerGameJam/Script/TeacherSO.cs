using UnityEngine;

[CreateAssetMenu(fileName ="Teacher" ,menuName = "Game/CreateTeacher")]
public class TeacherSO : ScriptableObject
{
    public string teacherName; //先生の名前
    public string hobby; // 得意なこと
    public Sprite teacherImage; //先生の画像

    [Header("ストレス増加量")]
    [Min(0)]
    public int correctStlessUp; //成功時ストレスアップ
    [Min(0)]
    public int missStlessUp; //失敗時ストレスアップ
    public int efficiencyUp; //効率アップ
    public int efficiencyDown; //効率ダウン
    public int intimacy; //親密度
}
