
using UnityEngine;
using Assets.Scripts.Managers;

public class StageManager : MonoBehaviour
{
    #region Singleton

        public static StageManager Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

        _levelExit.SetActive(false);
        _levelExitIndicators.SetActive(false);
        }

    #endregion

    [SerializeField] private GameObject _levelExit;
    [SerializeField] private GameObject _levelExitIndicators;

    private void Start() {
        EventManager.Instance.OnWaveFinish += OnWaveFinish;
        EventManager.Instance.OnGameStart += OnGameStart;

    }

    private void OnDisable() {
        EventManager.Instance.OnWaveFinish -= OnWaveFinish;
        EventManager.Instance.OnGameStart -= OnGameStart;
    }

    private void OnWaveFinish() {
        _levelExit.SetActive(true);
        _levelExitIndicators.SetActive(true);
    }

    private void OnGameStart() {
        
    }
}
