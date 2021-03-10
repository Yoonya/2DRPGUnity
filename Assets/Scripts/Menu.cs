using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public static Menu instance;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);//다른 신으로 로드할 때마다 이 객체를 파괴하지 말아라
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public GameObject go;
    public AudioManager theAudio;

    public OrderManager theOrder;

    public GameObject[] gos;

    public string call_sound;
    public string cancel_sound;

    private bool activated;

    public void Exit()
    {
        Application.Quit();//게임종료
    }

    public void Continue()
    {
        theOrder.Move();
        activated = false;
        go.SetActive(false);
        theAudio.Play(cancel_sound);
    }

    public void GoToTitle()
    {
        for (int i = 0; i < gos.Length; i++)
        {
            Destroy(gos[i]);
        }
        go.SetActive(false);
        activated = false;
        SceneManager.LoadScene("title");
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            activated = !activated;

            if (activated)
            {
                theOrder.NotMove();
                go.SetActive(true);
                theAudio.Play(call_sound);
            }
            else
            {             
                go.SetActive(false);
                theAudio.Play(cancel_sound);
                theOrder.Move();//이렇게 두면 움직이지 못하는 상황일 때, esc를 통해서 움직일 수 있게된다. 후에 여러 조건문을 구현해야한다.
            }
        }
	}
}
