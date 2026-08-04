using System.Collections.Generic;
using UnityEngine;

public class TeacherManager : MonoBehaviour
{
    // 現在選ばれている先生
    [Header("先生一覧")]
    [SerializeField] private TeacherSO[] teachers;
    [SerializeField] private TeacherSO currentTeacher;
    [SerializeField] private TeacherUI teacherUI;
    private TeacherSO[] currentSlotTeachers = new TeacherSO[3];
    private void Start()
    {
        ChengeRandomTeacher();
        ShowRandomTeachers();
    }
    /// <summary>
    /// 下の3か所にランダムな先生を表示する
    /// </summary>
    public void ShowRandomTeachers()
    {
        List<TeacherSO> teacherList = new List<TeacherSO>();

        foreach (TeacherSO teacher in teachers)
        {
            if (teacher == null) continue;

            // 上に表示中の先生は候補から外す
            if (teacher == currentTeacher) continue;

            // 同じTeacherSOを重複登録しない
            if (teacherList.Contains(teacher)) continue;

            teacherList.Add(teacher);
        }

        if (teacherList.Count < 3)
        {
            Debug.LogWarning(
                "上の先生以外に、別々の先生を3人以上登録してください"
            );
            return;
        }

        TeacherSO[] selectedTeachers = new TeacherSO[3];

        for (int i = 0; i < selectedTeachers.Length; i++)
        {
            int randomIndex = Random.Range(0, teacherList.Count);

            selectedTeachers[i] = teacherList[randomIndex];

            // 選ばれた先生は候補から削除
            teacherList.RemoveAt(randomIndex);
        }

        currentSlotTeachers = selectedTeachers;
        teacherUI.UpdateTeacherSlots(selectedTeachers);
    }
    public void ChengeRandomTeacher()
    {
        if (teachers == null || teachers.Length == 0)
        {
            Debug.LogWarning("先生が登録されてません");
            return;
        }
        int randomIndex = Random.Range(0, teachers.Length);
        currentTeacher = teachers[randomIndex];
        teacherUI.UpdateTeacherUI(currentTeacher);
        Debug.Log($"{currentTeacher.teacherName}に変更しました");
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
    private bool IsSameTeachers(TeacherSO[] newTeachers)
    {
        for (int i = 0; i < currentSlotTeachers.Length; i++)
        {
            if (currentSlotTeachers[i] != newTeachers[i])
            {
                return false;
            }
        }

        return true;
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