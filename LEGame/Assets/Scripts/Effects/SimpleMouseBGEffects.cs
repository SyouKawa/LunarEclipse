using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMouseBGEffects : MonoBehaviour
{
    public float XLimit;
    public float YLimit;
    public int middleXAxis = 1280;
    //public int middleYAxis = 720;
    private Vector3 newPos = Vector3.zero;

    void UpdateMovedPos()
    {
        newPos.z = transform.position.z;
        float tempX = Input.mousePosition.x - middleXAxis;
        newPos.x = (tempX / middleXAxis) * XLimit;

        /*float tempY = Input.mousePosition.y - middleYAxis;
        newPos.y = (tempY / middleYAxis) * YLimit;*/
        
    }

    void Update()
    {
        UpdateMovedPos();
        if (Mathf.Abs(newPos.x) < XLimit && Mathf.Abs(newPos.y) < YLimit)
        {
            transform.position = newPos;
            //transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime);
        }
    }
}
