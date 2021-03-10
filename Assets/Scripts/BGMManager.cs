using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour {

    static public BGMManager instance;

    public AudioClip[] clips;//배경음악들

    private AudioSource source;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f); //반복문 안에서 계속 new 실행하는 것보다 이게 성능 좋음

    //Start보다 먼저 실행됨
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

    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
	}

    public void Play(int _playMusicTrack)
    {
        source.volume = 1;
        source.clip = clips[_playMusicTrack];
        source.Play();
    }

    public void SetVolumn(float _volumn)
    {
        source.volume = _volumn;
    }

    public void Pause()
    {
        source.Pause();
    }

    public void Unpause()
    {
        source.UnPause();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void FadeOutMusic()
    {
        StopAllCoroutines();//fadeinout 겹치지 않게 미리 앞에꺼 다 종료
        StartCoroutine(FadeOutMusicCoroutine());
    }

    IEnumerator FadeOutMusicCoroutine()
    {
        for (float i = 1.0f; i >= 0f; i -= 0.01f) 
        {
            source.volume = i;
            yield return waitTime;
        }
    }

    public void FadeInMusic()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInMusicCoroutine());
    }

    IEnumerator FadeInMusicCoroutine()
    {
        for (float i = 1.0f; i <= 1f; i += 0.01f)
        {
            source.volume = i;
            yield return waitTime;
        }
    }
}
