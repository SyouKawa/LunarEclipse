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
    [Header("当前物体的本体物理组件（不包含子物体或子弹等）")]
    public Rigidbody2D body;
    public BoxCollider2D colldr;
    
    //不同逐帧函数的倒计时列表
    protected List<int> Clocks;
    protected List<EveryFrameHandler> FrameFuncList;

    //受击操作委托类型
    public delegate void OnceBeHitHandler(OtherData data);
    //逐帧操作委托类型
    public delegate void EveryFrameHandler(int clockCount);
    
    //受击事件列表
    public OnceBeHitHandler onceBeHitEvent;

    /// <summary>
    /// 被击触发函数（检测到单次受击的时候调用）
    /// </summary>
    /// <param name="data"></param>
    public void OnBeHit(OtherData data){
        if(data!= null){
            onceBeHitEvent.Invoke(data);
        }else{
            Debug.LogWarning("当前透传数据为空，可能存在异常，请注意！");
        }
    }

    /// <summary>
    /// 添加逐帧倒计时及其对应要执行的函数列表
    /// </summary>
    /// <param name="sumTime">执行多长时间的倒计时</param>
    /// <param name="FrameFuncs">对应当前倒计时，要加入逐帧运行的函数列表</param>
    public void AddClockEvent(float sumTime,List<EveryFrameHandler> FrameFuncs)
    {
        // 1 将秒换算为帧数
        int count = SETools.TimeToFrameCount(sumTime);
        // 2 添加帧数钟
        if(Clocks == null){
            Clocks = new List<int>();
        }
        Clocks.Add(count);
        //获取当前时钟的下标
        int index = Clocks.Count - 1;

        // 3 添加时钟对应的执行函数列表
        if(FrameFuncList == null){
            FrameFuncList = new List<EveryFrameHandler>();
        }
        EveryFrameHandler curClockEvents = new EveryFrameHandler(FrameFuncs[0]);
        for(int i =1;i<FrameFuncs.Count;i++){
            curClockEvents += FrameFuncs[i];
        }
        FrameFuncList.Add(curClockEvents);
    }
}
