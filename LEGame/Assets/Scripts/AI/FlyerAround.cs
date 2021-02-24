using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class FlyerAround : Monster
{
    public GameObject Pivot;
    public GameObject Flyer;

    public float perAngle;

    public float radius;

    void Start()
    {
        Vector3 initPos = Flyer.transform.position;
        initPos.x = -radius;
        Flyer.transform.position = initPos;
    }

    void DestroySelf()
    {
        Debug.Log("I'm dead already.");
    }

    void Update()
    {
        BaseUpdate();
        if(HP <= 0)
        {
            //停止环绕转动
            perAngle = 0;
            DestroySelf();
        }
        Flyer.transform.position = MMMaths.RotatePointAroundPivot(Flyer.transform.position,Pivot.transform.position,perAngle);
    }
}
