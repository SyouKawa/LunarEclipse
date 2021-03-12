using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 带HP物体：除了Player及怪物之外，可摧毁的箱子或墙体亦可使用
/// </summary>
public class HasHPObject : MonoBehaviour
{
    [Header("基类HasHPObject的基本数值")]
    public float HP;
    public float Att;
    [Header("当前物体的本体物理组件（不包含子物体或子弹等）")]
    public Rigidbody2D body;
    public BoxCollider2D colldr;

    //受击一次性操作委托类型
    public delegate void OnceBeHitHandler(OtherData data);
    //受击逐帧操作委托类型
    public delegate void DotBeHitHandler();
    //攻击操作委托
    public delegate void AttackHandler();
    
    //受击事件列表
    public OnceBeHitHandler onceBeHitEvent;
    public DotBeHitHandler dotBehitEvent;
    //攻击操作委托
    public AttackHandler attackEvent; 

    //触发函数（检测到单次受击的时候调用）
    public void OnBeHit(OtherData data)
    {
        if(data!= null)
        {
            onceBeHitEvent.Invoke(data);
        }
        else
        {
            Debug.LogWarning("当前透传数据为空，可能存在异常，请注意！");
        }
    }

}
