using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static bool isPause;

    public SoundController soundController;

    [Space]
    public PlayerMovement playerMovement;
    public TilemapUtil tilemapUtil;

    private float levelTimer;
    private int waterEnemiesCount;
    private int groundEnemiesCount;
    private byte playerLife;

    private void Awake()
    {
        PauseGame(true);
        Initial();

        PlayerMovement.OnPlayerDamaged += DecrementPlayerLife;
        TilemapUtil.OnCalcLevelProgress += OnCalcLevelProgress;
        TilemapUtil.OnEnemyDestroy += OnEnemyDestroy;
    }

    private void OnDestroy()
    {
        PlayerMovement.OnPlayerDamaged -= DecrementPlayerLife;
        TilemapUtil.OnCalcLevelProgress -= OnCalcLevelProgress;
        TilemapUtil.OnEnemyDestroy -= OnEnemyDestroy;
    }

    private void Update()
    {
        UpdateTimer();

    }


    private void Initial()
    {
        waterEnemiesCount = 0;
        groundEnemiesCount = 0;
        levelTimer = 60;
        playerLife = 3;

        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            switch (enemy.GetComponent<EnemyMovement>().characterType)
            {
                case CharacterType.EnemyWater:
                    waterEnemiesCount++;
                    break;
                case CharacterType.EnemyGround:
                    groundEnemiesCount++;
                    break;
            }
        }

    }


    public void PauseGame(bool pause)
    {
        isPause = pause;
        Time.timeScale = isPause ? 0f : 1f;
    }

    private void UpdateTimer()
    {
        levelTimer -= Time.deltaTime;

        if (levelTimer <= 0)
        {
            GameOver();
        }

        LevelUI.SetTimer(levelTimer);
    }


    private void LevelComplete()
    {
        PauseGame(true);
        LevelUI.ShowLevelComplete();
        soundController.LevelComplete();
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void GameOver()
    {
        PauseGame(true);
        LevelUI.ShowGameOver();
        soundController.GameOver();
    }

    public void CloseLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void DecrementPlayerLife()
    {
        if (playerLife > 0)
        {
            playerLife--;
            LevelUI.i.HideHeart(playerLife);
            soundController.PlayerDead();
        }

        if (playerLife > 0)
        {
            tilemapUtil.ClearPlayerTrack();
            playerMovement.RespawnPlayer();
        }
        else
        {
            GameOver();
        }
    }

    private void OnEnemyDestroy(CharacterType characterType)
    {
        switch (characterType)
        {
            case CharacterType.EnemyWater:
                waterEnemiesCount--;
                break;
            case CharacterType.EnemyGround:
                groundEnemiesCount--;
                break;
        }

        if (waterEnemiesCount == 0)
        {
            LevelComplete();
        }
    }

    private void OnCalcLevelProgress(float progress)
    {
        LevelUI.SetLevelProgress(progress);

        if (progress >= 80)
        {
            LevelComplete();
        }
    }

}
