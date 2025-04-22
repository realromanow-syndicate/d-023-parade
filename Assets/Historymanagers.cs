using UnityEngine;
using UnityEngine.UI;

public class Historymanagers : MonoBehaviour
{
    public GameObject startHistory;
    public InputField name;
    public Button nae;


    private void Update()
    {
        nae.interactable = name.text.Length > 0;
    }
    public void Start()
    {
        if (PlayerPrefs.HasKey("tutorial"))
            startHistory.SetActive(false);
    }


    public void SetHistory()
    {
        PlayerPrefs.SetString("tutorial", "his");

    }
}
