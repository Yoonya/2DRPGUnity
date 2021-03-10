using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


    private Bound[] bounds;
    private PlayerManager thePlayer;
    private CameraManager theCamera;
    private FadeManager theFade;
    private Menu theMenu;
    private DialogueManager theDM;
    private Camera cam;

    public GameObject HPBar;
    public GameObject MPBar;

    public void LoadStart()
    {
        StartCoroutine(LoadWaitCoroutine());
    }

    IEnumerator LoadWaitCoroutine()
    {
        yield return new WaitForSeconds(0.5f);//다른 객체들이 자리를 잡을 때까지 기다림

        thePlayer = FindObjectOfType<PlayerManager>();
        bounds = FindObjectsOfType<Bound>();
        theCamera = FindObjectOfType<CameraManager>();
        theFade = FindObjectOfType<FadeManager>();
        theMenu = FindObjectOfType<Menu>();
        theDM = FindObjectOfType<DialogueManager>();
        cam = FindObjectOfType<Camera>();

        Color color = thePlayer.GetComponent<SpriteRenderer>().color;
        color.a = 1f;
        thePlayer.GetComponent<SpriteRenderer>().color = color;

        theCamera.target = GameObject.Find("Player");//카메라의 타겟설정
        theMenu.GetComponent<Canvas>().worldCamera = cam;
        theDM.GetComponent<Canvas>().worldCamera = cam;

        for (int i = 0; i < bounds.Length; i++)
        {
            if (bounds[i].boundName == thePlayer.currentMapName)
            {
                bounds[i].SetBound();
                break;
            }
        }

        HPBar.SetActive(true);
        MPBar.SetActive(true);

        theFade.FadeIn();
    }
}
