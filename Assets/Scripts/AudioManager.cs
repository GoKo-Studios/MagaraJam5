using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Managers {
    public class AudioManager : MonoBehaviour
    {
        #region Singleton

        public static AudioManager Instance;

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

        [SerializeField] private AudioSource _musicSource, _effectSource;

        public void PlaySound(AudioClip clip) {
            _effectSource.PlayOneShot(clip);
        }

        public void ChangeMasterVolume(float value) {
            AudioListener.volume = value;
        }

        public void ToggleEffects() {
            _effectSource.mute = !_effectSource.mute;
        }

        public void ToggleMusic() {
            _musicSource.mute = !_musicSource.mute;
        }
    }
}

