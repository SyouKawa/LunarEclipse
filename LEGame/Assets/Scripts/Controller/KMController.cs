using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace KMGame
{
    public class KMController : MonoBehaviour
    {
        public LayerMask PlatformMask = 0;

        #region 方向撞墙检测
        public bool IsCollidingRight = false;
        public bool IsCollidingLeft = false;
        public bool IsCollidingAbove = false;
        public bool IsCollidingBelow = false;
        public bool HasCollisions { get { return IsCollidingRight || IsCollidingLeft || IsCollidingAbove || IsCollidingBelow; } }

        public enum RaycastDirections { up, down, left, right };

        protected int NumberOfHorizontalRays = 4;
        protected int NumberOfVerticalRays = 4;
        protected const float _smallValue = 0.0001f;
        protected const float _obstacleHeightTolerance = 0.05f;
        #endregion

        // 是否绘制碰撞线
        public bool DrawRaycastsGizmos = true;
        // 碰撞体边界
        public float top;
        public float bottom;
        public float left;
        public float right;
        protected BoxCollider2D _boxCollider;
        public Rigidbody2D _rigidbody;
        void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _rigidbody = GetComponent<Rigidbody2D>();
            top = _boxCollider.offset.y + (_boxCollider.size.y / 2f);
            bottom = _boxCollider.offset.y - (_boxCollider.size.y / 2f);
            left = _boxCollider.offset.x - (_boxCollider.size.x / 2f);
            right = _boxCollider.offset.x + (_boxCollider.size.x / 2f);

        }
        void Update()
        {

            IsCollidingRight = false;
            IsCollidingLeft = false;
            IsCollidingAbove = false;
            IsCollidingBelow = false;
            //向左发射线,检测左侧是否撞墙了
            RaycastHit2D[] storageArray = new RaycastHit2D[4];
            for (int i = 0; i < NumberOfHorizontalRays; i++)
            {
                Vector2 rayOriginPoint = Vector2.Lerp(transform.position + new Vector3(0, bottom, 0), transform.position + new Vector3(0, top, 0), (float)i / (float)(NumberOfHorizontalRays - 1));
                if (DrawRaycastsGizmos)
                {
                    Debug.DrawRay(rayOriginPoint, -transform.right * (_obstacleHeightTolerance + Mathf.Abs(left)), MMColors.Red);
                }
                storageArray[i] = Physics2D.Raycast(rayOriginPoint, -transform.right, _obstacleHeightTolerance + Mathf.Abs(left), PlatformMask);
                if (storageArray[i])
                {
                    IsCollidingLeft = true;
                }
            }


        }

        public float speed = 10f;

        void FixedUpdate()
        {
            _rigidbody.MovePosition(MMMaths.Vector3ToVector2(transform.position) + InputManager.Instance.PrimaryMovement * speed * Time.deltaTime);
        }
    }
}