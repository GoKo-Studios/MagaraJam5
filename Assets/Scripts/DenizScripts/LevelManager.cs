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
        [SerializeField] private int _currentlevelIndex;
        [SerializeField] private int LevelID = 0;
        [SerializeField] private List<GameObject> _levelList;

        #endregion

        #endregion

        private void OnEnable()
        {
            EventManager.Instance.OnLoadLevel += OnLoadLevel;
            EventManager.Instance.OnClearLevel += OnClearLevel;
            EventManager.Instance.OnNextLevel += OnNextLevel;
            EventManager.Instance.OnRestartLevel += OnRestartLevel;

            EventManager.Instance.OnLoadLevel?.Invoke(LevelID);
        }

        private void OnDisable()
        {
            EventManager.Instance.OnLoadLevel -= OnLoadLevel;
            EventManager.Instance.OnClearLevel -= OnClearLevel;
            EventManager.Instance.OnNextLevel -= OnNextLevel;
            EventManager.Instance.OnRestartLevel -= OnRestartLevel;

        }

        private void PopulateList()
        {
            _levelList.AddRange(ResourceLoader.LoadResources<GameObject>("Levels"));
            _totalLevelCount = _levelList.Count;
        }


        private void OnNextLevel()
        {
            LevelID++;
            // EventManager.Instance.onClearActiveLevel?.Invoke();
            // DOVirtual.DelayedCall(.1f, () => EventManager.Instance.onLevelInitialize?.Invoke(GetLevelID()));
            // EventManager.Instance.onSaveGameData?.Invoke(new GameSaveDataParams()
            // {
            //     Level = LevelID
            // });
        }

        private void OnRestartLevel()
        {
            //EventManager.Instance.onClearActiveLevel?.Invoke();
            //DOVirtual.DelayedCall(.1f, () => EventManager.Instance.onLevelInitialize?.Invoke(GetLevelID()));
            //EventManager.Instance.onSaveGameData?.Invoke(new GameSaveDataParams()
            // {
            //     Level = LevelID
            // });
        }

        private void OnLoadLevel(int levelID)
        {
            var newLevelObject = Resources.Load<GameObject>($"Prefabs/LevelPrefabs/level {levelID}");
            var newLevel = Instantiate(newLevelObject, Vector3.zero, Quaternion.identity);
            if (newLevel != null) newLevel.transform.SetParent(_levelHolder.transform);
        }

        private void OnClearLevel()
        {
            Destroy(_levelHolder.transform.GetChild(0).gameObject);
        }
    }
}
