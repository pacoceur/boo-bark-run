using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Singleton

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #endregion

    public float currentScore = 0f;
    public float highscore = 0f;

    public bool isPlaying = false;

    public PlayfabManager playfabManager;

    public UnityEvent onPlay = new UnityEvent();
    public UnityEvent onGameOver = new UnityEvent();

    private void Start()
    {
        highscore = PlayerPrefs.GetFloat("Highscore", 0);
    }

    private void Update()
    {
        if (isPlaying)
        {
            currentScore += Time.deltaTime * 2;
        }
    }

    public void StartGame()
    {
        onPlay.Invoke();
        isPlaying = true;
        currentScore = 0;
    }

    public void GameOver()
    {
        onGameOver.Invoke();
        playfabManager.SendLeaderboard(currentScore);
        isPlaying = false;

        if (PlayerPrefs.GetFloat("Highscore", 0) < currentScore)
        {
            highscore = currentScore;
            PlayerPrefs.SetFloat("Highscore", currentScore);
        }
    }

    public string PrettyScore()
    {
        return "Score: " + Mathf.RoundToInt(currentScore).ToString() + "m";
    }

    public string PrettyHighscore()
    {
        return "Highscore: " + Mathf.RoundToInt(highscore).ToString() + "m";
    }
}
