using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float HP;
    public float Att;

    public virtual void BeHit(float damage){}
}
