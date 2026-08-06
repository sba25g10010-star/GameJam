using UnityEngine;

public class CommentMaager : MonoBehaviour
{
    public string GetAIComment(QuestionSO question, bool isFailed)
    {
        if (question == null) return string.Empty;

        return isFailed ? question.missAIComment : question.correctAIComment;
    }

    public string GetTeacherComment(QuestionSO question, bool isFailed)
    {
        if (question == null) return string.Empty;

        return isFailed ? question.missTeacherComment : question.correctTeacherComment;
    }
}
