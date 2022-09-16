using UnityEngine;
using System.Collections.Generic;

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

        #region Self Variables

        #region Serialized Variables

        [Header("Holder")] [SerializeField] private GameObject _levelHolder;

        [Space] [SerializeField] private int _totalLevelCount;
        [SerializeField] private int LevelID = 0;

        #endregion

        #endregion

        private void Start()
        {
            EventManager.Instance.OnLoadLevel += OnLoadLevel;
            EventManager.Instance.OnClearLevel += OnClearLevel;
            EventManager.Instance.OnNextLevel += OnNextLevel;
            EventManager.Instance.OnRestartLevel += OnRestartLevel;

            _levelHolder = GameObject.Find("LevelHolder");
            EventManager.Instance.OnLoadLevel?.Invoke(LevelID);
        }

        private void OnDisable()
        {
            EventManager.Instance.OnLoadLevel -= OnLoadLevel;
            EventManager.Instance.OnClearLevel -= OnClearLevel;
            EventManager.Instance.OnNextLevel -= OnNextLevel;
            EventManager.Instance.OnRestartLevel -= OnRestartLevel;

        }

        private void OnNextLevel()
        {
            LevelID++;
             EventManager.Instance.OnClearLevel?.Invoke();
            // DOVirtual.DelayedCall(.1f, () => EventManager.Instance.onLevelInitialize?.Invoke(GetLevelID()));
            // EventManager.Instance.onSaveGameData?.Invoke(new GameSaveDataParams()
            // {
            //     Level = LevelID
            // });
        }

        private void OnRestartLevel()
        {
            EventManager.Instance.OnClearLevel?.Invoke();
            //DOVirtual.DelayedCall(.1f, () => EventManager.Instance.onLevelInitialize?.Invoke(GetLevelID()));
            //EventManager.Instance.onSaveGameData?.Invoke(new GameSaveDataParams()
            // {
            //     Level = LevelID
            // });
        }

        private void OnLoadLevel(int levelID)
        {
            var newLevelObject = ResourceLoader.LoadResource<GameObject>(($"Levels/Level{levelID}"));
            var newLevel = Instantiate(newLevelObject, Vector3.zero, Quaternion.identity);
            if (newLevel != null) newLevel.transform.SetParent(_levelHolder.transform);
        }

        private void OnClearLevel()
        {
            Destroy(_levelHolder.transform.GetChild(0).gameObject);
        }
    }
}
