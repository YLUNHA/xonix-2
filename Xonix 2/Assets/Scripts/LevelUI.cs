using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUI : MonoBehaviour
{
    public static LevelUI i;

    private void Awake()
    {
        i = this;
    }

    // ----------------------

    [Space]
    public GameController gameController;

    [Header("Game")]
    public GameObject pauseButton;
    public GameObject[] hearts;

    public GameObject timerText;
    public TextMeshProUGUI timerTextMesh;

    public GameObject levelProgress;
    public Slider levelProgressSlider;
    public Animator levelProgressAnimator;

    public GameObject lifePanel;
    public Animator lifePanelAnimator;

    [Header("Game over")]
    public GameObject gameOverPanel;

    [Header("Level complete")]
    public GameObject levelCmpletePanel;

    // ----------------------

    public void GameStatus(bool play)
    {
        gameController.PauseGame(!play);
    }

    public void QuitLevel()
    {
        gameController.CloseLevel();
    }

    public void LoadNextLevel()
    {
        gameController.LoadNextLevel();
    }

    public void HideHeart(byte heartsHide)
    {
        RunLifeAnimation();

        for (int i = 0; i < (hearts.Length - heartsHide); i++)
        {
            hearts[i].SetActive(false);
        }
    }


    private void RunLifeAnimation()
    {
        lifePanelAnimator.SetBool("Animate", true);
        Invoke("StopLifeAnimation", 1f);
    }

    private void StopLifeAnimation()
    {
        lifePanelAnimator.SetBool("Animate", false);
    }

    private void RunLevelProgressAnimation()
    {
        levelProgressAnimator.SetBool("Animate", true);
        Invoke("StopProgressAnimation", 1f);
    }

    private void StopProgressAnimation()
    {
        levelProgressAnimator.SetBool("Animate", false);
    }


    public static void ShowGameOver()
    {
        i.pauseButton.SetActive(false);
        i.gameOverPanel.SetActive(true);
    }

    public static void ShowLevelComplete()
    {
        i.pauseButton.SetActive(false);
        i.levelCmpletePanel.SetActive(true);
    }

    internal static void SetTimer(float levelTimer)
    {
        i.timerTextMesh.text = levelTimer.ToString("0");
    }

    internal static void SetLevelProgress(float progress)
    {
        i.RunLevelProgressAnimation();
        i.levelProgressSlider.value = progress;
    }
}
