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
