using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers {
    public class GameManager : MonoBehaviour
    {
        #region Singleton

        public static GameManager Instance;

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
    }
}


