using UnityEngine;
using UnityEngine.UI;

public class BookBaloon : MonoBehaviour
{
    public Image[] img;
    public Sprite[] skin;


    private void Start()
    {
        for (int i = 0; i < PlayerPrefs.GetInt("level"); i++)
        {
            img[i].sprite = skin[i];
        }
    }
}
