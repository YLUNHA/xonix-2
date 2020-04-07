using UnityEngine;

public class LevelHelper : MonoBehaviour
{
    public GameController gameController;

    private float levelTimer;
    private int waterEnemiesCount;
    private int groundEnemiesCount;

    void Awake()
    {
        waterEnemiesCount = 0;
        groundEnemiesCount = 0;
        levelTimer = 60;

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

        TilemapUtil.OnCalcLevelProgress += OnCalcLevelProgress;
        TilemapUtil.OnEnemyDestroy += OnEnemyDestroy;
    }

    private void OnDestroy()
    {
        TilemapUtil.OnCalcLevelProgress -= OnCalcLevelProgress;
        TilemapUtil.OnEnemyDestroy -= OnEnemyDestroy;
    }

    void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        levelTimer -= Time.deltaTime;

        if (levelTimer <= 0)
        {
            gameController.GameOver();
        }

        LevelUI.SetTimer(levelTimer);
    }

    private void OnCalcLevelProgress(float progress)
    {
        LevelUI.SetLevelProgress(progress);

        if (progress >= 80)
        {
            gameController.LevelComplete();
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
            gameController.LevelComplete();
        }
    }
}
