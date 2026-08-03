using System.Collections.Generic;
using UnityEngine;

public class TeacherManager : MonoBehaviour
{
    private List<TeacherSO> teacherDatabase = new List<TeacherSO>();
    
    public TeacherSO CurrentTeacher { get; private set; }
    private List<TeacherSO> changeCandidates = new List<TeacherSO>();

    private bool hasChangedThisTurn = false;

    [Header("対応するUIクラス")]
    [SerializeField] private TeacherUI teacherUI;

    // 🚀 インスペクターからチェンジ候補の人数を設定できるようにする（初期値は3人）
    [Header("ゲームバランス調整")]
    [Range(1, 5)] // 最低1人〜最大5人までに制限（必要に応じて数値は変えてOK）
    [SerializeField] private int maxChangeCandidates = 3;

    void Awake()
    {
        TeacherSO[] loadedTeachers = Resources.LoadAll<TeacherSO>("system/Teachers");
        teacherDatabase.AddRange(loadedTeachers);
    }

    public void SetupInitialTeacherForTurn()
    {
        if (teacherDatabase.Count == 0) return;

        hasChangedThisTurn = false;
        teacherUI.SetChangeButtonInteractable(true);

        int randomIndex = Random.Range(0, teacherDatabase.Count);
        CurrentTeacher = teacherDatabase[randomIndex];

        // チェンジ候補を作る
        SetupChangeCandidates(CurrentTeacher);

        teacherUI.UpdateTeacherDisplay(CurrentTeacher);
    }

    private void SetupChangeCandidates(TeacherSO excludedTeacher)
    {
        changeCandidates.Clear();

        List<TeacherSO> tempPool = new List<TeacherSO>(teacherDatabase);
        tempPool.Remove(excludedTeacher);

        // 🚀 固定の「3」ではなく、インスペクターで設定した「maxChangeCandidates」を使う
        // ※ただし、登録されている先生の総数を超えないように Mathf.Min で安全弁をかけます
        int candidateCount = Mathf.Min(maxChangeCandidates, tempPool.Count); 
        
        for (int i = 0; i < candidateCount; i++)
        {
            int randomIndex = Random.Range(0, tempPool.Count);
            changeCandidates.Add(tempPool[randomIndex]);
            tempPool.RemoveAt(randomIndex);
        }
    }

    public void ChangeTeacher()
    {
        if (hasChangedThisTurn || changeCandidates.Count == 0) return;

        int randomIndex = Random.Range(0, changeCandidates.Count);
        CurrentTeacher = changeCandidates[randomIndex];
        
        hasChangedThisTurn = true;
        
        teacherUI.UpdateTeacherDisplay(CurrentTeacher);
        teacherUI.SetChangeButtonInteractable(false);

        Debug.Log($"先生をチェンジしました: {CurrentTeacher.teacherName} (候補数: {changeCandidates.Count}人の中から選出)");
    }
}