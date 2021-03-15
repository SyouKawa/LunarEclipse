using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 透传数据
/// </summary>
public class OtherData
{
    //击退方向
    public Vector2 hitBackDir;
    //需要传输的单次伤害
    public float damage;

    public OtherData(float _damage){ damage = _damage;}
}

public static class SETools
{
    /// <summary>
    /// 计算对应时间需要花费多少帧
    /// </summary>
    /// <param name="timeCount">需要计算的时间</param>
    /// <returns></returns>
    public static int TimeToFrameCount(float timeCount){
        return Mathf.CeilToInt(timeCount/0.02f);
    }

    /// <summary>
    /// 如果当前layerMask中包含layer返回true
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool isLayerContained(this LayerMask mask, int layer)
    {
        return ((mask.value & (1 << layer)) > 0);
    }

    /// <summary>
    /// 如果当前layerMask中包含入参的物体所在的layer返回true
    /// </summary>
    /// <param name="mask"></param>
    /// <param name="gameobject"></param>
    /// <returns></returns>
    public static bool isLayerContained(this LayerMask mask, GameObject gameobject)
    {
        return ((mask.value & (1 << gameobject.layer)) > 0);
    }
}
