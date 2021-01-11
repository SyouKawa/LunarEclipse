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
    public bool isGrounded;
    public float jumpInitSpeed;
    public Vector2 gravity = new Vector2(0,-10f);
    public Vector2 velocity = Vector2.zero;
    public GameObject tempPlayer;

    public float rangeofPatrol;
    public float checkline;
    public bool isHit;
    public bool isLock;
    private BoxCollider2D body;


    void Start()
    {
        isGrounded = true;
        body = GetComponent<BoxCollider2D>();
    }

    void DestroySelf(){}

    void CheckGround()
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, -1*Vector3.up,3f,7);
        LayerMask mask = LayerMask.GetMask("Ground");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up,checkline,mask);
        Debug.Log(hit.collider);
        //接触到地面层，且脚下有东西（防止是墙面）是否在地面上判定
        if(hit.collider != null)
        {
            isGrounded = true;
            if(!isLock)
            {
                velocity = Vector2.zero;
            }
        }
        else
        {
            isGrounded = false;
            velocity += gravity *Time.deltaTime;
            Vector2 pos = (Vector2)transform.position + velocity* Time.deltaTime;
            transform.position = pos;
        }
    }

    void Jump()
    {
        isLock = true;
        Vector2 dir = Vector2.zero;
        if(transform.position.x - tempPlayer.transform.position.x > 0)
        {
            velocity.x = -3;
        }
        else
        {
            velocity.x = 3;
        }
        //给予上升的初始速度                            
        velocity.y += jumpInitSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //循环检测是否在地面上
        CheckGround();
        //附近是否存在玩家
        if(tempPlayer == null)
        {
            curState = Status.Patrol;
            velocity = Vector2.zero;
        }
        //检测到有玩家时的AI
        else
        {
            //死亡判定
            if(HP <= 0)
            {
                curState = Status.Die;
                //死亡时立即垂直下落
                velocity.x = 0;
                DestroySelf();
            }
            //按距离条件判定：是否进入对玩家的攻击状态
            float distance = Vector2.Distance(tempPlayer.transform.position,transform.position);
            if(!isLock && distance < rangeofPatrol)
            {
                //进玩家入攻击范围，且未在做其他动作，将巡逻状态变为“接近玩家”
                curState = Status.Approch;
                //处理
                Jump();
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
        Gizmos.DrawRay(transform.position,checkline*Vector2.up);
    }
}