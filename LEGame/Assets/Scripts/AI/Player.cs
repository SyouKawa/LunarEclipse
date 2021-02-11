using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    
    #endregion

    public SpriteRenderer sp;
    public Sprite[] texs;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        sp = transform.GetChild(0).GetComponent<SpriteRenderer>();
        
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
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube((Vector2)transform.position + PointOffset,Size);
    }
}
