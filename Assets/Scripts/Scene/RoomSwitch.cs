using System.Collections;
using Cinemachine;
using UnityEngine;

public class RoomSwitch : MonoBehaviour
{
    CinemachineVirtualCamera vc;
    static bool firstTrigger = true;
    void Start()
    {
        vc = GetComponentInChildren<CinemachineVirtualCamera>();
        vc.gameObject.transform.position = transform.position + new Vector3(0, 0, -11);
        vc.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!firstTrigger)
        {
            StartCoroutine(Freeze());
        }
        else
        {
            firstTrigger = false;
        }
        Debug.Log(vc.name);
        vc.gameObject.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        vc.gameObject.SetActive(false);
        vc.gameObject.transform.position = transform.position + new Vector3(0, 0, -11);
    }

    IEnumerator Freeze()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.4f);
        Time.timeScale = 1;
    }
}
