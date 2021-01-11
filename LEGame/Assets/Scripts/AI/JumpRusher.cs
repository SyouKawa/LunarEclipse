using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public enum Status
{
    Patrol,
    Approch,
    Die
}
public class JumpRusher : MonoBehaviour
{
    public float HP;
    public Status curState;
    public float angle;
    private bool isGrounded;
    public float jumpInitSpeed;
    public Vector2 gravity = new Vector2(0,-10f);
    public Vector2 velocity = Vector2.zero;
    public GameObject tempPlayer;

    public float rangeofPatrol;
    public bool isLock;
    public bool isHit;

    private BoxCollider2D body;


    void Start()
    {
        isGrounded = true;
    }

    void DestroySelf(){}

    // Update is called once per frame
    void Update()
    {
        if(body.IsTouchingLayers("Ground"){}
        
        if(tempPlayer == null)
        {
            curState = Status.Patrol;
            velocity = Vector2.zero;
        }
        //检测到有玩家时的AI
        else
        {
            if(HP <= 0)
            {
                curState = Status.Die;
                //死亡时立即垂直下落
                velocity.x = 0;
                DestroySelf();
            }
            //按距离条件判定：是否进入对玩家的攻击状态
            float distance = Vector2.Distance(tempPlayer.transform.position,transform.position);
            if(distance < rangeofPatrol)
            {
                //进玩家入攻击范围，且未在做其他动作，将巡逻状态变为“接近玩家”
                curState = Status.Approch;
                //处理
                if(isGrounded)
                {
                    Vector2 dir = Vector2.zero;
                    if(transform.position.x - tempPlayer.transform.position.x > 0)
                    {
                        velocity.x = -1;
                    }
                    else
                    {
                        velocity.x = 1;
                    }
                    //给予上升的初始速度                            
                    velocity.y += jumpInitSpeed;
                }
                else
                {
                    //每秒受重力作用减少速度
                    velocity += gravity *Time.deltaTime;
                    Vector2 pos = (Vector2)transform.position + velocity* Time.deltaTime;
                }
            }
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
    }
}
