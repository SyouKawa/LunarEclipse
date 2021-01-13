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
    public float initXSpeed;
    public bool isGrounded;
    public float jumpInitSpeed;
    public Vector2 gravity = new Vector2(0,-10f);
    public Vector2 velocity = Vector2.zero;
    public GameObject tempPlayer;

    public float rangeofPatrol;
    public float checkline;
    public bool isHit;
    public bool isJumping;
    private BoxCollider2D body;


    void Start()
    {
        //isGrounded = true;
        isJumping = false;
        body = GetComponent<BoxCollider2D>();
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
        }
        else
        {
            isGrounded = false;
        }
    }

    void Jump()
    {
        isJumping = true;
        transform.position = new Vector2(transform.position.x,transform.position.y+0.7f);
        if(transform.position.x - tempPlayer.transform.position.x > 0)
        {
            velocity.x = -initXSpeed;
        }
        else
        {
            velocity.x = initXSpeed;
        }
        //给予上升的初始速度                            
        velocity.y += jumpInitSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //地面轮询检测
        CheckGround();
        //与是否在地面上相关的变量处理
        if(!isGrounded)
        {
            Vector2 pos = (Vector2)transform.position + velocity* Time.deltaTime;
            velocity += gravity *Time.deltaTime;
            transform.position = pos;
        }
        else
        {
            isJumping = false;
            velocity = Vector2.zero;
        }

        //受击检测
        if(isHit)
        {
            isHit = false;
            //受击后停止所有状态
            StopAllCoroutines();
            //受击动画
            StartCoroutine(BeHit(35));
        }

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
        Gizmos.DrawRay(transform.position,checkline*Vector2.up);
    }
}