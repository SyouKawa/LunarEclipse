using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class FlyerAround : MonoBehaviour
{
    public GameObject Pivot;
    public GameObject Flyer;

    public float HP;
    public float preHP;

    public bool isHit;

    public float perAngle;

    public float radius;
    void Start()
    {
        preHP = HP;
        Vector3 initPos = Flyer.transform.position;
        initPos.x = -radius;
        Flyer.transform.position = initPos;
    }

    void DestroySelf()
    {
        Debug.Log("I'm dead already.");
    }

    IEnumerator BeHit(float damage)
    {
        HP = HP - damage;
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = MMColors.Red;
        yield return new WaitForSeconds(0.5f);
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.white;
    }

    void Update()
    {
        if(isHit)
        {
            isHit = false;
            //受击后停止所有状态
            StopAllCoroutines();
            //受击动画
            StartCoroutine(BeHit(35));
        }
        if(HP < preHP)
        {
            //更新血量，等待下一次攻击判定
            preHP = HP;
        }
        if(HP <= 0)
        {
            //停止环绕转动
            perAngle = 0;
            DestroySelf();
        }
        Flyer.transform.position = MMMaths.RotatePointAroundPivot(Flyer.transform.position,Pivot.transform.position,perAngle);
    }
}
