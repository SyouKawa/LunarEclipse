using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class GoBehinder : MonoBehaviour
{
    public enum Status
    {
        Patrol,
        Approch,
        Attack,
        Die
    }
    
    public Status curState;
    public float speed;
    public float HP;
    public float preHP;
    public float rangeofPatrol;
    public bool isLock;
    public bool isHit;
    public GameObject tempPlayer;

    private Vector2 behindPos;

    //挨打时钻地，再绕边打

    void Start()
    {
        curState = Status.Patrol;
        preHP = HP;
        isLock = false;
    }

    IEnumerator Attack()
    {
        Debug.Log("Attack!");
        yield return new WaitForSeconds(1f);
        isLock = false;
    }

    void DestroySelf()
    {
        Debug.Log("I'm dead already.");
    }

    IEnumerator BeHit(float damage)
    {
        HP = HP - damage;
        GetComponent<SpriteRenderer>().color = MMColors.Red;
        yield return new WaitForSeconds(0.5f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    void Update()
    {
        //TODO:等待Player代码部分
        //受击测试
        if(isHit)
        {
            isHit = false;
            isLock = false;
            //受击后停止所有状态
            StopAllCoroutines();
            //受击动画
            StartCoroutine(BeHit(35));
        }
        //未巡逻到有Player，接着巡逻
        if(tempPlayer == null)
        {
            isLock = false;
            curState = Status.Patrol;
        }
        //检测到有玩家时的AI
        else
        {
            //死亡判定
            if(HP <= 0)
            {
                isLock = true;
                curState = Status.Die;
                DestroySelf();
            }
            
            //受击判定
            //在目前站位中有没有被打？被打绕边，再被打再绕边
            if(!isLock && HP < preHP)
            {
                //更新血量，等待下一次攻击判定
                preHP = HP;
                curState = Status.Approch;
                isLock = false;
            }
            //攻击判定
            if(!isLock && curState == Status.Attack)
            {
                isLock = true;
                StartCoroutine(Attack());//每2秒攻击一次
            }
            //按距离条件判定：是否进入对玩家的攻击状态
            float distance = Vector2.Distance(tempPlayer.transform.position,transform.position);
            if(distance < rangeofPatrol)
            {
                //进玩家入攻击范围，且未在做其他动作，将巡逻状态变为“接近玩家”
                if(!isLock && curState == Status.Patrol)
                {
                    curState = Status.Approch;
                    isLock = false;
                }
                //未做其他动作且准备接近时，计算要前往的位置
                if(!isLock && curState == Status.Approch)
                {
                    isLock = true;
                    behindPos = transform.position;
                    if(transform.position.x - tempPlayer.transform.position.x > 0)
                    {
                        behindPos.x = tempPlayer.transform.position.x - 1.5f;
                    }
                    else
                    {
                        behindPos.x = tempPlayer.transform.position.x + 1.5f;
                    }
                }
                //处理逐帧连续动作
                if(isLock)
                {
                    if(curState == Status.Approch)
                    {
                        //动作未完成
                        if(Vector2.Distance(transform.position,behindPos) > 0.1)
                        {
                            transform.position = Vector2.MoveTowards(transform.position,behindPos,speed*Time.deltaTime);
                        }
                        //判定为完成动作，解锁并发动攻击
                        else
                        {
                            isLock = false;
                            curState = Status.Attack;
                        }
                    }
                }
            }
            //不在范围内，依旧巡逻
            else
            {
                curState = Status.Patrol;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position,new Vector3(rangeofPatrol,3,2));
        if(curState == Status.Attack)
        {
            Gizmos.DrawLine(transform.position,tempPlayer.transform.position);
        }
    }
}
