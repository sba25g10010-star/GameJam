using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeacherUI : MonoBehaviour
{
    [Header("先生UIコンポーネント")]
    [SerializeField] private TextMeshProUGUI teacherNameText;
    [SerializeField] private Image teacherIconImage;
    [SerializeField] private Button changeButton; // チェンジ用のボタン

    /// <summary>
    /// 画面の先生情報を書き換える
    /// </summary>
    public void UpdateTeacherDisplay(TeacherSO teacher)
    {
        teacherNameText.text = teacher.teacherName;
        teacherIconImage.sprite = teacher.teacherImage;
    }

    /// <summary>
    /// チェンジボタンの押しやすさを切り替える
    /// </summary>
    public void SetChangeButtonInteractable(bool isInteractable)
    {
        changeButton.interactable = isInteractable;
    }
}