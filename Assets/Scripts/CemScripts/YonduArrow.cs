using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YonduArrow : MonoBehaviour
{

    private void Update(){
        //Debug.Log(Input.mousePosition);
    }

    public Vector3 mosueWorldPosition(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit)){
            return hit.point;
        }else{
            return Vector3.zero;
        }
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mosueWorldPosition() + new Vector3(0.0f, 1.84f, 0.0f), 1);
    }

    public static float MapRange(float input_start, float input_end, float output_start, float output_end, float value ){
        return output_start + ((output_end - output_start) / (input_end - input_start)) * (value - input_start);
    }

    public static float MapRange(float input_start, float input_end, float output_start, float slope, float value, bool optimized ){
        return output_start + slope * (value - input_start);
    }
}
