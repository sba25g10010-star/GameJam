using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialImage;
    [SerializeField] private GameObject closeButton;


    void Start()
    {

        // 一度ゲームを開始していたら最初から非表示
        if (TitleData.HasPlayed)
        {
            tutorialImage.SetActive(false);
            closeButton.SetActive(false);
        }
        else
        {
            tutorialImage.SetActive(true);
            closeButton.SetActive(true);
        }
    }

    public void Close()
    {
        // 一度閉じたことを記録
        TitleData.HasPlayed = true;

        tutorialImage.SetActive(false);
        closeButton.SetActive(false);
    }
}