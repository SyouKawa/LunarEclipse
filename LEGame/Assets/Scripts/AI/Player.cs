using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Player Info
    [Header("Player人物信息")]
    public float HP;
    public float Att;
    #endregion
    #region Physics VARs

    [Header("水平参数（走动）")]
    public float moveSpeed;
    public Vector2 InputOffset;
    public float accelerateTime;
    private float Xvelocity;

    [Header("垂直参数（跳跃）")]
    public float jumpSpeed;
    public float fallMulti;
    public float lowJumpMulti;

    [Header("Unity物理控制")]
    public Rigidbody2D rig;

    public Vector2 PointOffset;
    public Vector2 Size;
    public LayerMask GroundLayerMask;

    [Header("判定显示")]
    public bool isJumping;
    public bool isOnGround;

    public bool isAttack;
    #endregion

    //[Space(50)]
    [Header("显示配置")]
    public SpriteRenderer sp;
    public Sprite[] texs;
    public int attclk;

    [Header("子对象")]
    public GameObject fighter;
    public ContactFilter2D filter;
    public Collider2D[] result;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        sp = transform.GetChild(0).GetComponent<SpriteRenderer>();
        fighter.gameObject.SetActive(false);
        LayerMask mask = LayerMask.GetMask("Monsters");
        filter.layerMask = mask;
    }

    void XFlipBody()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        // Vector2 p = transform.position;
        // p.x += moveX * moveSpeed * Time.deltaTime;
        // transform.position = p;
        //改变朝向
        if (moveX != 0)
        {
            transform.localScale = new Vector3(moveX, 1,1);
        }
    }
    void RigCheckMove()
    {
        //左右翻转
        XFlipBody();
        //速度调整
        if(Input.GetAxisRaw("Horizontal") > InputOffset.x)
        {
            rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x,moveSpeed *Time.fixedDeltaTime *60 ,ref Xvelocity,accelerateTime), rig.velocity.y);
        }
        else if(Input.GetAxisRaw("Horizontal") < (InputOffset.x *-1))
        {
            rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x,moveSpeed *Time.fixedDeltaTime *60 *-1 ,ref Xvelocity,accelerateTime), rig.velocity.y);
        }
        else if(Input.GetAxisRaw("Horizontal") == 0)
        {
            rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x,0 ,ref Xvelocity,accelerateTime), rig.velocity.y);
        }
    }

    bool CheckGround()
    {
        Collider2D coll =  Physics2D.OverlapBox((Vector2)transform.position + PointOffset,Size,0,GroundLayerMask);
        if(coll !=null)
        {
            return true;
        }
        else return false;
    }

    bool CheckJump()
    {
        //如果不属于以下两种状态，则保持上一帧的isJumping状态
        bool result = isJumping;
        //如果在按跳跃键且当前并非跳跃状态====>切换为跳跃状态
        if(Input.GetAxis("Jump") == 1 && !isJumping)
        {
            result = true;
        }
        //如果在地面上且当前并没有输入跳跃动作====>切换为着地状态(可跳跃状态)
        if(Input.GetAxis("Jump") == 0)
        {
            result = false;
        }
        return result;
    }
    //调整的下落函数
    void Gravity()
    {
        if(rig.velocity.y < 0)
        {
            rig.velocity += Vector2.up *Physics2D.gravity.y * (fallMulti-1) * Time.fixedDeltaTime;
        }
        //跳跃上升阶段，且没有按下跳跃键的小跳
       else if(rig.velocity.y>0 && Input.GetAxis("Jump")!=1)
        {
            rig.velocity +=Vector2.up *Physics2D.gravity.y *(lowJumpMulti -1) * Time.fixedDeltaTime;
        }
    }

    void SwitchSprite()
    {
        if(!isAttack)
        {
            if(rig.velocity.y == 0)
            {
                sp.sprite = texs[0];
            }
            if(rig.velocity.y > 0)
            {
                sp.sprite = texs[1];
            }
            if(rig.velocity.y < 0)
            {
                sp.sprite = texs[2];
            }
        }
    }
    void FixedUpdate()
    {
        //获取实时检测变量
        isOnGround = CheckGround();
        isJumping = CheckJump();
        //恒运行函数
        RigCheckMove();
        Gravity();

        //显示切换测试
        SwitchSprite();
        
        //对应状态的部分运动调整
        if(isOnGround)
        {
            if(isJumping)
            {
                rig.velocity = new Vector2(rig.velocity.x, jumpSpeed);
            }
        }


        if(attclk > 0)
        {
            attclk --;
        }
        else
        {
            isAttack = false;
            fighter.gameObject.SetActive(false);
        }

        if(!isAttack && Input.GetKeyDown(KeyCode.X))
        {
            isAttack = true;
            attclk = 15;
            sp.sprite = texs[3];

            fighter.gameObject.SetActive(true);
            // Collider2D att = fighter.GetComponent<Collider2D>();
            // int count =  Physics2D.GetContacts(att,result);
            // print(count);
            // print(result[0].name);

            // if(count >0)
            // {
            //     Monster enemy = result[0].GetComponentInParent<Monster>();
            //     Vector2 dir =  enemy.transform.position - transform.position;
            //     dir = dir.normalized;
            //     //触发敌人被击
            //     enemy.backDir = dir;
            //     enemy.OnBeHit(new HitData(this));
            // }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + PointOffset,Size);
    }
}
