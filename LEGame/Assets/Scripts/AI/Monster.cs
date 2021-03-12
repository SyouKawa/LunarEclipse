using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : HasHPObject
{
    [Space(20,order = 0)]
    [Header("Monster类自定义参数",order = 1)]
    [Space(5,order = 2)]
    [Header("Monster的显示",order = 3)]
    public SpriteRenderer sp;
    [Tooltip("Monster的攻击目标")]
    public GameObject Target;
    [Header("部分标志位")]
    public bool canDestroyRootNode;
    [Header("被击退相关参数")]
    public float hurtCount;
    public float hurtSumTime;
    public Vector2 backDir;
    public float multi;//击退向量调整参数
    public float halftime;//击退动画中间峰值

    void Awake()
    {
        //将被击逐帧函数压入
        dotBehitEvent = new DotBeHitHandler(BeHitBack);
        
        //被击单次函数压入
        onceBeHitEvent = new OnceBeHitHandler(BeHitDamage);
        onceBeHitEvent += InitHitClock;//该函数中OtherData未使用

        //攻击函数压入(攻击内容全部由不同敌人的override承担)
        attackEvent = new AttackHandler(Attack);

        //初始化标志位
        canDestroyRootNode = true;
        //初始化计时器
        hurtCount = 0;
        hurtSumTime = 0.2f;
        //初始化获取
        sp = transform.GetComponent<SpriteRenderer>();
        if(sp == null)
        {
            sp = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }
        body = transform.GetComponent<Rigidbody2D>();
        colldr = transform.GetComponent<BoxCollider2D>();
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

    public virtual void BeHitDamage(OtherData data)
    {
        HP -= data.damage;
    }

    public void InitHitClock(OtherData data)
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
    
    protected void BaseUpdate()
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        print("enter");
        if (collision.gameObject.tag == "Weapon")
        {
            print("enter W");
            //先利用节点关系查找施加攻击的主物体
            HasHPObject player = collision.transform.parent.GetComponent<HasHPObject>();
            //如果没有找到，则查找是否有挂载“攻击类脚本”（例如：没有主节点的已经飞出的子弹或者飞镖等武器）
            if(player == null)
            {
                collision.gameObject.GetComponent<Weapon>();
            }

            if(player != null)
            {
                backDir = transform.position - player.transform.position;
                backDir = backDir.normalized;
                OtherData data = new OtherData(player.Att);
                data.hitBackDir = backDir;
                //触发被击
                OnBeHit(data);
            }
            else 
            {
                Debug.LogWarning("当前碰撞体未找到Player脚本，无法获取Player的攻击数值");
            }
        }
    }
}
