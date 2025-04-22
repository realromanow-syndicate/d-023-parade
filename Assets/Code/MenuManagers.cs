using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManagers : MonoBehaviour
{
    public CanvasGroup[] panel;
    public Slider music, sound;

    public static MenuManagers instance;
    public CanvasGroup gamePanel;

    public Text money;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        money.text = PlayerPrefs.GetInt("money").ToString();
    }
    public void LoadScene()
    {
        gamePanel.gameObject.SetActive(true);
        gamePanel.alpha = 0f;

        gamePanel.DOFade(1f, 0.5f).OnComplete(() =>
        {
            SceneManager.LoadScene("Game");
        });
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
        });
    }

    public void SaveMusic()
    {
        PlayerPrefs.SetFloat("music",music.value);
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

        panel[id].transform.DOScale(Vector3.one, 0.5f);
        panel[id].DOFade(1f, 0.5f);
    }
    public void CloseGame(int id)
    {
        panel[id].transform.DOScale(Vector3.one*3f, 0.5f);
        panel[id].DOFade(0f, 0.5f).OnComplete(() =>
        {
            panel[id].gameObject.SetActive(false);
        });
    }
}
