using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

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
    public BoxCollider2D coll;

    [Space(30, order = 0)]
    [Header("碰撞检测", order = 1)]
    [Space(15, order = 2)]
    [Header("4方向检测标志位", order = 3)]
    public bool IsCollidingRight = false;
    public bool IsCollidingLeft = false;
    public bool IsCollidingCeiling = false;
    public bool IsCollidingGround = false;

    public int NumberOfHorizontalRays = 3;
    public int NumberOfVerticalRays = 3;
    public float _smallValue = 0.0001f;
    public float checkDelta;

    // 是否绘制碰撞线
    public bool DrawRaycastsGizmos = true;
    // 碰撞体边界
    public float top;
    public float bottom;
    public float left;
    public float right;

    [Header("天花板检测")]
    public Vector2 GroundPointOffset;
    public Vector2 GroundCheckSize;
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

    /// <summary>
    /// 每帧重置碰撞检测标志位
    /// </summary>
    private void InitCollideStatusByFrame()
    {
        IsCollidingLeft = false;
        IsCollidingRight = false;
        IsCollidingCeiling = false;
        IsCollidingGround = false;
    }

    /// <summary>
    /// 计算射线检测射线布局的上下左右边界（由碰撞体的偏移位置为原点，加减碰撞体一半的大小）
    /// </summary>
    private void CalculateColliderLimit()
    {
        //计算水平方向射线的上下边界
        top = coll.offset.y + (coll.size.y / 2f);
        bottom = coll.offset.y - (coll.size.y / 2f);
        //计算垂直方向射线的左右边界
        left = coll.offset.x - (coll.size.x / 2f);
        right = coll.offset.x + (coll.size.x / 2f);
    }

    /// <summary>
    /// 检查入参方向是否发生碰撞
    /// </summary>
    /// <param name="dir">需要检测的方向（地面，天花板，左侧墙，右侧墙）</param>
    /// <param name="limit">当前检测方向的边界值</param>
    /// <param name="isCollided">方向碰撞标志位</param>
    /// <param name="rayColor">射线检测的DebugGizmos颜色</param>
    private void CheckDirCollide(Vector2 dir,float limit,ref bool isCollided,Color rayColor)
    {
        //依次生成射线
        for(int i = 0;i < NumberOfHorizontalRays;i++)
        {
            Vector2 rayOriginPoint = Vector2.zero;
            //如果是水平方向，则在从下到上开始铺设射线检测
            if(Mathf.Abs(dir.x) > 0)
            {
                //线性插值计算上下边界中射线的原点，从下边界===》到上边界
                rayOriginPoint = Vector2.Lerp(
                            transform.position + new Vector3(0, bottom, 0),
                            transform.position + new Vector3(0, top, 0),
                            (float)i / (float)(NumberOfHorizontalRays - 1));
            }
            //如果是垂直方向，则在从左到右开始铺设射线检测
            else
            {
                rayOriginPoint = Vector2.Lerp(
                            transform.localPosition + new Vector3(left, 0, 0),
                            transform.localPosition + new Vector3(right, 0, 0),
                            (float)i / (float)(NumberOfHorizontalRays - 1));
            }

            //准备工作（结果列表初始化及层级设置）
            RaycastHit2D[] result = new RaycastHit2D[3];
            LayerMask mask = LayerMask.GetMask("Ground");
            //射线检测(射线原点，方向，长度（边界值+防撞delta值），检测图层)
            result[i] = Physics2D.Raycast(
                            rayOriginPoint, 
                            dir, 
                            checkDelta + Mathf.Abs(limit),
                            mask);
            //如果当前检测有，则置位标志位
            if (result[i])
            {
                isCollided = true;
            }
            
            //DebugTools
            if(DrawRaycastsGizmos)
            {
                //原点，方向，颜色
                Debug.DrawRay(rayOriginPoint, dir * (checkDelta + Mathf.Abs(limit)), rayColor);
            }
        }
    }

    void Start()
    {
        //物理组件初始化
        rig = GetComponent<Rigidbody2D>();
        sp = transform.GetChild(0).GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        //计算碰撞边界
        CalculateColliderLimit();

        weaponX.gameObject.SetActive(false);
        weaponY.gameObject.SetActive(false);

        LayerMask mask = LayerMask.GetMask("Monsters");
        filter.layerMask = mask;
    }

    void XFlipBody()
    {
        //Player 翻转
        float moveX = Input.GetAxisRaw("Horizontal");
        if (moveX != 0)
        {
            transform.localScale = new Vector3(moveX, 1, 1);
        }
    }

    void RigCheckMove()
    {
        //左右翻转
        XFlipBody();
        //速度调整
        if (Input.GetAxisRaw("Horizontal") > InputOffset.x)
        {
            rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x, moveSpeed * Time.fixedDeltaTime * 60, ref Xvelocity, XaccelerateTime), rig.velocity.y);
        }
        else if (Input.GetAxisRaw("Horizontal") < (InputOffset.x * -1))
        {
            rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x, moveSpeed * Time.fixedDeltaTime * 60 * -1, ref Xvelocity, XaccelerateTime), rig.velocity.y);
        }
        else if (Input.GetAxisRaw("Horizontal") == 0)
        {
            rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x, 0, ref Xvelocity, XaccelerateTime), rig.velocity.y);
        }
    }

    private bool CheckCeiling()
    {
        Collider2D coll = Physics2D.OverlapBox((Vector2)transform.position + CeilingPointOffset, CeilingCheckSize, 0, GroundLayerMask);

        if (coll != null)
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
        if (!isJumping)
        {
            result = true;
            initYpos = transform.position.y;
        }
        //如果在地面上且当前并没有输入跳跃动作====>切换为着地状态(可跳跃状态)
        if (!Input.GetKeyDown(KeyCode.C))
        {
            result = false;
        }
        return result;
    }

    //调整的重力加速度函数
    void Gravity()
    {
        //在原重力影响下，加入fallMulti作为乘数调整下落的加速度
        rig.velocity += Vector2.up * Physics2D.gravity.y * fallMulti * Time.fixedDeltaTime;
    }

    //TODO：临时的动画切换
    void SwitchSprite()
    {
        if (!isAttack)
        {
            if (isOnGround)
            {
                sp.sprite = texs[0];
            }
            if (isJumping && rig.velocity.y > 0)//速度大于0且是跳跃状态才切换
            {
                sp.sprite = texs[1];
            }
            if (rig.velocity.y < -0.2f)
            {
                sp.sprite = texs[2];
            }
        }
    }

    void Jump()
    {
        float delta = (Mathf.Abs(transform.position.y) - Mathf.Abs(initYpos));
        //处于上升状态
        if (rig.velocity.y >= 0 && delta < MaxHeight)
        {
            rig.velocity = new Vector2(rig.velocity.x, jumpInitSpeed);
        }
        //已达最大高度，变更为下落（此时无法再进入y速度需要大于0的上个if）
        else if (delta >= MaxHeight)
        {
            //达到最大高端，取消跳跃状态（相当于变为下坠）
            isJumping = false;
            rig.velocity = new Vector2(rig.velocity.x, jumpDownSpeed);
        }
    }

    private void Update()
    {
        //每帧初始化
        InitCollideStatusByFrame();
        //获取实时碰撞标志位
        CheckDirCollide(-transform.right,left,ref IsCollidingLeft,Color.red);
        CheckDirCollide(-transform.up,bottom,ref IsCollidingGround,Color.blue);
        CheckDirCollide(transform.up,top,ref IsCollidingCeiling,Color.green);
        CheckDirCollide(transform.right,right,ref IsCollidingRight,Color.yellow);
        //获取实时检测变量
        isOnGround = IsCollidingGround;
        collideCeiling = CheckCeiling();
        //对应状态的部分运动调整

        //跳跃检测
        if(Input.GetKeyDown(KeyCode.C))
        {
            //跳跃初始化（记录起跳高度和置位标志）
            if(isOnGround && !isJumping)
            {
                isJumping = true;
                initYpos = transform.position.y;
            }
        }

        //跳跃中
        if(isJumping)
        {
            Jump();
            if (collideCeiling)//如果跳跃过程中撞上天花板，则直接取消跳跃状态，并开始下落
            {
                isJumping = false;
                rig.velocity = new Vector2(rig.velocity.x, jumpDownSpeed);
            }
        }
        else//如果没有跳跃，也不在地上，说明在下坠
        {
            if(!isOnGround)
            {
                rig.velocity = new Vector2(rig.velocity.x, jumpDownSpeed);
            }
        }
    }

    private void FixedUpdate()
    {
        //Debug显示
        curVelocity = rig.velocity;
        //恒运行函数
        RigCheckMove();
        //是否打开重力影响
        if (GravityEffect)
        {
            Gravity();
        }

        //显示切换测试
        SwitchSprite();

        //攻击动画时钟  TODO：攻击动画
        if (attclk > 0)
        {
            attclk--;
        }
        else
        {
            isAttack = false;
            weaponX.gameObject.SetActive(false);
            weaponY.gameObject.SetActive(false);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            print("Prepare for DownAtk");
            if (!isAttack && Input.GetKey(KeyCode.X))
            {
                isAttack = true;
                attclk = 15;
                sp.sprite = texs[4];
                weaponY.gameObject.SetActive(true);
            }
        }
        else//没有按上下方向键则为平A
        {
            if (!isAttack && Input.GetKeyDown(KeyCode.X))
            {
                isAttack = true;
                attclk = 15;
                sp.sprite = texs[3];
                weaponX.gameObject.SetActive(true);
            }
        }
    }
    private void OnDrawGizmos()
    {
        //天花板检测
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)transform.position + CeilingPointOffset, CeilingCheckSize);
    }
}
