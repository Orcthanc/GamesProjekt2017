using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
public class NewPlayerMovement : MonoBehaviour {

    [Serializable]
    public class MovementSettings
    {
        public float StandardSpeed = 2f;
        [HideInInspector]
        public float Speed = 2f;
        public float RunMultiplier = 1.5f;   // Speed when sprinting
        public KeyCode RunKey = KeyCode.LeftShift;
        public float JumpForce = 7f;
        public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));

        private bool m_Running;

        public void UpdateDesiredTargetSpeed(Vector2 input)
        {
            Speed = StandardSpeed;

            if (Input.GetKey(RunKey))
            {
                Speed *= RunMultiplier;
                m_Running = true;
            }
            else
            {
                m_Running = false;
            }

        }

        public bool Running
        {
            get { return m_Running; }
        }
    }


    [Serializable]
    public class AdvancedSettings
    {
        public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        public float stickToGroundHelperDistance = 0.5f; // stops the character
        public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
        public float airRotationSpeed = 100;
        public bool airControl; // can the user control the direction that is being moved in the air
        public bool IsDoubleJumpPossible = true;
        public GameObject spawnPoint;
        public int timeReverseSize = 200;
    }

    public Camera cam;
    public MovementSettings movementSettings = new MovementSettings();
    public MouseLook mouseLook = new MouseLook();
    public AdvancedSettings advancedSettings = new AdvancedSettings();


    private CharacterController charController;
    private CapsuleCollider m_Capsule;
    private float m_YRotation;
    private Vector3 m_GroundContactNormal, m_Airspeed;
    private Quaternion m_AirDirection;
    private CircleBuffer m_CircleBuffer;
    private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded, m_DoubleJumpReady, m_SlowTime, m_ChangeTimeScale, m_ReverseTime, m_PrevTimeReverse;

    private int m_hp;

    /// <summary>
    /// Gets remaining hp.
    /// The setter is used to apply damage
    /// </summary>
    public int Damage
    {
        get { return m_hp; }
        set
        {
            m_hp -= value;
            if (m_hp < 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        //TODO
        Debug.Log("Rip Player");
    }


    public bool Grounded
    {
        get { return m_IsGrounded; }
    }

    public bool Jumping
    {
        get { return m_Jumping; }
    }

    public bool Running
    {
        get
        {
            return movementSettings.Running;
        }
    }

    private Vector2 GetInput()
    {

        Vector2 input = new Vector2
        {
            x = Input.GetAxis("Horizontal"),
            y = Input.GetAxis("Vertical")
        };
        movementSettings.UpdateDesiredTargetSpeed(input);
        return input;
    }

    // Use this for initialization
    void Start () {
        mouseLook.Init(transform, cam.transform);
        charController = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButton("Ability2"))
        {
            m_ReverseTime = true;
            m_PrevTimeReverse = true;
            return;
        }
        else
        {
            if (!m_ReverseTime)
            {
                m_PrevTimeReverse = false;
            }
            m_ReverseTime = false;
        }

        Debug.Log("test");

        mouseLook.LookRotation(transform, cam.transform, false);

        Vector2 input = GetInput();

        charController.Move(transform.rotation * new Vector3(input.x, input.y));

    }
}
