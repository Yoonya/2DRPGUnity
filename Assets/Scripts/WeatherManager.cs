using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour {

    static public WeatherManager instance;

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

    private AudioManager theAudio;
    public ParticleSystem rain;
    public string rain_sound;

    // Use this for initialization
    void Start () {
        theAudio = FindObjectOfType<AudioManager>();
	}

    public void Rain()
    {
        theAudio.Play(rain_sound);
        rain.Play();
    }

    public void RainStop()
    {
        theAudio.Stop(rain_sound);
        rain.Stop();
    }

    public void RainDrop()
    {
        rain.Emit(10);
    }
}
