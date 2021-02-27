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
        body = transform.GetChild(0).GetComponent<Rigidbody2D>();
        colldr = transform.GetChild(0).GetComponent<Collider2D>();
        Vector3 initPos = Flyer.transform.position;
        initPos.x = -radius;
        Flyer.transform.position = initPos;
    }

    public override void DestroySelf()
    {
        Destroy(this.transform.GetChild(0).gameObject);
    }

    void Update()
    {
        BaseUpdate();
        Flyer.transform.position = MMMaths.RotatePointAroundPivot(Flyer.transform.position,Pivot.transform.position,perAngle);
    }
}
