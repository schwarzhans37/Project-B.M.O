using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    public GameObject playerUI;
    public GameObject interactiveUI;
    public GameObject deathedUI;
    public GameObject deathCam;

    public void SetPlayerUIActive(bool active)
    {
        playerUI.SetActive(active);
    }
    
    public void SetInteractiveUIActive(bool active)
    {
        interactiveUI.SetActive(active);
    }

    public IEnumerator SetDeathedUIActive(bool active)
    {
        deathedUI.SetActive(active);
        deathedUI.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        deathedUI.transform.GetChild(1).GetComponent<CanvasGroup>().alpha = 0;

        if (active)
        {
            while (deathedUI.transform.GetChild(0).GetComponent<CanvasGroup>().alpha < 1)
            {
                deathedUI.transform.GetChild(0).GetComponent<CanvasGroup>().alpha += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(1f);
            deathedUI.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
            deathedUI.transform.GetChild(1).GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    public void SetDeathCamActive(bool active, Transform target = null)
    {
        deathCam.GetComponent<DeathCamera>().targetPlayer = target;
        deathCam.SetActive(active);
    }
}
