using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static bool isPause;

    public SoundController soundController;

    private void Awake()
    {
        PauseGame(true);
    }

    public void PauseGame(bool pause)
    {
        isPause = pause;
        Time.timeScale = isPause ? 0f : 1f;
    }

    public void LevelComplete()
    {
        PauseGame(true);
        LevelUI.ShowLevelComplete();
        soundController.LevelComplete();
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GameOver()
    {
        PauseGame(true);
        LevelUI.ShowGameOver();
        soundController.GameOver();
    }

    public void CloseLevel()
    {
        SceneManager.LoadScene(0);
    }


}
