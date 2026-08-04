using UnityEngine;
public class ExitManager : MonoBehaviour
{
   // ゲーム終了ボタンが押されたときに実行
   public void EndGame()
   {
       #if UNITY_EDITOR
       UnityEditor.EditorApplication.isPlaying = false; // エディター内で再生を停止
       #else
       Application.Quit(); // スタンドアロンビルドでアプリケーションを終了
       #endif
   }
}