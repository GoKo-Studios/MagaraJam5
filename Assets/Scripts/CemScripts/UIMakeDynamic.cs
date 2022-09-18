using UnityEngine;
using UnityEngine.UI;

public class UIMakeDynamic : MonoBehaviour
{
    private Slider[] uiTransforms = new Slider[4];

    void Start(){
        for(int i = 0; i < 2; i++){
            Transform targetTransform;
            targetTransform = transform.GetChild(i).GetChild(0);
            uiTransforms[i] = targetTransform.GetComponent<Slider>();
        }
    }

    void Update(){
        uiTransforms[0].value = theArrowMovement.MapRange(0.0f, 100.0f, 0.0f, 1.0f, UIDynamicVariables.Instance.playerMana);
        uiTransforms[1].value = theArrowMovement.MapRange(0.0f, 100.0f, 0.0f, 1.0f, UIDynamicVariables.Instance.playerHPSmooth);
    }
}
