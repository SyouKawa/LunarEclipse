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
        //依赖外部脚本(GameManager)的Awake初始化，所以必须放到Start中的部分赋值
        Target = GameManager.Instance.player;
    }

    void Update()
    {
        BaseUpdate();
        Flyer.transform.position = MMMaths.RotatePointAroundPivot(Flyer.transform.position,Pivot.transform.position,perAngle);
    }
}
