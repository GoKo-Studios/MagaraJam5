using UnityEngine;
using UnityEngine.Audio;
using Assets.Scripts.Managers;

public class StartMenu : MonoBehaviour
{
    public GameObject startCanvas;
    public GameObject optionsCanvas;

    void Update(){
        
    }

    public void SetVolume(float volume){
    }

    public void StartFirstLevel(){
        EventManager.Instance.OnNextLevel?.Invoke();
    }

    public void GoToOptions(){
        startCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void BackToMenu(){
        startCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
    }
}
