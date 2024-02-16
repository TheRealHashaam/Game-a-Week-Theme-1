using Cinemachine;
using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    public Animator[] Chickens;
    public string[] States = { "Eat", "TurnHead" };
    Coroutine cor;
    public GameObject ChickenParents;
    public GameObject Menucamera, PlayerCamera;
    public CinemachineBrain brain;
    public CinemachineVirtualCamera ScoreCam;
    public Image FadePanel;
    public GameObject MainmenuPanel, ControlsPanel;
    public Bow bow;
    public StarterAssetsInputs inputs;
    public GameObject PausePanel;
    bool _mainmenu = true;
    bool _gameplay = false;
    public ThirdPersonController personController;
    public float countdownDuration = 5f;
    private float countdown;
    bool startcounter = false;
    public TextMeshProUGUI counter;
    public GameObject Gameplay;
    public GameObject ScorePanel;
    public TextMeshProUGUI GameplayScoreText;
    public TextMeshProUGUI Scorepaneltext, Highscorepaneltext;
    public int score;
    private int _highscore;
    public AudioSource ClickSound, ScoreSound;

    private void Awake()
    {
        if(PlayerPrefs.HasKey("FirstTime"))
        {
            PlayerPrefs.SetInt("FirstTime", 0);
        }
        else
        {
            PlayerPrefs.SetInt("FirstTime", 1);
        }
        if (PlayerPrefs.HasKey("HighScore"))
        {
            _highscore = PlayerPrefs.GetInt("HighScore");
        }
        else
        {
            _highscore = 0;
        }
    }

    private void Start()
    {
        OpenMainmenu();
    }

    public void Pause()
    {
        bow.CanAim = false;
        Time.timeScale = 0f;
        personController.enabled = false;
        PausePanel.SetActive(true);
        Gameplay.SetActive(false);
        inputs.SetCursorState(false);
    }

    public void Resume()
    {
        if(bow.firingchicken)
        {
            bow.CanAim = false;
        }
        else
        {
            bow.CanAim = true;
        }    
        personController.enabled = true;
        Time.timeScale = 1f;
        PausePanel.SetActive(false);
        inputs.SetCursorState(true);
        Gameplay.SetActive(true);
        ClickSound.Play();
    }

    public void Quit()
    {
        ClickSound.Play();
        Time.timeScale = 1f;
        FadePanel.DOFade(1, 0.5f).OnComplete(()=>
        {
            Application.Quit();
        });
    }

    void OpenMainmenu()
    {
        _gameplay = false;
        brain.m_DefaultBlend.m_Time = 1f;
        FadePanel.DOFade(0, 0.5f);
        cor = StartCoroutine(Animate());
    }

    public void StartGame()
    {
        StartCoroutine(GamePlay_Delay());
        _gameplay = true;
        ClickSound.Play();
    }
    IEnumerator GamePlay_Delay()
    {
        _mainmenu = false;
        inputs.SetCursorState(true);
        MainmenuPanel.SetActive(false);
        PlayerCamera.SetActive(true);
        Menucamera.SetActive(false);
        yield return new WaitForSeconds(1f);
        brain.m_DefaultBlend.m_Time = 0.2f;
        bow.CanAim = true;
        StopCoroutine(cor);
        ChickenParents.SetActive(false);
        countdown = countdownDuration;
        ScorePanel.SetActive(false);
        Gameplay.SetActive(true);
        UpdateScore();
        GameplayScoreText.text = score.ToString();
        startcounter = true;
    }

    public void OpenControlsPanel()
    {
        if(!_gameplay)
        {
            MainmenuPanel.SetActive(false);
        }
        else
        {
            PausePanel.SetActive(false);
        }
        ControlsPanel.SetActive(true);
        ClickSound.Play();

    }
    public void CloseControlsPanel()
    {
        if (!_gameplay)
        {
            MainmenuPanel.SetActive(true);
        }
        else
        {
            PausePanel.SetActive(true);
        }
        ControlsPanel.SetActive(false);
        ClickSound.Play();
    }

    IEnumerator Animate()
    {
        int r = Random.Range(0, 3);
        for (int i = 0; i <= r; i++)
        {
            int index = Random.Range(0, 2);
            Chickens[i].SetTrigger(States[index]);
            yield return new WaitForSeconds(0.2f);
        }

        int delay = Random.Range(2, 5);
        yield return new WaitForSeconds(delay);
        StartCoroutine(Animate());
    }

    public void ResetScore()
    {
        score = 0;
        countdown = countdownDuration;
        UpdateScore();

    }

    public void RestartGame()
    {
        personController.enabled = true;
        _mainmenu = false;
        inputs.SetCursorState(true);
        PlayerCamera.SetActive(true);
        bow.CanAim = true;
        ResetScore();
        ScorePanel.SetActive(false);
        Gameplay.SetActive(true);
        GameplayScoreText.text = score.ToString();
        startcounter = true;
        ClickSound.Play();
    }
    public void UpdateScore()
    {
        GameplayScoreText.text = score.ToString();
    }
    void OpenScorePanel()
    {
        inputs.SetCursorState(false);
        _mainmenu = true;
        startcounter = false;
        Scorepaneltext.text = score.ToString();
        if(score >_highscore)
        {
            _highscore = score;
            Highscorepaneltext.text = _highscore.ToString();
            PlayerPrefs.SetInt("HighScore", _highscore);
        }
        else
        {
            Highscorepaneltext.text = _highscore.ToString();
        }
        ScorePanel.SetActive(true);
        Gameplay.SetActive(false);
        bow.CanAim = false;
        personController.enabled = false;
    }



    private void Update()
    {
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape)&& !_mainmenu)
        {
            Pause();
        }
        if(startcounter)
        {
            countdown -= Time.deltaTime;
            counter.text = Mathf.Max(0, countdown).ToString("F0") + " s";
            if (countdown <= 0f)
            {
                OpenScorePanel();
            }
        }
    }

}
