using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitData
{
    //击退方向
    public Vector2 hitBackDir;
    public Player player;

    public HitData(Player _player){player = _player;}
}

public class Monster : MonoBehaviour
{
    [Header("Monster基本数值")]
    public float HP;
    public float Att;
    [Tooltip("Monster的攻击目标")]
    public GameObject Target;
    [Header("Monster的显示")]
    public SpriteRenderer sp;
    [Header("Monster物理性质")]
    public Rigidbody2D body;
    public Collider2D colldr;
    [Header("部分标志位")]
    public bool canDestroyRootNode;

    [Header("被击退相关参数")]
    public float hurtCount;
    public float hurtSumTime;
    public Vector2 backDir;
    public float multi;//击退向量调整参数
    public float halftime;//击退动画中间峰值


    public delegate void OnceBeHitHandler(HitData data);
    //受击逐帧操作委托类型
    public delegate void DotBeHitHandler();
    //攻击操作委托
    public delegate void AttackHandler();
    
    //受击事件列表
    public OnceBeHitHandler onceBeHitEvent;
    public DotBeHitHandler dotBehitEvent;

    public AttackHandler attackEvent; 
    private void Awake()
    {
        //将被击逐帧函数压入
        dotBehitEvent = new DotBeHitHandler(BeHitBack);
        
        //被击单次函数压入
        onceBeHitEvent = new OnceBeHitHandler(BeHitDamage);
        onceBeHitEvent += InitHitClock;//该函数中Hitdata未使用

        //攻击函数压入(攻击内容全部由不同敌人的override承担)
        attackEvent = new AttackHandler(Attack);

        //初始化标志位
        canDestroyRootNode = true;
        //初始化计时器
        hurtCount = 0;
        hurtSumTime = 0.2f;
        //初始化获取
        sp = transform.GetChild(0).GetComponent<SpriteRenderer>();
        body = transform.GetComponent<Rigidbody2D>();
        colldr = transform.GetComponent<Collider2D>();
    }

    //依赖外部脚本的Awake初始化，所以必须放到Start中的部分赋值
    private void Start()
    {
        Target = GameManager.Instance.player;
    }

#region  被击逐帧函数列表（无参）
    public virtual void BeHitBack()
    {
        //判断是否是前半段动画，前半段则取击退向量同向，后半段取击退向量反向
        int brehalf;
        if(hurtCount > halftime)
        {
            brehalf = 1;
        }
        else
        {
            brehalf = -1;
        }

        //每帧操作，执行击退位移,峰值时间折返
        //Kine形式下采用纯位移形式
        if(body.bodyType == RigidbodyType2D.Kinematic)
        {
            transform.Translate(brehalf * backDir.x * multi,brehalf *backDir.y *multi,0);
        }
        else //物理模拟形式下采用刚体速度形式
        {
            body.velocity = brehalf * backDir;
        }
    }

#endregion


#region 被击单次函数列表（带参）
    
    //触发函数（在外部调用）
    public void OnBeHit(HitData data)
    {
        if(data!= null)
        {
            onceBeHitEvent.Invoke(data);
        }
        else
        {
            //TODO:抛出异常
        }
    }

    public virtual void BeHitDamage(HitData data)
    {
        HP -= data.player.Att;
    }

    public void InitHitClock(HitData data)
    {
        hurtCount = hurtSumTime;
        sp.material.SetFloat("_FlashAmount", 1);
    }
    
#endregion

    public virtual void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public virtual void Attack(){}

    public virtual void CheckDeath()
    {
        if(HP <= 0 && canDestroyRootNode)
        {
            DestroySelf();
        }
    }
    
    public void BaseUpdate()
    {
        CheckDeath();
        //如果处于被击计时中，则循环触发逐帧受击函数
        if(hurtCount >= 0)
        {
            hurtCount -= Time.deltaTime;
            dotBehitEvent.Invoke();
        }
        else
        {
            sp.material.SetFloat("_FlashAmount", 0);
        }
    }
}