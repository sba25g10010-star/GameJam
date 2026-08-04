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

            // 上の先生と同じデータ、または同じ名前の先生は除外
            if (teacher == currentTeacher) continue;

            if (teacher.teacherName == currentTeacher.teacherName)
            {
                continue;
            }

            // 下の候補にも同じ名前の先生を入れない
            bool alreadyExists = teacherList.Exists(
                registeredTeacher =>
                    registeredTeacher.teacherName == teacher.teacherName
            );

            if (alreadyExists) continue;

            teacherList.Add(teacher);
        }

        if (teacherList.Count < 3)
        {
            Debug.LogWarning(
                "上の先生を除いて、名前の違う先生を3人以上登録してください"
            );
            return;
        }

        TeacherSO[] selectedTeachers = new TeacherSO[3];

        for (int i = 0; i < selectedTeachers.Length; i++)
        {
            int randomIndex = Random.Range(0, teacherList.Count);

            selectedTeachers[i] = teacherList[randomIndex];
            teacherList.RemoveAt(randomIndex);
        }

        currentSlotTeachers = selectedTeachers;
        teacherUI.UpdateTeacherSlots(currentSlotTeachers);

        Debug.Log($"上：{currentTeacher.teacherName}");

        for (int i = 0; i < currentSlotTeachers.Length; i++)
        {
            Debug.Log($"下{i + 1}：{currentSlotTeachers[i].teacherName}");
        }
    }
    /// <summary>
    /// 下に表示されている3人から、1人をランダムで選ぶ
    /// </summary>
    public void SelectRandomSlotTeacher()
    {
        int randomIndex = Random.Range(0, currentSlotTeachers.Length);

        TeacherSO selectedTeacher = currentSlotTeachers[randomIndex];

        // 先に上を変更
        currentTeacher = selectedTeacher;
        teacherUI.UpdateTeacherUI(currentTeacher);

        // 新しい上の先生を除外して下3人を再抽選
        ShowRandomTeachers();
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

        if(question.correctTeachers == null || question.correctTeachers.Count == 0)return false;

        return question.correctTeachers.Contains(currentTeacher);
    }

}