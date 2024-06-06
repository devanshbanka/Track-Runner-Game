using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool isAlive = true;

    public static GameManager MyInstance;

    private PlayerController playercontroller;

    private GroundSpawner groundspawner;


    public int Score;
    public float timeLeft;
    private int highscoreEasy;
    private int highscoreMedium;
    private int highscoreHard;
    public int DifficultyMode;

    public GameObject ScoreObjLandscape;
    public TextMeshProUGUI ScoreTextLandscape;
    public GameObject StartTimerObjLandscape;
    public TextMeshProUGUI StartTimerTextLandscape;
    public GameObject PauseButtonLandscape;
    public GameObject PausedPanelLandscape;
    public GameObject GameoverPanelLandscape;
    public GameObject StartGamePanelLandscape;
    public TextMeshProUGUI GameOverScoreLandscape;
    public TextMeshProUGUI BestScoreLandscape;
    public GameObject DifficultyPanelLandscape;
    public TextMeshProUGUI GameOverPanelDifficultyLandscape;

    public GameObject ScoreObjPortrait;
    public TextMeshProUGUI ScoreTextPortrait;
    public GameObject StartTimerObjPortrait;
    public TextMeshProUGUI StartTimerTextPortrait;
    public GameObject PauseButtonPortrait;
    public GameObject PausedPanelPortrait;
    public GameObject GameoverPanelPortrait;
    public GameObject StartGamePanelPortrait;
    public TextMeshProUGUI GameOverScorePortrait;
    public TextMeshProUGUI BestScorePortrait;
    public GameObject DifficultyPanelPortrait;
    public TextMeshProUGUI GameOverPanelDifficultyPortrait;

    private GameObject ScoreObj;
    private TextMeshProUGUI ScoreText;
    private GameObject StartTimerObj;
    private TextMeshProUGUI StartTimerText;
    private GameObject PauseButton;
    private GameObject PausedPanel;
    private GameObject GameoverPanel;
    private GameObject StartGamePanel;
    private TextMeshProUGUI GameOverScore;
    private TextMeshProUGUI BestScore;
    private GameObject DifficultyPanel;
    private TextMeshProUGUI GameOverPanelDifficulty;



    private void Awake()
    {
        MyInstance = this;
        playercontroller = GameObject.FindObjectOfType<PlayerController>();
        groundspawner = GameObject.FindObjectOfType<GroundSpawner>();
    }

    // Start is called before the first frame update
    void Start()
    {
        highscoreEasy = PlayerPrefs.GetInt("highscoreEasy", highscoreEasy);
        highscoreMedium = PlayerPrefs.GetInt("highscoreMedium", highscoreMedium);
        highscoreHard = PlayerPrefs.GetInt("highscoreHard", highscoreHard);
        Time.timeScale = 0;
        InitializeUIReferences();
        StartGamePanel.SetActive(true);
        GameoverPanel.SetActive(false);
        StartTimerObj.SetActive(false);
        PauseButton.SetActive(false);
        PausedPanel.SetActive(false);
        ScoreObj.SetActive(false);
        DifficultyPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = Score.ToString();

        if (StartTimerObj.activeSelf)
        {
            //SoundManager.PlaySound("Countdown");
            timeLeft -= Time.unscaledDeltaTime;
            StartTimerText.text = (timeLeft).ToString("0");
            if (timeLeft < 1)
            {
                Time.timeScale = 1;
                StartTimerObj.SetActive(false);
                ScoreObj.SetActive(true);
                PauseButton.SetActive(true);
            }
        }
    }

    public IEnumerator Dead()
    {
        if(DifficultyMode == 0)
        {
            if (Score > highscoreEasy)
            {
                highscoreEasy = Score;
                GameOverScore.text = "" + Score;
                BestScore.text = "New High Score!";

                PlayerPrefs.SetInt("highscoreEasy", highscoreEasy);
            }
            else
            {
                GameOverScore.text = "" + Score;
                BestScore.text = "Best: " + highscoreEasy;
            }
        }

        else if (DifficultyMode == 1)
        {
            if (Score > highscoreMedium)
            {
                highscoreMedium = Score;
                GameOverScore.text = "" + Score;
                BestScore.text = "New High Score!";

                PlayerPrefs.SetInt("highscoreMedium", highscoreMedium);
            }
            else
            {
                GameOverScore.text = "" + Score;
                BestScore.text = "Best: " + highscoreMedium;
            }
        }

        else if (DifficultyMode == 2)
        {
            if (Score > highscoreHard)
            {
                highscoreHard = Score;
                GameOverScore.text = "" + Score;
                BestScore.text = "New High Score!";

                PlayerPrefs.SetInt("highscoreHard", highscoreHard);
            }
            else
            {
                GameOverScore.text = "" + Score;
                BestScore.text = "Best: " + highscoreHard;
            }
        }

        isAlive = false;

        float startTime = Time.unscaledTime;
        float waitTime = 2f;

        PauseButton.SetActive(false);

        while (Time.unscaledTime - startTime < waitTime)
        {
            yield return null;
        }
        Time.timeScale = 0;
        ScoreObj.SetActive(false);
        GameoverPanel.SetActive(true);
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void EasyMode()
    {
        DifficultyMode = 0;
        ModeSelected();
    }

    public void MediumMode()
    {
        DifficultyMode = 1;
        ModeSelected();
    }

    public void HardMode()
    {
        DifficultyMode = 2;
        ModeSelected();
    }

    public void ModeSelected()
    {
        switch (DifficultyMode)
        {
            case 0:
                playercontroller.RunSpeed = 15f;
                playercontroller.speedIncrease = 0.2f;
                playercontroller.maxSpeed = 35f;
                GameOverPanelDifficulty.text = "Easy";

                break;
            case 1:
                playercontroller.RunSpeed = 20f;
                playercontroller.speedIncrease = 0.25f;
                playercontroller.maxSpeed = 45f;
                GameOverPanelDifficulty.text = "Medium";
                break;
            case 2:
                playercontroller.RunSpeed = 25f;
                playercontroller.speedIncrease = 0.3f;
                playercontroller.maxSpeed = 40f;
                GameOverPanelDifficulty.text = "Hard";
                break;
        }
        DifficultyPanel.SetActive(false);
        StartTimerObj.SetActive(true);

        StartCoroutine(SpawnInitialTiles());
    }

    private IEnumerator SpawnInitialTiles()
    {
        for (int i = 0; i < 10; i++)
        {
            groundspawner.spawnTile();
            yield return new WaitForSeconds(0.1f);
        }
    }


    public void StartGame()
    {
        StartGamePanel.SetActive(false);
        DifficultyPanel.SetActive(true);
    }

    public void PauseGame()
    {
        PauseButton.SetActive(false);
        Time.timeScale = 0;
        PausedPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        PauseButton.SetActive(true);
        PausedPanel.SetActive(false);
        Time.timeScale = 1;
    }

    private void InitializeUIReferences()
    {
        if (Screen.width > Screen.height)
        {
            SetLandscapeReferences();
        }
        else
        {
            SetPortraitReferences();
        }
    }

    public void SetLandscapeReferences()
    {
        ScoreObj = ScoreObjLandscape;
        ScoreText = ScoreTextLandscape;
        StartTimerObj = StartTimerObjLandscape;
        StartTimerText = StartTimerTextLandscape;
        PauseButton = PauseButtonLandscape;
        PausedPanel = PausedPanelLandscape;
        GameoverPanel = GameoverPanelLandscape;
        StartGamePanel = StartGamePanelLandscape;
        GameOverScore = GameOverScoreLandscape;
        BestScore = BestScoreLandscape;
        DifficultyPanel = DifficultyPanelLandscape;
        GameOverPanelDifficulty = GameOverPanelDifficultyLandscape;
    }

    public void SetPortraitReferences()
    {
        ScoreObj = ScoreObjPortrait;
        ScoreText = ScoreTextPortrait;
        StartTimerObj = StartTimerObjPortrait;
        StartTimerText = StartTimerTextPortrait;
        PauseButton = PauseButtonPortrait;
        PausedPanel = PausedPanelPortrait;
        GameoverPanel = GameoverPanelPortrait;
        StartGamePanel = StartGamePanelPortrait;
        GameOverScore = GameOverScorePortrait;
        BestScore = BestScorePortrait;
        DifficultyPanel = DifficultyPanelPortrait;
        GameOverPanelDifficulty = GameOverPanelDifficultyPortrait;
    }
}
