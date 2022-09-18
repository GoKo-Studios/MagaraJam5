using UnityEngine.Audio;
using System;
using UnityEngine;


public class AudioManagerC : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManagerC Instance;

    void Awake(){

        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        setSoundsArray();
    }

    void setSoundsArray(){
        foreach(Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }
    }

    void Start(){
        Play("CityAmbiance");
    }

    public void Play(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null){
            return;
        }
        s.source.Play();
    }

    public void SetVolume(float volume){
        foreach(Sound s in sounds){
            s.source.volume = volume;
        }
    }
}
