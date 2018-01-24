using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
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

        public UnityEvent onUpdate;
        public UnityEvent onDeath;

        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody m_RigidBody;
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
            set {
                m_hp -= value;
                if(m_hp < 0)
                {
                    onDeath.Invoke();
                    Die();
                }
            }
        }

        public void Die()
        {
            //TODO
            Debug.Log("Rip Player");
        }


        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
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


        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_DoubleJumpReady = true;
            mouseLook.Init (transform, cam.transform);
            m_CircleBuffer = new CircleBuffer(advancedSettings.timeReverseSize);
        }


        private void Update()
        {
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

            RotateView();

            if (Input.GetButtonDown("Jump"))
            {
                if (!m_Jump)
                    m_Jump = true;
                if(m_Jumping && !m_IsGrounded && m_DoubleJumpReady && advancedSettings.IsDoubleJumpPossible)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_DoubleJumpReady = false;
                }
            }
            if (Input.GetButtonDown("Ability1"))
            {
                m_ChangeTimeScale = true;
            }

            onUpdate.Invoke();
        }


        private void FixedUpdate()
        {

            if (m_ReverseTime)
            {
                Vector3 pos;
                Quaternion rot;
                Quaternion camRot;
                if(m_CircleBuffer.Pop(out pos, out rot, out camRot))
                {
                    transform.position = pos;
                    transform.rotation = rot;
                    cam.transform.rotation = camRot;
                    m_RigidBody.Sleep();
                    return;
                }
            }

            if (m_ChangeTimeScale)
            {
                if (!m_SlowTime)
                {
                    m_SlowTime = true;
                    Time.timeScale = 0.2f;
                }
                else
                {
                    m_SlowTime = false;
                    Time.timeScale = 1f;
                }
                Time.fixedDeltaTime = 0.02F * Time.timeScale;

                m_ChangeTimeScale = false;
            }

            GroundCheck();
            Vector2 input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward*input.y + cam.transform.right*input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;
                if (m_IsGrounded)
                {
                    desiredMove.x = desiredMove.x * movementSettings.Speed;
                    desiredMove.z = desiredMove.z * movementSettings.Speed;
                    desiredMove.y = desiredMove.y * movementSettings.Speed;
                    if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.Speed * movementSettings.Speed))
                    {
                        m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                    }
                }
                else
                {
                    desiredMove.x = (m_AirDirection * m_Airspeed).x;
                    desiredMove.y = (m_AirDirection * m_Airspeed).y;
                    desiredMove.z = (m_AirDirection * m_Airspeed).x;
                    if (m_RigidBody.velocity.sqrMagnitude <
                    (m_Airspeed.magnitude * m_Airspeed.magnitude))
                    {
                        m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                    }
                }
            }

            if (m_IsGrounded)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Airspeed = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_AirDirection = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                    m_Jumping = true;
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                m_RigidBody.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;

            m_CircleBuffer.Push(transform.position, transform.rotation, cam.transform.rotation);
        }


        public void rotate(Vector2 vec2)
        {
            transform.localRotation *= Quaternion.Euler(0f, vec2.y, 0f);
            cam.transform.localRotation *= Quaternion.Euler(-vec2.x, 0f, 0f);
        }


        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * 1.0f, Vector3.down, out hitInfo, ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
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


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation (transform, cam.transform, m_PrevTimeReverse);

            if (m_IsGrounded)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
            }
            else if (advancedSettings.airControl)
            {
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    if (Quaternion.Angle(m_AirDirection, transform.rotation) < 45)
                    {
                        m_RigidBody.velocity = Quaternion.AngleAxis(Input.GetAxis("Horizontal") * advancedSettings.airRotationSpeed * Time.deltaTime, Vector3.up) * m_RigidBody.velocity;
                    }
                }
                else if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    if (Quaternion.Angle(transform.rotation, m_AirDirection) < 45)
                    {
                        m_RigidBody.velocity = Quaternion.AngleAxis(Input.GetAxis("Horizontal") * advancedSettings.airRotationSpeed * Time.deltaTime, Vector3.up) * m_RigidBody.velocity;
                    }
                }
                if (!m_PrevTimeReverse)
                    m_AirDirection = Quaternion.LookRotation(m_RigidBody.velocity);
                else
                    m_Airspeed = new Vector3(0, 0, 0);
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * 1.0f, Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
                m_DoubleJumpReady = true;
            }
        }

        public void Spawn()
        {
            transform.position = advancedSettings.spawnPoint.transform.position;
        }

        public void OnTriggerEnter(Collider obj)
        {
            if(obj.gameObject.tag.Equals("DeathZone"))
            {
                Spawn();
            }
        }
    }
}
