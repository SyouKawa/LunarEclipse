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
public class JumpRusher : Monster
{
    public Status curState;
    public float initXSpeed;
    public bool isGrounded;
    public float jumpInitSpeed;
    public Vector2 velocity = Vector2.zero;

    public float rangeofPatrol;
    public float checkline;
    public bool isHit;
    public bool isJumping;
    private BoxCollider2D col;


    void Start()
    {
        //依赖外部脚本(GameManager)的Awake初始化，所以必须放到Start中的部分赋值
        Target = GameManager.Instance.player;
        isJumping = false;
        col = GetComponent<BoxCollider2D>();
    }

    void DestroySelf()
    {
        Debug.Log("I'm dead already.");
        Destroy(this.gameObject);
    }

    void CheckGround()
    {
        LayerMask mask = LayerMask.GetMask("Ground");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up,checkline,mask);
        Debug.Log(hit.collider);
        //接触到地面层，且脚下有东西（防止是墙面）是否在地面上判定
        if(hit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void Jump()
    {
        isJumping = true;
        if(transform.position.x - Target.transform.position.x > 0)
        {
            velocity.x = -initXSpeed;
        }
        else
        {
            velocity.x = initXSpeed;
        }
        //给予上升的初始速度
        velocity.y = jumpInitSpeed;
        body.velocity = velocity;
    }

    void Update()
    {
        BaseUpdate();
        //地面轮询检测
        CheckGround();
        //与是否在地面上相关的变量处理
        if(isGrounded)
        {
            isJumping = false;
            body.velocity = Vector2.zero;
        }

        //附近是否存在玩家
        if(Target == null)
        {
            curState = Status.Patrol;
            body.velocity = Vector2.zero;
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
            float distance = Vector2.Distance(Target.transform.position,transform.position);
            if(distance < rangeofPatrol)
            {
                //进玩家入攻击范围，且未在做其他动作，将巡逻状态变为“接近玩家”
                curState = Status.Approch;
                //如果没有在跳且在地面上，则产生一次跳跃
                if(!isJumping && isGrounded)
                {
                    Jump();
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
        Gizmos.DrawWireSphere(transform.position,rangeofPatrol);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position,-1 * checkline*Vector2.up);
    }
}