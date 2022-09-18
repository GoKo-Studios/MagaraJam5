using UnityEngine;
using Assets.Scripts.Managers;

public class SceneHandler : MonoBehaviour
{
    public void StarButton(){
        EventManager.Instance.OnNextLevel?.Invoke();
    }
}
