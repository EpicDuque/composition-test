using System;
using System.Linq;
using CoolTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    public abstract class Actor : MonoBehaviour
    {
        [Serializable]
        public class ActorEvents
        {
            [SerializeField] private UnityEvent<Actor> onActorCreated;
            [SerializeField] private UnityEvent<Actor> onActorDestroyed;
            [SerializeField] private UnityEvent onActorJump;
            
            public UnityEvent<Actor> OnActorCreated => onActorCreated;
            public UnityEvent<Actor> OnActorDestroyed => onActorDestroyed;
            public UnityEvent OnActorJump => onActorJump;
        }

        #region Serialized Fields
        
        [SerializeField] protected ActorData actorData;
        [Space(10f)]
        [SerializeField] protected float groundCheckRadius = 0.25f;
        [SerializeField] protected LayerMask whatIsGround;
        [ColorSpacer("Actor Events", 0.6f, 0.8f, 0.9f, 20f)]
        [SerializeField] protected ActorEvents actorEventCallbacks;

        [ColorSpacer("Editor Settings", 0.7f, 0.8f, 0.7f)]
        [SerializeField] protected Animator animator;
        #endregion
        
        protected CharacterController characterController;
        protected float speed;
        private float walkRunAnimBlend;
        private float targetRotationAngle;

        protected bool hasAnimator;
        private float fallTimeoutDelta;
        private float verticalVelocity;
        protected float jumpTimeoutDelta;
        private float originalMoveSpeed;
        private LayerMask castLayer;
        private float rotationVelocityY;
        private float rotationVelocityX;
        protected Vector2 movement;

        protected int animID_RunBlend;
        protected int animID_Jump;
        protected int animID_FreeFall;
        protected int animID_MotionSpeed;
        
        private RaycastHit[] obstructionRaycastResults;

        #region Public Properties

        public Animator Animator
        {
            get => animator;
            set => animator = value;
        }

        public CharacterController CharacterController => characterController;

        public ActorData ActorData
        {
            get => actorData;
            set => actorData = value;
        }
        /// <summary>
        /// Resultant scalar movement speed of this Actor.
        /// </summary>
        public float Speed => speed;

        public bool IsGrounded { get; protected set; }
        
        /// <summary>
        /// If set to true, will attempt a jump if the Actor is grounded.
        /// </summary>
        public bool Jump { get; set; }

        public Vector2 Movement
        {
            get => movement;
            set { movement = value; }
        }

        public Vector3 LookDirection { get; set; }
        public bool CanMove { get; set; }
        public bool CanRotate { get; set; }
        public float MovementMult { get; set; } = 1.0f;

        public bool HasAnimator
        {
            get => hasAnimator;
            set => hasAnimator = value;
        }
        public ActorEvents ActorEventCallbacks => actorEventCallbacks;

        #endregion

        protected void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        protected void Start()
        {
            animID_Jump = Animator.StringToHash(actorData.JumpParam);
            animID_FreeFall = Animator.StringToHash(actorData.FreeFallParam);
            animID_MotionSpeed = Animator.StringToHash(actorData.MotionSpeedParam);
            animID_RunBlend = Animator.StringToHash(actorData.RunBlendParam);
            
            LookDirection = transform.forward;

            hasAnimator = animator != null;
            
            // reset our timeouts on start
            jumpTimeoutDelta = actorData.JumpTimeout;
            fallTimeoutDelta = actorData.FallTimeout;

            CanMove = true;
            CanRotate = true;

            ActorEventCallbacks.OnActorCreated?.Invoke(this);

            characterController.SimpleMove(Vector3.zero);
        }

        protected void Update()
        {
            MoveStep();
            RotateStep();
            JumpGravityStep();
            
            IsGrounded = Physics.CheckSphere(transform.position, 0.35f, whatIsGround);
        }
        
        protected virtual float GetCurrentSpeed()
        {
            return new Vector3(
                    characterController.velocity.x,
                    0.0f,
                    characterController.velocity.z)
                .magnitude;
        }

        /// <summary>
        /// Execute a step in the Actor's movement. This is called in the Update method in Actor.
        /// </summary>
        protected virtual void MoveStep()
        {
            if (!CanMove)
            {
                // if(characterController != null)
                //     characterController.SimpleMove(Vector3.zero);
                
                Movement = Vector2.zero;
            }

            var targetSpeed = actorData.MoveSpeed;
            
            var currentSpeed = GetCurrentSpeed();

            const float speedOffset = 0.1f;

            // accelerate or decelerate to target speed
            if (currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentSpeed, targetSpeed * Movement.magnitude * MovementMult,
                    Time.deltaTime * actorData.MoveAccel);

                speed = Mathf.Round(speed * 1000f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }
            
            CharacterControllerMoveStep();
            
            UpdateAnimator(currentSpeed);
        }

        protected virtual void CharacterControllerMoveStep()
        {
            var m = new Vector3(Movement.x, 0f, Movement.y);

            if (characterController != null)
            {
                characterController.Move(m * (speed * Time.deltaTime) + 
                                         new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
            }
        }

        protected virtual void UpdateAnimator(float currentSpeed)
        {
            walkRunAnimBlend = Mathf.Lerp(walkRunAnimBlend, currentSpeed, Time.deltaTime * actorData.MoveAccel);
            // update animator if using character
            if (hasAnimator)
            {
                animator.SetFloat(animID_RunBlend, walkRunAnimBlend);

                animator.SetFloat(animID_MotionSpeed, 1f);
            }
        }

        /// <summary>
        /// Execute a rotation step in the Actor's movement. This is called in the Update method in Actor.
        /// </summary>
        protected virtual void RotateStep()
        {
            if (!CanRotate) return;

            var transformY = GetRotationTransformY();
            if (transformY != null)
            {
                var target = Mathf.Atan2(LookDirection.x, LookDirection.z) * Mathf.Rad2Deg;

                var rotationY = Mathf.SmoothDampAngle(transformY.rotation.eulerAngles.y, target,
                    ref rotationVelocityY, actorData.RotationSmoothTime);

                transformY.rotation = Quaternion.Euler(0.0f, rotationY, 0.0f);
            }

            var transformX = GetRotationTransformX();
            if (transformX != null)
            {
                var rotationX = Mathf.SmoothDampAngle(transformX.rotation.eulerAngles.x, LookDirection.x,
                    ref rotationVelocityX, actorData.RotationSmoothTime);

                transformX.rotation = Quaternion.Euler(rotationX, 0f, 0.0f);
            }
        }

        public void InstantLookDir(Vector3 dir)
        {
            var target = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            GetRotationTransformY().rotation = Quaternion.Euler(0.0f, target, 0.0f);
        }

        /// <summary>
        /// Returns the transform used for applying rotation on Y axis to this Actor.
        /// </summary>
        /// <returns></returns>
        public virtual Transform GetRotationTransformY() => transform;

        /// <summary>
        /// Returns the transform used for applying rotation on X axis to this Actor.
        /// </summary>
        /// <returns></returns>
        public virtual Transform GetRotationTransformX() => null;

        /// <summary>
        /// Execute jump and apply gravity for this Actor.
        /// </summary>
        protected virtual void JumpGravityStep()
        {
            if (IsGrounded)
            {
                // reset the fall timeout timer
                fallTimeoutDelta = actorData.FallTimeout;

                // update animator if using character
                if (hasAnimator)
                {
                    animator.SetBool(animID_Jump, false);
                    animator.SetBool(animID_FreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (verticalVelocity < 0.0f)
                {
                    verticalVelocity = -2f;
                }

                // Jump
                if (Jump && jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    verticalVelocity = Mathf.Sqrt(actorData.JumpHeight * -2f * actorData.Gravity);

                    // update animator if using character
                    if (hasAnimator)
                    {
                        animator.SetBool(animID_Jump, true);
                    }
                    
                    ActorEventCallbacks.OnActorJump?.Invoke();
                }

                // jump timeout
                if (jumpTimeoutDelta >= 0.0f)
                {
                    jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                jumpTimeoutDelta = actorData.JumpTimeout;

                // fall timeout
                if (fallTimeoutDelta >= 0.0f)
                {
                    fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (hasAnimator)
                    {
                        animator.SetBool(animID_FreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                Jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (verticalVelocity < actorData.TerminalVelocity)
            {
                verticalVelocity += actorData.Gravity * Time.deltaTime;
            }
        }

        /// <summary>
        /// Helper function to set the LookDirection vector to point to target.
        /// </summary>
        /// <param name="target"></param>
        public virtual void SetLookDirectionTarget(Transform target)
        {
            if (target == null) return;

            LookDirection = DirectionToPosition(target.position);
        }

        /// <summary>
        /// Returns true if there is a collider with specific tag between this Actor's position and another position.
        /// </summary>
        /// <param name="targetPos">Target position.</param>
        /// <param name="offset">Offset to apply to the origin position of the Actor.</param>
        /// <param name="colliderTag">Collider Tag to test against.</param>
        /// <param name="maxRaycastHits">Max amount of colliders the raycast can hit.</param>
        /// <returns></returns>
        public bool IsPositionNotObstructed(Vector3 targetPos, Vector3 offset = default, string colliderTag = "Default", int maxRaycastHits = 20)
        {
            var origin = transform.position + offset;

            var dir = targetPos - origin;

            obstructionRaycastResults = new RaycastHit[maxRaycastHits];
            var size = Physics.RaycastNonAlloc(origin, dir.normalized, obstructionRaycastResults, dir.magnitude);

            if (size == 0) return true;

            var solid = obstructionRaycastResults
                .Where(r => r.collider != null)
                .Any(r => r.collider.CompareTag(colliderTag));
            
            return !solid;
        }

        /// <summary>
        /// Helper function that returns the direction from this Actor's position to another position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected Vector3 DirectionToPosition(Vector3 pos)
        {
            var relativePos = new Vector3(pos.x, transform.position.y, pos.z);
            
            return (relativePos - transform.position).normalized;
        }

        public void DestroyActor()
        {
            ActorEventCallbacks.OnActorDestroyed?.Invoke(this);
            
            Destroy(gameObject);
        }
    }
}
