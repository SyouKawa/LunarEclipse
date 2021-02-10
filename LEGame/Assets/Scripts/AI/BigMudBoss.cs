using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMudBoss : MonoBehaviour
{
    // Start is called before the first frame update
    public enum Status
    {
        Hide,
        StartAnim,
        Part1,
        Part2,
        Angry,
        FakeDeath,
        Die
    }
    public int HP;

    public Vector2 leftLimit;
    public Vector2 rightLimit;

    public Status curState;
    public bool isLock;
    public GameObject Fist1; 
    public GameObject tempTarget;
    public Vector2 prePos;
    public float groundY;

    public int timer1;
    public float hurtSumTime;
    public float hurtCount;
    private SpriteRenderer sp;
    void Start()
    {
        //curState = Status.Hide;
        isLock = false;
        timer1 = 0;
        hurtCount = hurtSumTime;
    }

    void FallFirstFist()
    {
        timer1 += 1;
        if(timer1<=40)
        {
            Vector2 pos = tempTarget.transform.position;
            pos.y = 3;
            Fist1.transform.position = Vector2.MoveTowards(Fist1.transform.position,pos,0.2f);
        }
        if(timer1 > 40 && timer1<=50)
        {
            prePos = Fist1.transform.localPosition;
            prePos.y = groundY;
            
        }
        if(timer1 > 50)
        {
            Fist1.transform.localPosition = Vector2.MoveTowards(Fist1.transform.localPosition,prePos,0.5f);
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

    public void Hurt(int damage)
    {
        //伤害
        HP -= damage;
        isLock = true;
        //伤害显示
        hurtCount = hurtSumTime;
        sp.material.SetFloat("_FlashAmount",1);
    }

    void HurtShader()
    {
        if(hurtCount <= 0)
        {
            sp.material.SetFloat("_FlashAmount", 0);
            isLock = false;
        }
        else
        {
            hurtCount -= Time.deltaTime;
        }
    }

    void Update()
    {
        if(isLock)
        {
            HurtShader();
        }
    }
    void FixedUpdate()
    {
        if(curState == Status.Angry)
        {
            timer1 = 0;
            //PlayAnimation("Angry");
        }
        if(curState == Status.Part1)
        {
            FallFirstFist();
        }
        if(curState == Status.Part2)
        {
            RushAndFall();
        }
    }
}
