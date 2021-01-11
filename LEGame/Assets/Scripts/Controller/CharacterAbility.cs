using UnityEngine;
using System.Collections;
using MoreMountains.Tools;


namespace KMGame
{
    /// <summary>
    /// A class meant to be overridden that handles a character's ability. 
    /// </summary>
    public class CharacterAbility : MonoBehaviour
    {
        /// if true, this ability can perform as usual, if not, it'll be ignored. You can use this to unlock abilities over time for example
        [Header("Permissions")]
        public bool AbilityPermitted = true;
        /// true if the ability has already been initialized
		public bool AbilityInitialized { get { return _abilityInitialized; } }

        protected Character _character;
        protected CorgiController _controller;
        protected InputManager _inputManager;
        protected CameraController _sceneCamera;
        protected Animator _animator;

        protected SpriteRenderer _spriteRenderer;
        protected bool _abilityInitialized = false;
        // protected CharacterGravity _characterGravity;
        protected float _verticalInput;
        protected float _horizontalInput;
        protected bool _startFeedbackIsPlaying = false;

        /// This method is only used to display a helpbox text at the beginning of the ability's inspector
        public virtual string HelpBoxText() { return ""; }

        /// <summary>
        /// On Start(), we call the ability's intialization
        /// </summary>
        protected virtual void Start()
        {
            Initialization();
        }

        /// <summary>
        /// Gets and stores components for further use
        /// </summary>
        protected virtual void Initialization()
        {
            _character = GetComponent<Character>();
            _controller = GetComponent<CorgiController>();

            //  _characterGravity = GetComponent<CharacterGravity>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            BindAnimator();
            _abilityInitialized = true;
        }

        /// <summary>
        /// Sets a new input manager for this ability to get input from
        /// </summary>
        /// <param name="inputManager"></param>
        public virtual void SetInputManager(InputManager inputManager)
        {
            _inputManager = inputManager;
        }

        /// <summary>
        /// Binds the animator from the character and initializes the animator parameters
        /// </summary>
        protected virtual void BindAnimator()
        {
            // _animator = _character._animator;
            if (_animator != null)
            {
                InitializeAnimatorParameters();
            }
        }

        /// <summary>
        /// Adds required animator parameters to the animator parameters list if they exist
        /// </summary>
        protected virtual void InitializeAnimatorParameters()
        {

        }

        /// <summary>
        /// Internal method to check if an input manager is present or not
        /// </summary>
        protected virtual void InternalHandleInput()
        {
            if (_inputManager == null) { return; }

            _verticalInput = _inputManager.PrimaryMovement.y;
            _horizontalInput = _inputManager.PrimaryMovement.x;

            HandleInput();
        }

        /// <summary>
        /// Called at the very start of the ability's cycle, and intended to be overridden, looks for input and calls methods if conditions are met
        /// </summary>
        protected virtual void HandleInput()
        {

        }

        /// <summary>
        /// Resets all input for this ability. Can be overridden for ability specific directives
        /// </summary>
        public virtual void ResetInput()
        {
            _horizontalInput = 0f;
            _verticalInput = 0f;
        }

        /// <summary>
        /// The first of the 3 passes you can have in your ability. Think of it as EarlyUpdate() if it existed
        /// </summary>
        public virtual void EarlyProcessAbility()
        {
            InternalHandleInput();
        }

        /// <summary>
        /// The second of the 3 passes you can have in your ability. Think of it as Update()
        /// </summary>
        public virtual void ProcessAbility()
        {

        }

        /// <summary>
        /// The last of the 3 passes you can have in your ability. Think of it as LateUpdate()
        /// </summary>
        public virtual void LateProcessAbility()
        {

        }

        /// <summary>
        /// Override this to send parameters to the character's animator. This is called once per cycle, by the Character class, after Early, normal and Late process().
        /// </summary>
        public virtual void UpdateAnimator()
        {

        }

        /// <summary>
        /// Changes the status of the ability's permission
        /// </summary>
        /// <param name="abilityPermitted">If set to <c>true</c> ability permitted.</param>
        public virtual void PermitAbility(bool abilityPermitted)
        {
            AbilityPermitted = abilityPermitted;
        }

        /// <summary>
        /// Override this to specify what should happen in this ability when the character flips
        /// </summary>
        public virtual void Flip()
        {

        }

        /// <summary>
        /// Override this to reset this ability's parameters. It'll be automatically called when the character gets killed, in anticipation for its respawn.
        /// </summary>
        public virtual void ResetAbility()
        {

        }



        /// <summary>
        /// Override this to describe what should happen to this ability when the character respawns
        /// </summary>
        protected virtual void OnRespawn()
        {
        }


        /// <summary>
        /// Override this to describe what should happen to this ability when the character takes a hit
        /// </summary>
        protected virtual void OnHit()
        {

        }


    }
}