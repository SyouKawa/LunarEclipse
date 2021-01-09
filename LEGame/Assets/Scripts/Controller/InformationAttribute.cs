using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using MoreMountains.Tools;

namespace Corgi
{


#if UNITY_EDITOR
    using UnityEditor;
#endif

    public enum InformationType { Error, Info, None, Warning }

    public class InformationAttribute : PropertyAttribute
    {


#if UNITY_EDITOR
        public string Message;
        public MessageType Type;
        public bool MessageAfterProperty;

        public InformationAttribute(string message, InformationType type, bool messageAfterProperty)
        {
            this.Message = message;
            if (type == InformationType.Error) { this.Type = UnityEditor.MessageType.Error; }
            if (type == InformationType.Info) { this.Type = UnityEditor.MessageType.Info; }
            if (type == InformationType.Warning) { this.Type = UnityEditor.MessageType.Warning; }
            if (type == InformationType.None) { this.Type = UnityEditor.MessageType.None; }
            this.MessageAfterProperty = messageAfterProperty;
        }
#else
		public InformationAttribute(string message, InformationType type, bool messageAfterProperty)
		{

		}
#endif
    }

    /// <summary>
    /// The various states you can use to check if your character is doing something at the current frame
    /// </summary>
    public class CorgiControllerState
    {
        /// is the character colliding right ?
        public bool IsCollidingRight { get; set; }
        /// is the character colliding left ?
        public bool IsCollidingLeft { get; set; }
        /// is the character colliding with something above it ?
        public bool IsCollidingAbove { get; set; }
        /// is the character colliding with something above it ?
        public bool IsCollidingBelow { get; set; }
        /// is the character colliding with anything ?
        public bool HasCollisions { get { return IsCollidingRight || IsCollidingLeft || IsCollidingAbove || IsCollidingBelow; } }

        /// returns the distance to the left collider, equals -1 if not colliding left
        public float DistanceToLeftCollider;
        /// returns the distance to the right collider, equals -1 if not colliding right
        public float DistanceToRightCollider;

        /// returns the slope angle met horizontally
        public float LateralSlopeAngle { get; set; }
        /// returns the slope the character is moving on angle
        public float BelowSlopeAngle { get; set; }
        /// returns true if the slope angle is ok to walk on
        public bool SlopeAngleOK { get; set; }
        /// returns true if the character is standing on a moving platform
        public bool OnAMovingPlatform { get; set; }

        /// Is the character grounded ? 
        public bool IsGrounded { get { return IsCollidingBelow; } }
        /// is the character falling right now ?
        public bool IsFalling { get; set; }
        /// is the character falling right now ?
        public bool IsJumping { get; set; }
        /// was the character grounded last frame ?
        public bool WasGroundedLastFrame { get; set; }
        /// was the character grounded last frame ?
        public bool WasTouchingTheCeilingLastFrame { get; set; }
        /// did the character just become grounded ?
        public bool JustGotGrounded { get; set; }
        /// is the character being resized to fit in tight spaces?
        public bool ColliderResized { get; set; }
        /// is the character touching level bounds?
        public bool TouchingLevelBounds { get; set; }

        /// <summary>
        /// Reset all collision states to false
        /// </summary>
        public virtual void Reset()
        {
            IsCollidingLeft = false;
            IsCollidingRight = false;
            IsCollidingAbove = false;
            DistanceToLeftCollider = -1;
            DistanceToRightCollider = -1;
            SlopeAngleOK = false;
            JustGotGrounded = false;
            IsFalling = true;
            LateralSlopeAngle = 0;
        }

        /// <summary>
        /// Serializes the collision states
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current collision states.</returns>
        public override string ToString()
        {
            return string.Format("(controller: collidingRight:{0} collidingLeft:{1} collidingAbove:{2} collidingBelow:{3} lateralSlopeAngle:{4} belowSlopeAngle:{5} isGrounded: {6}",
            IsCollidingRight,
            IsCollidingLeft,
            IsCollidingAbove,
            IsCollidingBelow,
            LateralSlopeAngle,
            BelowSlopeAngle,
            IsGrounded);
        }
    }

    /// <summary>
    /// Parameters for the Corgi Controller class.
    /// This is where you define your slope limit, gravity, and speed dampening factors
    /// </summary>

    [Serializable]
    public class CorgiControllerParameters
    {
        [Header("Gravity")]
        /// Gravity
        public float Gravity = -30f;
        /// a multiplier applied to the character's gravity when going down
        public float FallMultiplier = 1f;
        /// a multiplier applied to the character's gravity when going up
        public float AscentMultiplier = 1f;

        [Header("Speed")]
        /// Maximum velocity for your character, to prevent it from moving too fast on a slope for example
        public Vector2 MaxVelocity = new Vector2(100f, 100f);
        /// Speed factor on the ground
        public float SpeedAccelerationOnGround = 20f;
        /// Speed factor in the air
        public float SpeedAccelerationInAir = 5f;
        /// general speed factor
        public float SpeedFactor = 1f;

        [Header("Slopes")]
        /// Maximum angle (in degrees) the character can walk on
        [Range(0, 90)]
        public float MaximumSlopeAngle = 30f;
        /// the speed multiplier to apply when walking on a slope
        public AnimationCurve SlopeAngleSpeedFactor = new AnimationCurve(new Keyframe(-90f, 1f), new Keyframe(0f, 1f), new Keyframe(90f, 1f));

        [Header("Physics2D Interaction [Experimental]")]
        /// if set to true, the character will transfer its force to all the rigidbodies it collides with horizontally
        public bool Physics2DInteraction = true;
        /// the force applied to the objects the character encounters
        public float Physics2DPushForce = 2.0f;

        [Header("Gizmos")]
        /// if set to true, will draw the various raycasts used by the CorgiController to detect collisions in scene view if gizmos are active
        public bool DrawRaycastsGizmos = true;
        /// if this is true, warnings will be displayed if settings are not done properly
        public bool DisplayWarnings = true;
    }
}