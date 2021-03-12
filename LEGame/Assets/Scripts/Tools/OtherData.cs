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
    public Player player;

    public OtherData(Player _player){player = _player;}
}
