using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MoreMountains.Tools;

public class Player : HasHPObject
{
    [Space(20,order = 0)]
    [Header("Player类自定义参数",order = 1)]
    #region Adjust Vars
    [Space(5,order =2)]
    [Header("可调参数",order = 3)]
    [Tooltip("是否受重力加速度影响？不受影响则跳跃的上升下降皆为匀速运动")]
    public bool GravityEffect;
    [Tooltip("重力加速度-调整参数")]
    protected float fallMulti;
    public float moveSpeed;
    protected Vector2 InputOffset;
    [Tooltip("水平速度Smooth过渡时间")]
    protected float XaccelerateTime;
    protected float Xvelocity;
    [Tooltip("跳跃的初始速度")]
    public float jumpInitSpeed;
    [Tooltip("匀速下落速度（不受重力加速度影响，GravityEffect关闭时生效）")]
    public float jumpDownSpeed;
    [Tooltip("跳跃可提升的最大高度")]
    public float MaxHeight;
    [HideInInspector]
    public float initYpos{get;private set;}
    [HideInInspector]
    public float targetYPostion{get;private set;}

    //该部分的显示已移至Debug面板
    [HideInInspector]
    public Vector2 curVelocity{get;private set;}
    [HideInInspector]
    public bool isJumping{get;private set;}
    [HideInInspector]
    public bool isAttack{get;private set;}
    [HideInInspector]
    public bool isHitted{get;private set;}
    [HideInInspector]
    public bool isSuper{get;private set;}

    [Space(5, order = 0)]
    [Header("碰撞检测", order = 1)]
    [Header("是否绘制碰撞线",order = 2)]
    public bool DrawRaycastsGizmos = true;
    [Header("水平方向的射线检测条数")]
    public int NumberOfHorizontalRays = 3;
    [Header("竖直方向的射线检测条数")]
    public int NumberOfVerticalRays = 3;
    [Header("射线检测的延伸长度")]
    [Tooltip("即：从Player的碰撞体延伸出去多长的检测范围")]
    public float checkDelta = 0.05f;
    [Header("4方向检测标志位")]
    public bool IsCollidingRight = false;
    public bool IsCollidingLeft = false;
    public bool IsCollidingCeiling = false;
    public bool IsCollidingGround = false;
    // 碰撞体边界
    protected float top;
    protected float bottom;
    protected float left;
    protected float right;
    protected LayerMask GroundLayerMask;
    #endregion
    [Header("显示配置")]
    public SpriteRenderer sp;
    public Sprite[] texs;
    //攻击动作持续时间
    public int attclk;
    //受击动作持续时间
    public int behitclk;

    //子对象
    protected GameObject weaponX;
    protected GameObject weaponY;
    protected ContactFilter2D filter;
    protected Collider2D[] result;

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
        top = colldr.offset.y + (colldr.size.y / 2f);
        bottom = colldr.offset.y - (colldr.size.y / 2f);
        //计算垂直方向射线的左右边界
        left = colldr.offset.x - (colldr.size.x / 2f);
        right = colldr.offset.x + (colldr.size.x / 2f);
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
            LayerMask ground = LayerMask.GetMask("Ground");
            LayerMask monster = LayerMask.GetMask("Monsters");
            //射线检测(射线原点，方向，长度（边界值+防撞delta值），检测图层)
            result[i] = Physics2D.Raycast(
                            rayOriginPoint, 
                            dir, 
                            checkDelta + Mathf.Abs(limit),
                            ground|monster);
            //如果当前检测有，则置位标志位
            if (result[i])
            {
                isCollided = true;
                //Debug.Log(result[i].collider.name);
            }
            
            //DebugTools
            if(DrawRaycastsGizmos)
            {
                //原点，方向，颜色
                Debug.DrawRay(rayOriginPoint, dir * (checkDelta + Mathf.Abs(limit)), rayColor);
            }
        }
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
            body.velocity = new Vector2(Mathf.SmoothDamp(body.velocity.x, moveSpeed * Time.fixedDeltaTime * 60, ref Xvelocity, XaccelerateTime), body.velocity.y);
        }
        else if (Input.GetAxisRaw("Horizontal") < (InputOffset.x * -1))
        {
            body.velocity = new Vector2(Mathf.SmoothDamp(body.velocity.x, moveSpeed * Time.fixedDeltaTime * 60 * -1, ref Xvelocity, XaccelerateTime), body.velocity.y);
        }
        else if (Input.GetAxisRaw("Horizontal") == 0)
        {
            body.velocity = new Vector2(Mathf.SmoothDamp(body.velocity.x, 0, ref Xvelocity, XaccelerateTime), body.velocity.y);
        }
    }

    //调整的重力加速度函数
    void Gravity()
    {
        //在原重力影响下，加入fallMulti作为乘数调整下落的加速度
        body.velocity += Vector2.up * Physics2D.gravity.y * fallMulti * Time.fixedDeltaTime;
    }

    //TODO：临时的动画切换
    void SwitchSprite()
    {
        if (!isAttack)
        {
            if (IsCollidingGround)
            {
                sp.sprite = texs[0];
            }
            if (isJumping && body.velocity.y > 0)//速度大于0且是跳跃状态才切换
            {
                sp.sprite = texs[1];
            }
            if (body.velocity.y < -0.2f)
            {
                sp.sprite = texs[2];
            }
        }
    }

    void Jump(float targetYPostion)
    {        
        //处于上升状态
        if (body.velocity.y >= 0 && transform.position.y < targetYPostion)
        {
            body.velocity = new Vector2(body.velocity.x, jumpInitSpeed);
        }
        //已达最大高度，变更为下落（此时无法再进入y速度需要大于0的上个if）
        else if (transform.position.y >= targetYPostion)
        {
            //达到最大高端，取消跳跃状态（相当于变为下坠）
            isJumping = false;
            body.velocity = new Vector2(body.velocity.x, jumpDownSpeed);
        }
    }

    private void BeHitted(float damage)
    {
        isHitted = true;
        HP -= damage;
    }

    private void Start()
    {
        //物理组件初始化
        body = GetComponent<Rigidbody2D>();
        sp = transform.GetChild(0).GetComponent<SpriteRenderer>();
        colldr = GetComponent<BoxCollider2D>();
        //计算碰撞边界
        CalculateColliderLimit();

        weaponX = transform.GetChild(1).gameObject;
        weaponX.SetActive(false);
        weaponY = transform.GetChild(2).gameObject;
        weaponY.SetActive(false);

        LayerMask mask = LayerMask.GetMask("Monsters");
        filter.layerMask = mask;
    }

    private void Update()
    {
        //每帧初始化
        InitCollideStatusByFrame();
        //获取实时碰撞标志位（射线检测(射线原点，方向，长度（边界值+防撞delta值），检测图层)）
        CheckDirCollide(-transform.right,left,ref IsCollidingLeft,Color.red);
        CheckDirCollide(-transform.up,bottom,ref IsCollidingGround,Color.blue);

        CheckDirCollide(transform.up,top,ref IsCollidingCeiling,Color.green);
        
        CheckDirCollide(transform.right,right,ref IsCollidingRight,Color.yellow);
        //对应状态的部分运动调整

#region  Jump Part
        //跳跃检测
        if(Input.GetKeyDown(KeyCode.C))
        {
            //跳跃初始化（记录起跳高度和置位标志）
            if(IsCollidingGround && !isJumping)
            {
                isJumping = true;
                initYpos = transform.position.y;
                targetYPostion = transform.position.y +MaxHeight;
            }
        }
        //跳跃中判定
        if(isJumping)
        {
            Jump(targetYPostion);
            if (IsCollidingCeiling)//如果跳跃过程中撞上天花板，则直接取消跳跃状态，并开始下落
            {
                //EditorApplication.isPaused = true;
                isJumping = false;
                body.velocity = new Vector2(body.velocity.x, jumpDownSpeed);
            }
        }
        else//如果没有跳跃，也不在地上，说明在下坠
        {
            if(!IsCollidingGround)
            {
                body.velocity = new Vector2(body.velocity.x, jumpDownSpeed);
            }
        }
#endregion

#region Attack Part
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
#endregion

#region BeHit Part
        if(behitclk > 0)
        {
            behitclk--;
        }
        else
        {
            isHitted = false;
        }
#endregion

    }

    private void FixedUpdate()
    {
        //Debug显示
        curVelocity = body.velocity;
        //恒运行函数
        RigCheckMove();
        //是否打开重力影响
        if (GravityEffect)
        {
            Gravity();
        }

        //显示切换测试
        SwitchSprite();

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monsters"))
        {
            Monster enemy = collision.gameObject.GetComponent<Monster>();
            if(enemy == null)
            {
                //子物体为Monster的攻击碰撞体（例如：Monster的剑，组合怪且脚本只存在于主物体）
                enemy = collision.transform.parent.GetComponent<Monster>();
            }

            if(enemy !=null)
            {
                HP -= Att;
                Debug.Log(collision.gameObject.name);
                isHitted = true;
                //collision.gameObject.SendMessage("ApplyDamage", 10);
            }
            else
            {
                Debug.LogWarning("当前碰撞体未找到Monster脚本，无法获取Monster的攻击数值");
            }
        }
    }



    private void OnDrawGizmos()
    {
        //天花板检测
        Gizmos.color = Color.blue;
    }
}
