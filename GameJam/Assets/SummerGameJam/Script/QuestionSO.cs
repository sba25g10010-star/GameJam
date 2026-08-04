using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Question" ,menuName = "Game/CreateQuestion")]
public class QuestionSO : ScriptableObject
{
    public string questionName; //問題の名前

    [Range(0,100)]
    public int failureChance; //問題が失敗する確率
    public List<TeacherSO> correctTeachers = new List<TeacherSO>();
    public string correctAIComment;
    public string missAIComment;
    public string correctTeacherComment;
    public string missTeacherComment;
}
