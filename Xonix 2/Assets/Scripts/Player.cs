using UnityEngine;

public class Player : MonoBehaviour
{
    public GameController gameController;
    public SoundController soundController;
    public TilemapUtil tilemapUtil;
    public PlayerMovement playerMovement;

    private byte playerLife;

    
    void Awake()
    {
        playerLife = 3;
        PlayerMovement.OnPlayerDamaged += DecrementPlayerLife;
    }

    private void OnDestroy()
    {
        PlayerMovement.OnPlayerDamaged -= DecrementPlayerLife;
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
            gameController.GameOver();
        }
    }
}
