using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers {
    public class GUIManager : MonoBehaviour
    {
        #region Singleton

        public static GUIManager Instance;

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

        private void OnEnable() {
            EventManager.Instance.OnUpdateTimer += UpdateWaveTimer;
        }

        private void OnDisable() {
            EventManager.Instance.OnUpdateTimer -= UpdateWaveTimer;
        }

        public void Button_SpawnAggressiveMob() {
            MobSpawnerManager.Instance.SpawnObjectWithPooling(ResourceLoader.LoadResource<ScriptableObject>("Objects/PoolableObjects/New Spider") as MobDataBase);
        }

        public void Button_SpawnPassiveMob() {
            MobSpawnerManager.Instance.SpawnObjectWithPooling(ResourceLoader.LoadResource<ScriptableObject>("Objects/PoolableObjects/NewPassiveMob") as MobDataBase);
        }

        private void UpdateWaveTimer(int time) {
            
        }
    }
}

