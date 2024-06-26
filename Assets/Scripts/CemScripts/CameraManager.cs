using UnityEngine;
using System.Collections; 
using Cinemachine;

public class CameraManager : MonoBehaviour
{

    #region //singleton pattern

    public static CameraManager Instance;

    private void Awake(){
        if(Instance != this && Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    private CinemachineVirtualCamera playerFollowCam;
    private CinemachineImpulseSource impulseSource;
    private float orthographicSize;
    private float previousOrthoSize;
    private float orthoLerpPercentage;

    private void Start(){
        playerFollowCam = GameObject.Find("PlayerFollowCam").GetComponent<CinemachineVirtualCamera>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        orthographicSize = playerFollowCam.m_Lens.OrthographicSize;
        previousOrthoSize = orthographicSize;
    }

    private void Update(){
        if(playerFollowCam.m_Lens.OrthographicSize != orthographicSize){
            SmoothAdjustOrthoSize(previousOrthoSize, orthographicSize, 1.5f);
        }
        else{
            orthoLerpPercentage = 0.0f;
        }
        orthoLerpPercentage = Mathf.Clamp(orthoLerpPercentage, 0.0f, 1.0f);
    }

    private void SmoothAdjustOrthoSize(float startPoint, float target, float speed){
        playerFollowCam.m_Lens.OrthographicSize = Mathf.Lerp(startPoint, target, orthoLerpPercentage);
        orthoLerpPercentage += Time.deltaTime * speed;
    }

    public void setOrthoSize(float size){
        previousOrthoSize = playerFollowCam.m_Lens.OrthographicSize;
        orthographicSize = size;
    }

    public void ScreenShake() {
        impulseSource.GenerateImpulseAt(playerFollowCam.transform.position, new Vector3(0.0f, 0.2f, 0.0f));
    }

}


