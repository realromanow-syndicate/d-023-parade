using UnityEngine;
using UnityEngine.UI;

public class ShoMananger : MonoBehaviour
{
    public Text money;
    public DataShop[] shops;
    public Sprite select, buy;

    public Image newDesk;
    public void Buy(int id)
    {
        if(!PlayerPrefs.HasKey("shop-"+id.ToString()))
        {
            if (PlayerPrefs.GetInt("money") < shops[id].count)
            {
                MenuManagers.instance.openGame(4);
            }

            if (PlayerPrefs.GetInt("money") >= shops[id].count)
            {
                MenuManagers.instance.openGame(5);
                PlayerPrefs.SetInt("money", PlayerPrefs.GetInt("money") - shops[id].count);

                for (int i = 0; i < 10; i++)
                {
                    if (PlayerPrefs.GetString("shop-" + i.ToString()) == "select")
                        PlayerPrefs.SetString("shop-" + i.ToString(), "selected");
                }
                PlayerPrefs.SetString("shop-" + id.ToString(), "select");
                newDesk.sprite = shops[id].cost.transform.parent.GetComponent<Image>().sprite;
              
                    }
        }
        else
        {
            if(PlayerPrefs.GetString("shop-"+id.ToString())=="selected")
            {
                for (int i = 0; i < 10; i++)
                {
                    if (PlayerPrefs.GetString("shop-" + i.ToString()) == "select")
                        PlayerPrefs.SetString("shop-" + i.ToString(), "selected");
                }
                PlayerPrefs.SetString("shop-" + id.ToString(), "select");
            }
        }
        SetData();
    }
    private void SetData()
    {
        for (int i = 0; i < shops.Length; i++)
        {
            if(PlayerPrefs.HasKey("shop-"+i.ToString()))
            {
                shops[i].cost.SetActive(false);
                shops[i].button.GetComponent<Image>().sprite = select;
                if (PlayerPrefs.GetString("shop-"+i.ToString())=="select")
                {
                    shops[i].button.transform.GetChild(0).GetComponent<Text>().text = "select";
                }
                if (PlayerPrefs.GetString("shop-" + i.ToString()) == "selected")
                {
                    shops[i].button.transform.GetChild(0).GetComponent<Text>().text = "selected";
                }
            }
            else
            {
                shops[i].cost.SetActive(true);
                shops[i].cost.transform.GetChild(0).GetComponent<Text>().text= shops[i].count.ToString();
                shops[i].button.transform.GetChild(0).GetComponent<Text>().text = "Buy";
                shops[i].button.GetComponent<Image>().sprite = buy;
            }
        }
    }
    public void Start()
    {
       
        if (!PlayerPrefs.HasKey("shop-0"))
            PlayerPrefs.SetString("shop-0", "select");
        SetData();
    }
    private void Update()
    {
        money.text = PlayerPrefs.GetInt("money").ToString();
    }

    [System.Serializable]
    public struct DataShop
    {
        public GameObject cost;
        public int count;

        public GameObject button;
    }
}
