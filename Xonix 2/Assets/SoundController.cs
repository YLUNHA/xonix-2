using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource backgroundAudio;
    public AudioSource playerDeadAudio;
    public AudioSource cutWaterAudio;
    public AudioSource gameOverAudio;
    public AudioSource levelCompleteAudio;

    public void CutWater()
    {
        cutWaterAudio.Play();
    }

    public void PlayerDead()
    {
        playerDeadAudio.Play();
    }

    public void GameOver()
    {
        BackgroundMusic(false);
        gameOverAudio.Play();
    }

    public void LevelComplete()
    {
        BackgroundMusic(false);
        levelCompleteAudio.Play();
    }

    private void BackgroundMusic(bool enable)
    {
        if (enable)
        {
            backgroundAudio.Play();
        }
        else
        {
            backgroundAudio.Stop();
        }
    }
}
