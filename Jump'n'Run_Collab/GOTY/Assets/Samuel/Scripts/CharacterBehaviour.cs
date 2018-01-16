using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SphereCollider))]
public class CharacterBehaviour : MonoBehaviour {


    public MovementSettings movementSettings;
    public AdvancedSettings advancedSettings;

    private bool m_PreviouslyGrounded;
    private bool m_IsGrounded;
    private bool m_DoubleJumpReady;
    private bool m_Jumping;

    private float distToGround;
    private float yVel;

    private Vector3 m_GroundContactNormal;
    
    private CharacterController charController;
    private SphereCollider m_Sphere;

    [Serializable]
    public class MovementSettings
    {
        [SerializeField]
        private float speed;
        public float sprintMultiplier;
        public float jumpForce;
        public KeyCode sprintKey;

        public float Speed {
            get
            {
                return Input.GetKeyDown(sprintKey) ? (speed * sprintMultiplier) : speed;
            }
            set
            {
                speed = value;
            }
        }

        public void init()
        {
            Speed = speed;
        }

        public void UpdateLocalMovement(ref Vector2 vec)
        {
            vec.Normalize();
            vec.Set(vec.x * Speed * Time.deltaTime * 30, vec.y * Speed * Time.deltaTime * 30);
        }

    }

    [Serializable]
    public class AdvancedSettings
    {
        public float mouseSensitivity;
        public float gravity;
        public float groundCheckDistance;
        public bool doubleJump;
        public bool canSlowDownTime;
    }

	// Use this for initialization
	void Awake () {
        charController = gameObject.GetComponent<CharacterController>();
        movementSettings.init();
        m_Sphere = gameObject.GetComponent<SphereCollider>();
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 xzMotion = GetInput();

        Vector2 mouseMotion = GetMouseMotion();
 
        transform.Rotate(new Vector3(-mouseMotion.y, 0));
        transform.Rotate(new Vector3(0, mouseMotion.x), Space.World);

        GroundCheck();

        if (m_IsGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                yVel = movementSettings.jumpForce;
                m_Jumping = true;
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") && m_DoubleJumpReady)
            {
                m_DoubleJumpReady = false;
                yVel = movementSettings.jumpForce;
            }
            yVel -= advancedSettings.gravity * Time.deltaTime;
        }

        Debug.Log(m_IsGrounded);

        //Debug.Log(new Vector3(xzMotion.x * 30 * Time.deltaTime, yVel, xzMotion.y * 30 * Time.deltaTime));

        Vector3 dir = new Vector3(xzMotion.x * 30 * Time.deltaTime, yVel, xzMotion.y * 30 * Time.deltaTime);

        charController.Move(Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * dir);

    }

    private Vector2 GetInput()
    {

        Vector2 input = new Vector2
        {
            x = Input.GetAxisRaw("Horizontal"),
            y = Input.GetAxisRaw("Vertical")
        };
        movementSettings.UpdateLocalMovement(ref input);

        //Debug.Log(input);

        return input;
    }

    private Vector2 GetMouseMotion()
    {
        Vector2 input = new Vector2
        {
            x = Input.GetAxisRaw("Mouse X"),
            y = Input.GetAxisRaw("Mouse Y")
        };

        input.Normalize();
        input *= advancedSettings.mouseSensitivity;

        return input;
    }

    private void GroundCheck()
    {
        m_PreviouslyGrounded = m_IsGrounded;
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, m_Sphere.radius * 1.0f, Vector3.down, out hitInfo,
                               ((m_Sphere.radius / 2f) - m_Sphere.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore) && yVel <= 0)
        {
            Debug.Log("test");
            m_Jumping = false;
            yVel = 0;
            m_IsGrounded = true;
            m_DoubleJumpReady = true;
            m_GroundContactNormal = hitInfo.normal;

            charController.SimpleMove(new Vector3(0, y: hitInfo.distance - ((m_Sphere.radius / 2f) - m_Sphere.radius) + advancedSettings.groundCheckDistance));

            //transform.position.Set(transform.position.x, transform.position.y + hitInfo.distance - ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, transform.position.z);
        }
        else
        {
            m_IsGrounded = false;
            m_GroundContactNormal = Vector3.up;
        }
        //
        //Debug.Log(m_PreviouslyGrounded + " " + m_IsGrounded + " " + m_Jumping);
        //
        //if (m_PreviouslyGrounded && !m_IsGrounded && !m_Jumping)
        //    StickToGround();

    }

    //private void StickToGround()
    //{
    //
    //    //        RaycastHit hitInfo;
    //    //        if(Physics.Raycast(transform.position, Vector3.down, out hitInfo, 5f))
    //    //        {
    //    //
    //    //            Debug.Log(hitInfo.distance - ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance);
    //    //
    //    //            float temp = hitInfo.distance - ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance;
    //    //
    //    //            charController.SimpleMove(new Vector3(0, -temp + 0.1f));
    //    //
    //    //        }
    //
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, -Vector3.up, out hit, advancedSettings.groundCheckDistance + transform.GetComponent<Collider>().bounds.extents.y))
    //    {
    //        Debug.Log("Test");
    //        var distanceToGround = hit.distance;
    //        //use below code if your pivot point is in the middle
    //        transform.position.Set(transform.position.x, newY: hit.distance - transform.GetComponent<Collider>().bounds.extents.y, newZ: transform.position.z);
    //        //transform.position.y = hit.distance - transform.GetComponent<Collider>().bounds.extents;
    //
    //        m_IsGrounded = true;
    //    }
    //
    //}
    //
}
