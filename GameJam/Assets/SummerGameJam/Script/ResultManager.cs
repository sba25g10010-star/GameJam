using UnityEngine;
using TMPro;

public class ResultManager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI questionCountText;
    void Start()
    {
        questionCountText.text = $"{ResultData.SolvedQuestionCount}問";
    }
}
