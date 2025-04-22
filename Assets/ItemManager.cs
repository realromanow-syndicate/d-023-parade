using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ItemManager : MonoBehaviour
{
    public bool isDrag = false;
    public Vector3 startPos;


    public bool end=false;
    private void OnMouseDown()
    {
        isDrag = true;
        startPos = transform.position;
    }

    private void OnMouseUp()
    {
        isDrag = false;
        if (!end)
        {
            transform.position = startPos;
        }
        
       
    }
    private void Update()
    {
        if(isDrag)
        {
            transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.name==gameObject.name)
        {
            end = true;
            if (!isDrag)
            {
                
                transform.position=collision.transform.position;
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                Destroy(gameObject);
            }
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == gameObject.name)
        {
            end = false;
            
        }
    }
}
