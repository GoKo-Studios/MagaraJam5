using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers {
    public class LevelManager : MonoBehaviour
    {
        #region Singleton

        public static LevelManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        #endregion

        private void Start()
        {
            EventManager.Instance.OnLoadLevel += OnLoadLevel;
            EventManager.Instance.OnNextLevel += OnNextLevel;
            EventManager.Instance.OnRestartLevel += OnRestartLevel;
        }

        private void OnDisable()
        {
            EventManager.Instance.OnLoadLevel -= OnLoadLevel;
            EventManager.Instance.OnNextLevel -= OnNextLevel;
            EventManager.Instance.OnRestartLevel -= OnRestartLevel;
        }

        private void OnNextLevel()
        {
            int index = SceneManager.GetActiveScene().buildIndex + 1;
            EventManager.Instance.OnLoadLevel?.Invoke(index);
        }

        private void OnRestartLevel()
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            EventManager.Instance.OnLoadLevel?.Invoke(index);
        }

        private void OnLoadLevel(int levelID)
        { 
            SceneManager.LoadScene(levelID);
        }
    }
}
