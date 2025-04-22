using UnityEngine;
using UnityEngine.UI;

public class lvlController : MonoBehaviour
{
    public Sprite select, close, open;

    public int id;

    private void Start()
    {
        id = transform.GetSiblingIndex();

        if(id<PlayerPrefs.GetInt("level"))
        {
            GetComponent<Image>().sprite = open;
        }
        if (id > PlayerPrefs.GetInt("level"))
        {
            GetComponent<Image>().sprite = close;
            GetComponent<Button>().interactable = false;
        }
        if (id==PlayerPrefs.GetInt("level"))
        {
            GetComponent<Image>().sprite = select;
        }

        transform.GetChild(0).GetComponent<Text>().text="Level "+(id+1).ToString();
    }

    public void LoadScene()
    {
        PlayerPrefs.SetInt("select", id);
        MenuManagers.instance.LoadScene();
    }
}
