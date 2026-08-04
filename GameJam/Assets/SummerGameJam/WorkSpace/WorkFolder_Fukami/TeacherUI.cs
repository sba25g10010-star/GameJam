using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeacherUI : MonoBehaviour
{
    [Header("先生の指示UI")]
[SerializeField]private Image teacherImage;
[SerializeField]private TextMeshProUGUI teacherNameText;
[SerializeField]private TextMeshProUGUI  hobbyText;
/// <summary>
    /// 先生の情報をUIに表示する
    /// </summary>
    public void UpdateTeacherUI(TeacherSO teacher)
    {
        if (teacher == null)
        {
            Debug.LogWarning("表示する先生が設定されていません");
            return;
        }

        teacherImage.sprite = teacher.teacherImage;
        teacherNameText.text = teacher.teacherName;
        hobbyText.text = teacher.hobby;
    }
}
