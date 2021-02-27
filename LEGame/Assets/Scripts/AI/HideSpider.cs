using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

public class HideSpider : Monster
{
    public enum Status
    {
        Patrol,
        Approch,
        Attack,
        Die
    }
    [Space(25,order = 0)]
    [Header("继承类的自定义属性及参数",order = 1)]
    public Status curState;
    public float speed;

    public float preHP;
    [Header("AI控制范围")]
    public float patrolRange;
    public float attackRange;
    [Tooltip("范围偏移量，用于调整Monster对玩家检测范围的位置")]
    public Vector2 offset;

    [Header("攻击CD及计时器")]
    public float atkCDSumTime;
    public float atkCDCount;

    //挨打时隐身，再绕边打
    private Vector2 atkPos;

    void Start()
    {
        curState = Status.Patrol;
        attackEvent +=AttackAnimation;
    }

    public override void Attack()
    {
        Debug.Log("Attack!");
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        Debug.Log(other.name);
        curState = Status.Approch;
    }

    //巡逻检测玩家是否处于攻击范围
    private void CheckStatus()
    {
        if(Target != null)
        {
            //按水平距离条件判定蜘蛛的状态
            float distance = Mathf.Abs(Target.transform.position.x-transform.position.x);
            if(distance >= patrolRange)
            {
                curState = Status.Patrol;
            }
            else if(distance >= attackRange && distance < patrolRange)
            {
                curState = Status.Approch;
            }
            //攻击切换不但要在范围内，还需要站到对应位置上的
            else if(distance < attackRange && Mathf.Abs(transform.position.x-atkPos.x) < 0.1f)
            {
                curState = Status.Attack;
            }
            else//如果不符合上述条件，就一直处于“正在接近玩家”的状态
            {
                curState = Status.Approch;
            }
        }
        else
        {
            //抛出异常
            Debug.Log("未找到玩家");
        }
    }

    //Monster是否需要转向:-1为翻转，1为保持
    private float NeedXFlip(Vector2 comparePos)
    {
        float needXFlip = 1;
        if(transform.position.x - comparePos.x > 0)
        {
            needXFlip = -1;
        }
        return needXFlip;
    }

    private void OnApproch()
    {
        //关闭碰撞体,变更rigidbody为Kinematic,防止重力在无碰撞体时产生影响
        colldr.enabled = false;
        body.bodyType = RigidbodyType2D.Kinematic;
        //计算发起攻击的地点
        atkPos = Target.transform.position;
        atkPos.y = transform.position.y;//仅横向移动
        //攻击地点为玩家身后,即玩家朝向的反方向再加上attckRange
        float dir = -1 * GameManager.Instance.GetPlayerDir();
        atkPos.x = Target.transform.position.x + (dir * attackRange);
        //依据攻击的目标地点，检查是否需要转向
        dir = NeedXFlip(atkPos);
        transform.localScale = new Vector3(dir * 1,1,1);
        body.velocity = new Vector2(dir * speed,0);
        //隐匿贴图
        sp.color = new Color(1,1,1,0.4f);
    }
    private void OnAttack()
    {
        //---攻击前初始化---
        //1.速度归零，防止移动导致的状态切换
        body.velocity = Vector2.zero;
        //2.打开碰撞体，保证碰撞伤害
        colldr.enabled = true;
        //3.切换回物理模拟，保证被击的击退效果
        body.bodyType = RigidbodyType2D.Dynamic;
        //4.保证攻击朝向：位于玩家背后，且与玩家朝向相同（防止玩家运动改变攻击地点可能导致的朝向问题）
        float dir = NeedXFlip(Target.transform.position);
        transform.localScale = new Vector3(dir * 1,1,1);
        //---运行所有攻击相关函数---
        attackEvent.Invoke();
    }

    private void AttackAnimation()
    {
        sp.color = new Color(1,1,1,1);
    }

    private void OnPatrol()
    {
        body.velocity = Vector2.zero;
        //TODO:蜘蛛织毛衣的动画
    }

    void Update()
    {
        BaseUpdate();
        CheckStatus();
        switch(curState)
        {
            case Status.Patrol:
                OnPatrol();
            break;
            case Status.Approch:
                OnApproch();
            break;
            case Status.Attack:
                OnAttack();
            break;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //显示范围是以local坐标中点将range分为两半的，检测范围也要是range*2，两者才能对应
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + (Vector3)offset,new Vector3(2 * patrolRange,3,2));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)offset,new Vector3(2 * attackRange,3,2));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,atkPos);
        if(curState == Status.Attack)
        {
            Gizmos.DrawLine(transform.position,Target.transform.position);
        }
    }
}
