using UnityEngine;

public class TeacherManager : MonoBehaviour
{
    // 現在選ばれている先生
    [SerializeField] private TeacherSO currentTeacher;
    [SerializeField] private TeacherUI teacherUI;
    private void Start()
    {
        teacherUI.UpdateTeacherUI(currentTeacher);
    }
    /// <summary>
    /// 現在選ばれている先生を取得する
    /// </summary>
    public TeacherSO GetCurrentTeacher()
    {
        return currentTeacher;
    }

    /// <summary>
    /// 現在の先生を変更する
    /// </summary>
    public void SetCurrentTeacher(TeacherSO teacher)
    {
        if (teacher == null)
        {
            Debug.LogWarning("変更する先生が設定されていません");
            return;
        }

        currentTeacher = teacher;

        Debug.Log($"{currentTeacher.teacherName}に変更しました");
        teacherUI.UpdateTeacherUI(currentTeacher);
    }


    /// <summary>
    /// 現在の先生が問題の得意な先生か判定する
    /// </summary>
    public bool IsCorrectTeacher(QuestionSO question)
    {
        if (currentTeacher == null)
        {
            Debug.LogWarning("現在の先生が設定されていません");
            return false;
        }

        if (question == null)
        {
            Debug.LogWarning("問題が設定されていません");
            return false;
        }

        return currentTeacher == question.correctTeacher;
    }
}