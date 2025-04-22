using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public CanvasGroup[] panel;
    public Slider music, sound;
    public CanvasGroup gamePanel;

    public Text money;
    public AudioClip lClip, wClip;

    public Text level;
    public Text winCountCoin;

    public Text timerShow;
    public int[] second;
    public static GameManager instance;

    public SpriteRenderer desk;
    public Sprite[] skin;

    public GameObject[] lvl;
    private void Awake()
    {
        instance = this;

        for (int i = 0; i < skin.Length; i++)
        {
            if(PlayerPrefs.GetString("shop-"+i.ToString())=="select")
            {
                desk.sprite = skin[i];
                break;
            }
        }
        lvl[PlayerPrefs.GetInt("select")].SetActive(true);
    }
    private void Update()
    {
        money.text=PlayerPrefs.GetInt("money").ToString();
        level.text = "Level-" + (PlayerPrefs.GetInt("select") + 1).ToString();

        timerShow.text = (second[PlayerPrefs.GetInt("select")] / 60).ToString("00") + ":" + (second[PlayerPrefs.GetInt("select")] % 60).ToString("00");
    }

    public void OpenAndCloseRule(int id)
    {
        if(id==0)
        {
            Time.timeScale = 0f;
            openGame(0);
        }
        if (id == 1)
        {
            Time.timeScale = 1f;
            CloseGame(0);
        }
    }
    public void OpenAndClosePause(int id)
    {
        if (id == 0)
        {
            Time.timeScale = 0f;
            openGame(1);
        }
        if (id == 1)
        {
            Time.timeScale = 1f;
            CloseGame(1);
        }
    }
    public void WinGame()
    {
        if (panel[2].gameObject.activeSelf==false)
        {
            Time.timeScale=0f;
            openGame(2);
            transform.GetChild(1).GetComponent<AudioSource>().PlayOneShot(wClip);

            if(PlayerPrefs.GetInt("select")+1>
                PlayerPrefs.GetInt("level"))
            {
                PlayerPrefs.SetInt("level",
                    PlayerPrefs.GetInt("level") + 1);
            }
            winCountCoin.text=((PlayerPrefs.GetInt("select")+1)*25).ToString();
            PlayerPrefs.SetInt("money",
                PlayerPrefs.GetInt("money")+ (PlayerPrefs.GetInt("select") + 1) * 25);
            StopAllCoroutines();
        }
    }
    public void LoseGame()
    {
        if (panel[3].gameObject.activeSelf == false)
        {
            Time.timeScale = 0f;
            openGame(3);
            transform.GetChild(1).GetComponent<AudioSource>().PlayOneShot(lClip);


        }
    }
    public void Next()
    {
        PlayerPrefs.SetInt("select", PlayerPrefs.GetInt("select") + 1);
        LoadScene("Game");
    }
    public void LoadScene(string name)
    {
        gamePanel.gameObject.SetActive(true);
        gamePanel.alpha = 0f;

        gamePanel.DOFade(1f, 0.5f).OnComplete(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(name);
        }).SetUpdate(true);
    }
    public void SaveMusic()
    {
        PlayerPrefs.SetFloat("music", music.value);
    }
    public void SaveSound()
    {
        PlayerPrefs.SetFloat("sound", sound.value);
    }
    public void openGame(int id)
    {
        panel[id].gameObject.SetActive(true);
        panel[id].alpha = 0f;
        panel[id].transform.localScale = Vector3.one * 3f;

        panel[id].transform.DOScale(Vector3.one, 0.5f).SetUpdate(true);
        panel[id].DOFade(1f, 0.5f).SetUpdate(true);
    }
    public void CloseGame(int id)
    {
        panel[id].transform.DOScale(Vector3.one * 3f, 0.5f);
        panel[id].DOFade(0f, 0.5f).OnComplete(() =>
        {
            panel[id].gameObject.SetActive(false);
        }).SetUpdate(true);
    }
    private void Start()
    {
        music.value = PlayerPrefs.HasKey("music") ? PlayerPrefs.GetFloat("music") : 1f;
        sound.value = PlayerPrefs.HasKey("sound") ? PlayerPrefs.GetFloat("sound") : 1f;

        gamePanel.gameObject.SetActive(true);
        gamePanel.alpha = 1f;

        gamePanel.DOFade(0f, 0.5f).OnComplete(() =>
        {
            gamePanel.gameObject.SetActive(false);
            StartCoroutine("Timer");
        });

        
    }

    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            second[PlayerPrefs.GetInt("select")]--;
            if (second[PlayerPrefs.GetInt("select")]<=0)
            {
                LoseGame();
                break;

            }
        }
    }
}
