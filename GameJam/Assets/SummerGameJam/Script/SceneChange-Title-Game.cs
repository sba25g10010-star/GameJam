using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // ゲーム画面へ移動
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    // リザルト画面に移動
    public void LoadResult()
    {
        SceneManager.LoadScene("Result");
    }

    // タイトル画面に移動
    public void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }
}