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
    public List<int> Clocks;
    [Header("当前物体的本体物理组件（不包含子物体或子弹等）")]
    public Rigidbody2D body;
    public BoxCollider2D colldr;

    //受击操作委托类型
    public delegate void OnceBeHitHandler(OtherData data);
    //攻击操作委托
    public delegate void AttackHandler();
    //逐帧操作委托类型
    public delegate void EveryFrameHandler();
    
    //受击事件列表
    public OnceBeHitHandler onceBeHitEvent;
    //攻击操作列表
    public AttackHandler attackEvent; 
    //逐帧操作列表
    public EveryFrameHandler everyFrameEvent;

    //被击触发函数（检测到单次受击的时候调用）
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

    //添加逐帧触发函数
    public void AddClockEvent(float sumTime,EveryFrameHandler FrameFunc)
    {
        //将秒换算为帧数
        int count = Mathf.CeilToInt(sumTime/0.02f);
        //添加帧数钟
        if(Clocks == null)
        {
            Clocks = new List<int>();
            Clocks.Add(count);
        }
        else
        {
            Clocks.Add(count);
        }

        //获取当前时钟的下标
        int index = Clocks.Count - 1;
        void OnClock()
        {
            if(Clocks[index] > 0)
            {
                //每帧倒计时
                Clocks[index]--;
                print(Clocks[index]);
            }
            else//时间到则删除该逐帧事件和倒计时匿名函数
            {
                everyFrameEvent -= FrameFunc;
                everyFrameEvent -= OnClock;
            }
        }

        if(everyFrameEvent == null)
        {
            everyFrameEvent = new EveryFrameHandler(FrameFunc);
            everyFrameEvent += OnClock;
        }
        else
        {
            everyFrameEvent += FrameFunc;
            everyFrameEvent += OnClock;
        }
    }
}
