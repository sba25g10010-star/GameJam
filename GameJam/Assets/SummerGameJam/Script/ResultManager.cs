using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI solvedQuestionCount;
    [SerializeField] private TextMeshProUGUI questionNumberText;
    void Start()
    {
        questionNumberText.text = $"{ResultData.CurrentQuestionNumber}問やって";
        solvedQuestionCount.text = $"{ResultData.SolvedQuestionCount}問正解できた：";

    }
}
