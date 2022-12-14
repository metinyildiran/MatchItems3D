using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public class GameManager : TouchMove
{
    public static GameManager Instance { get; private set; }

    private bool isGameStarted = false;
    private bool isGameFinished = false;
    private bool isGameFailed = false;

    public int LastFinishedLevel { get; private set; }
    private int _score;

    public event Action OnGameStart;
    public event Action OnGameFailed;
    public event Action OnGameFinished;
    public event Action<int> OnScoreChanged;

    protected override void Awake()
    {
        base.Awake();

        Instance = this;

        LoadData();

        Application.targetFrameRate = 60;
    }

    protected override void Start()
    {
        base.Start();

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            LevelManager.Instance.LoadLastRemainingLevel();
        }

        Application.targetFrameRate = 60;
        Screen.SetResolution(720, 1280, false);
    }

    protected override void OnTouchMoved(CallbackContext context)
    {
        if (isGameStarted) return;

        isGameStarted = true;

        OnGameStart?.Invoke();
    }

    public void SetGameFailed()
    {
        isGameFailed = true;

        OnGameFailed?.Invoke();
    }

    public void GameFinished()
    {
        isGameFinished = true;

        SaveData();

        OnGameFinished?.Invoke();
    }

    public void AddScore(int amount = 10)
    {
        if (isGameFailed) return;

        OnScoreChanged?.Invoke(_score);
    }

    public bool CanPlayGame()
    {
        return isGameFailed || isGameFinished;
    }

    #region Getters and Setters

    public bool GetIsGameFailed()
    {
        return isGameFailed;
    }

    public bool GetIsGameFinished()
    {
        return isGameFinished;
    }

    #endregion

    #region Saving and Loading

    [Serializable]
    private class Data
    {
        public int lastFinishedLevel;
    }

    private void SaveData()
    {
        Data data = new Data
        {
            lastFinishedLevel = SceneManager.GetActiveScene().buildIndex
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    private void LoadData()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Data data = JsonUtility.FromJson<Data>(json);

            LastFinishedLevel = data.lastFinishedLevel;
        }
        else
        {
            LastFinishedLevel = 0;
        }
    }

    public void ResetData()
    {
        Data data = new Data
        {
            lastFinishedLevel = 0
        };

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);

        LoadData();
    }
    #endregion
}
