using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using Cinemachine;

namespace Assets.Scripts.Managers {
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCam;

        [SerializeField] private bool _start;
        [SerializeField] private CinemachineImpulseSource _impulseSource;

        private void Update() {
            if (_start) {
                _start = !_start;
                ScreenShake();
                //StartCoroutine(ScreenShake(_screenShakeDuration));
            }
        }

        private void ScreenShake() {
            _impulseSource.GenerateImpulseAt(_virtualCam.transform.position, new Vector3(0f, -1f, 0f));
        }

        // private void FollowTarget() {
        //     _isoCameraPivot.position = _followTarget.position;
        // }

        // IEnumerator ScreenShake(float duration) {
        //     Vector3 startPosition = _cameraTransform.localPosition;
        //     float elapsedTime = 0f;

        //     while (elapsedTime < duration) {
        //         elapsedTime += Time.deltaTime;
        //         float strength = _screenShakeCurve.Evaluate(elapsedTime / duration);
        //         _cameraTransform.localPosition = startPosition + Random.insideUnitSphere * strength;
        //         yield return null;
        //     }

        //     _cameraTransform.localPosition = startPosition;
        // }
    }
}

