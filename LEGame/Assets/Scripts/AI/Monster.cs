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
    //Monster的攻击目标
    protected GameObject Target;
    //被击退相关参数
    protected Vector2 backDir;
    protected float multi;//击退向量调整参数
    protected float halftime;//击退动画中间峰值

    void Awake(){
        //被击单次函数压入
        onceBeHitEvent = new OnceBeHitHandler(BeHitDamage);
        
        //初始化获取
        sp = transform.GetComponent<SpriteRenderer>();
        if(sp == null){
            sp = transform.GetChild(0).GetComponent<SpriteRenderer>();
        }

        body = transform.GetComponent<Rigidbody2D>();
        colldr = transform.GetComponent<BoxCollider2D>();
    }

    /// <summary>
    /// Monster受击时的击退动作
    /// </summary>
    /// <param name="count"></param>
    public virtual void BeHitBack(int count){
        if(count > 0){
            body.velocity = backDir*3;
        }else{
            body.velocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Monster受击时的颜色变化显示
    /// </summary>
    /// <param name="count"></param>
    public void BeHitColorChange(int count){
        if(count>0){
            sp.material.SetFloat("_FlashAmount", 1);
        }else{
            sp.material.SetFloat("_FlashAmount", 0);
        }
    }

    /// <summary>
    /// Monster受击时的伤害结算
    /// </summary>
    /// <param name="data"></param>
    public virtual void BeHitDamage(OtherData data){
        HP -= data.damage;
    }
    
    public virtual void Attack(){}

    public virtual void CheckDeath(){
        if(HP <= 0){
            Destroy(this.gameObject);
        }
    }
    
    protected void BaseUpdate()
    {
        //死亡检查
        CheckDeath();
        
        //逐帧函数列表触发
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
                List<EveryFrameHandler> duringHitFrame = new List<EveryFrameHandler>();
                duringHitFrame.Add(BeHitBack);
                duringHitFrame.Add(BeHitColorChange);
                AddClockEvent(0.2f,duringHitFrame);
            }
            else 
            {
                Debug.LogWarning("当前碰撞体未找到Player脚本，无法获取Player的攻击数值");
            }
        }
    }
}
