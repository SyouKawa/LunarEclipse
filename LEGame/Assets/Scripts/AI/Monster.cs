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
        //被击单次函数压入
        onceBeHitEvent = new OnceBeHitHandler(BeHitDamage);

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
    public virtual void BeHitBack(int count)
    {
        if(count > 0)
        {
            body.velocity = backDir*3;
        }
        else
        {
            body.velocity = Vector2.zero;
        }
    }

    public void InitHitClock(int count)
    {
        if(count>0)
        {
            sp.material.SetFloat("_FlashAmount", 1);
        }
        else
        {
            sp.material.SetFloat("_FlashAmount", 0);
        }
    }

#endregion


#region 被击单次函数列表（带参）

    public virtual void BeHitDamage(OtherData data)
    {
        HP -= data.damage;
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
        //倒着遍历，防止List中间element销毁时崩溃
        for(int i = Clocks.Count-1;i>=0;i--)
        {
            if(Clocks[i] >= 0)
            {
                FrameFuncList[i].Invoke(Clocks[i]);
                Clocks[i]--;
            }
            else
            {
                Clocks.RemoveAt(i);
                //FreeClockEvent(i);
                FrameFuncList.RemoveAt(i);
            }
        }
        CheckDeath();
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
                //建立被击逐帧函数队列委托
                List<EveryFrameHandler> handler = new List<EveryFrameHandler>();
                handler.Add(BeHitBack);
                handler.Add(InitHitClock);
                AddClockEvent(0.2f,handler);
            }
            else 
            {
                Debug.LogWarning("当前碰撞体未找到Player脚本，无法获取Player的攻击数值");
            }
        }
    }
}
