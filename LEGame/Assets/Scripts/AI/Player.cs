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
    [Tooltip("水平速度Smooth过渡时间")]
    public float XaccelerateTime;
    private float Xvelocity;

    [Header("垂直参数（跳跃）")]
    
    [Tooltip("是否受重力加速度影响？不受影响则跳跃的上升下降皆为匀速运动")]
    public bool GravityEffect;

    [Tooltip("当前人物速度（用于测试检查，不作为参数）")]
    public Vector2 curVelocity;
    
    [Tooltip("跳跃的初始速度")]
    public float jumpInitSpeed;

    [Tooltip("匀速下落速度（不受重力加速度影响，GravityEffect关闭时生效）")]
    public float jumpDownSpeed;
    
   [Tooltip("跳跃前的原本高度")] 
    public float initYpos;
    
    [Tooltip("跳跃可提升的最大高度")]
    public float MaxHeight;
    
    [Tooltip("重力加速度-调整参数")]
    public float fallMulti;

    [Header("Unity物理控制")]
    public Rigidbody2D rig;
    [Header("地面检测")]
    public Vector2 GroundPointOffset;
    public Vector2 GroundCheckSize;
    [Header("天花板检测")]
    public Vector2 CeilingPointOffset;
    public Vector2 CeilingCheckSize;

    public LayerMask GroundLayerMask;

    [Header("判定显示")]
    public bool isJumping;
    public bool isOnGround;
    public bool isAttack;
    public bool collideCeiling;
    #endregion

    //[Space(50)]
    [Header("显示配置")]
    public SpriteRenderer sp;
    public Sprite[] texs;
    public int attclk;

    [Header("子对象")]
    public GameObject weaponX;
    public GameObject weaponY;
    public ContactFilter2D filter;
    public Collider2D[] result;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        sp = transform.GetChild(0).GetComponent<SpriteRenderer>();
        
        weaponX.gameObject.SetActive(false);
        weaponY.gameObject.SetActive(false);
        
        LayerMask mask = LayerMask.GetMask("Monsters");
        filter.layerMask = mask;
    }

    void XFlipBody()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
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
            rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x,moveSpeed *Time.fixedDeltaTime *60 ,ref Xvelocity,XaccelerateTime), rig.velocity.y);
        }
        else if(Input.GetAxisRaw("Horizontal") < (InputOffset.x *-1))
        {
            rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x,moveSpeed *Time.fixedDeltaTime *60 *-1 ,ref Xvelocity,XaccelerateTime), rig.velocity.y);
        }
        else if(Input.GetAxisRaw("Horizontal") == 0)
        {
            rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x,0 ,ref Xvelocity,XaccelerateTime), rig.velocity.y);
        }
    }

    private bool CheckGround()
    {
        Collider2D coll =  Physics2D.OverlapBox((Vector2)transform.position + GroundPointOffset,GroundCheckSize,0,GroundLayerMask);
        if(coll !=null)
        {
            return true;
        }
        else return false;
    }

    private bool CheckCeiling()
    {
        Collider2D coll = Physics2D.OverlapBox((Vector2)transform.position+CeilingPointOffset,CeilingCheckSize,0,GroundLayerMask);
        
        if(coll != null)
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
        if(Input.GetKeyDown(KeyCode.C) && !isJumping)
        {
            result = true;
            initYpos = transform.position.y;
        }
        //如果在地面上且当前并没有输入跳跃动作====>切换为着地状态(可跳跃状态)
        if(!Input.GetKeyDown(KeyCode.C))
        {
            result = false;
        }
        return result;
    }

    //调整的重力加速度函数
    void Gravity()
    {
        //在原重力影响下，加入fallMulti作为乘数调整下落的加速度
        rig.velocity += Vector2.up *Physics2D.gravity.y * fallMulti * Time.fixedDeltaTime;
    }

    //TODO：临时的动画切换
    void SwitchSprite()
    {
        if(!isAttack)
        {
            if(isOnGround)
            {
                sp.sprite = texs[0];
            }
            if(isJumping && rig.velocity.y > 0)//速度大于0且是跳跃状态才切换
            {
                sp.sprite = texs[1];
            }
            if(rig.velocity.y < 0)
            {
                sp.sprite = texs[2];
            }
        }
    }

    void Jump()
    {
        float delta = (Mathf.Abs(transform.position.y) - Mathf.Abs(initYpos));
        //print("delta:" + delta);
        //处于上升状态
        if(rig.velocity.y >= 0 && delta < MaxHeight)
        {
            rig.velocity = new Vector2(rig.velocity.x,jumpInitSpeed);
        }
        //已达最大高度，变更为下落（此时无法再进入y速度需要大于0的上个if）
        else if(delta >= MaxHeight)
        {
            rig.velocity = new Vector2(rig.velocity.x,jumpDownSpeed);
        }
    }

    private void Update()
    {
        //获取实时检测变量
        isOnGround = CheckGround();
        collideCeiling = CheckCeiling();
        //对应状态的部分运动调整
        if(isOnGround)//如果在地面上的进一步操作和检测
        {
            //在地面才可以跳跃
            isJumping = CheckJump();
        }
        else//如果不在地面上的进一步操作和检测
        {
            if(!isJumping)
            {
                rig.velocity = new Vector2(rig.velocity.x,jumpDownSpeed);
            }
            if(collideCeiling)//如果装上天花板，则直接取消跳跃状态，并开始下落
            {
                isJumping = false;
                rig.velocity = new Vector2(rig.velocity.x,jumpDownSpeed);
            }
        }
        if(isJumping)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        //Debug显示
        curVelocity = rig.velocity;
        
        //恒运行函数
        RigCheckMove();
        //是否打开重力影响
        if(GravityEffect)
        {
            Gravity();
        }

        //显示切换测试
        SwitchSprite();

        //攻击动画时钟  TODO：攻击动画
        if(attclk > 0)
        {
            attclk --;
        }
        else
        {
            isAttack = false;
            weaponX.gameObject.SetActive(false);
            weaponY.gameObject.SetActive(false);
        }

        if(Input.GetKey(KeyCode.DownArrow))
        {
            print("Prepare for DownAtk");
            if(!isAttack && Input.GetKey(KeyCode.X))
            {
                isAttack = true;
                attclk = 15;
                sp.sprite = texs[4];
                weaponY.gameObject.SetActive(true);
            }
        }
        else//没有按上下方向键则为平A
        {
            if(!isAttack && Input.GetKeyDown(KeyCode.X))
            {
            isAttack = true;
            attclk = 15;
            sp.sprite = texs[3];
            weaponX.gameObject.SetActive(true);
            //TODO：作为Debug模块，输出所有攻击到的碰撞体接触点
            // Collider2D att = weaponX.GetComponent<Collider2D>();
            // int count =  Physics2D.GetContacts(att,result);
            // if(count >0)
            // {
            //     Monster enemy = result[0].GetComponentInParent<Monster>();
            //     Vector2 dir =  enemy.transform.position - transform.position;
            //     dir = dir.normalized;
            // }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + GroundPointOffset,GroundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + CeilingPointOffset,CeilingCheckSize);
    }
}
