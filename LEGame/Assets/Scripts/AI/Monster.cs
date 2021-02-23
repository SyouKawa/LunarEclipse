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
    public float HP;
    public float Att;

    public SpriteRenderer sp;
    public float hurtCount;

    public float hurtSumTime;

    public Vector2 backDir;

    public float multi;//击退向量调整参数
    public float halftime;//击退动画中间峰值

    //受击单次操作委托类型
    public delegate void OnceBeHitHandler(HitData data);
    //受击逐帧操作委托类型
    public delegate void DotBeHitHandler();
    //受击事件列表
    //public event BeHitHandler BeHitEvent;
    public OnceBeHitHandler onceBeHitEvent;
    public DotBeHitHandler dotBehitEvent;
    private void Awake()
    {
        //将被击逐帧函数压入
        dotBehitEvent = new DotBeHitHandler(BeHitBack);
        
        //被击单次函数压入
        onceBeHitEvent = new OnceBeHitHandler(BeHitDamage);
        onceBeHitEvent += InitHitClock;//该函数中Hitdata未使用
        
        //初始化计时器
        hurtCount = 0;
        hurtSumTime = 0.2f;
    }

#region  被击逐帧函数列表（无参）
    public virtual void BeHitBack()
    {
        //每帧操作，执行击退位移
        if(hurtCount > halftime)
        {
            transform.Translate(backDir.x * multi,backDir.y *multi,0);
        }
        else
        {
            transform.Translate(-backDir.x * multi,-backDir.y *multi,0);
        }
        //enemy.BeHit(player.GetComponent<Player>().Att);
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

    public void BaseUpdate()
    {
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
