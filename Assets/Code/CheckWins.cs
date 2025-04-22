using DG.Tweening;
using UnityEngine;

public class CheckWins : MonoBehaviour
{
    public GameObject wins,skin;
    public void OnTransformChildrenChanged()
    {
        if(transform.childCount<=0)
        {
            wins.SetActive(true);
            skin.SetActive(true);
            GameManager.instance.StopAllCoroutines();
            DOVirtual.DelayedCall(1f, () =>
            {
                GameManager.instance.WinGame();
            });
        }
    }
}
