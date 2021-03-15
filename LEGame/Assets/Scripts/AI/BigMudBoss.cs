using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMudBoss : Monster
{
    // Start is called before the first frame update
    public enum Status
    {
        Hide,
        StartAnim,
        FistAtk,
        FakeDeath,
        Die
    }

    public Vector2 leftLimit;
    public Vector2 rightLimit;

    public Status curState;
    public bool isLock;
    public GameObject Fist; 
    public GameObject tempTarget;
    public Vector2 prePos;
    public float groundY;

    public int timer1;
    void Start()
    {
        //curState = Status.Hide;
        isLock = false;
        timer1 = 0;
    }

    void FallFirstFist()
    {
        timer1 += 1;
        if(timer1<=40)
        {
            Vector2 pos = tempTarget.transform.position;
            pos.y = 3;
            Fist.transform.position = Vector2.MoveTowards(Fist.transform.position,pos,0.2f);
        }
        if(timer1 > 40 && timer1<=50)
        {
            prePos = Fist.transform.localPosition;
            prePos.y = groundY;
            
        }
        if(timer1 > 50)
        {
            Fist.transform.localPosition = Vector2.MoveTowards(Fist.transform.localPosition,prePos,0.5f);
        }
        if(timer1 == 80)
        {
            timer1 = 0;
        }
    }

    void RushAndFall()
    {
        //随机1--冲撞，随机2--下落
    }

    // public override void BeHit(float damage)
    // {
    //     //伤害
    //     HP -= damage;
    //     isLock = true;
    //     //伤害显示
    //     hurtCount = hurtSumTime;
    //     sp.material.SetFloat("_FlashAmount",1);
    // }

    // void HurtShader()
    // {
    //     if(hurtCount <= 0)
    //     {
    //         sp.material.SetFloat("_FlashAmount", 0);
    //         isLock = false;
    //     }
    //     else
    //     {
    //         hurtCount -= Time.deltaTime;
    //     }
    // }

    void Update()
    {
        
    }
    void FixedUpdate()
    {
        // if(curState == Status.Angry)
        // {
        //     timer1 = 0;
        //     //PlayAnimation("Angry");
        // }
        // if(curState == Status.Part1)
        // {
        //     FallFirstFist();
        // }
        // if(curState == Status.Part2)
        // {
        //     RushAndFall();
        // }
    }
}
