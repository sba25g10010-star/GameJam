using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class TeacherUI : MonoBehaviour
{
[Header("先生の指示UI")]
[SerializeField]private Image teacherImage;
[SerializeField]private TextMeshProUGUI teacherNameText;
[SerializeField]private TextMeshProUGUI  hobbyText;

[Header("下に表示する先生3人")]
[SerializeField] private Image[] teacherSlotImages;
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
    public void UpdateTeacherSlots(TeacherSO[]teachers)
    {
        if(teachers==null)return;
        for(int i=0;i< teacherSlotImages.Length;i++)
        {
            if(i<teachers.Length&&teachers[i]!=null)
            {
                teacherSlotImages[i].sprite=teachers[i].teacherImage;
                
            }
        }
    } 
}
