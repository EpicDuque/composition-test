using CoolTools.Attributes;
using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(menuName = "Actor/Actor Data", fileName = "New Actor Data")]
    public class ActorData : ScriptableObject
    {
        [ColorSpacer("Basic Actor Settings", 0.4f, 0.6f, 0.9f, 5f)] 
        [Header("Movement")] 
        [SerializeField] protected float moveSpeed;
        [SerializeField] protected float moveAccel;

        [Space(10f)]
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        [SerializeField] protected float rotationSmoothTime = 0.12f;

        [Tooltip("The height the player can jump")] 
        [SerializeField] protected float jumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")] 
        [SerializeField] protected float gravity = -15.0f;

        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        [SerializeField] protected float jumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        [SerializeField] protected float fallTimeout = 0.15f;

        [Tooltip("Max downward velocity of this Actor when free falling.")] 
        [SerializeField] private float terminalVelocity = 53.0f;

        [Header("Animator Settings")]
        [Tooltip("Animator parameter used for blending between idle and run animations")]
        [SerializeField] private string runBlendParam = "RunBlend";
        [SerializeField] private string jumpParam = "Jump";

        [Tooltip("Animation parameter used for then the Actor is free falling")] 
        [SerializeField] private string freeFallParam = "FreeFall";

        [Tooltip("Animation parameter used for changing the run animation speed depending on the speed of the Actor")]
        [SerializeField] private string motionSpeedParam = "MotionSpeed";

        
        public string MotionSpeedParam => motionSpeedParam;

        public string FreeFallParam => freeFallParam;

        public string JumpParam => jumpParam;

        public string RunBlendParam => runBlendParam;

        public float TerminalVelocity => terminalVelocity;

        public float FallTimeout => fallTimeout;

        public float JumpTimeout => jumpTimeout;

        public float Gravity => gravity;

        public float JumpHeight => jumpHeight;

        public float RotationSmoothTime => rotationSmoothTime;

        public float MoveAccel => moveAccel;

        public float MoveSpeed => moveSpeed;
    }
}