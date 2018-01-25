using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
public class NewPlayerMovement : MonoBehaviour
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
        public float groundCheckDistance = 0.01f;
        public GameObject spawnPoint;
        public int timeReverseSize = 1000;
        public float gravity = 10f;
    }

    [Serializable]
    public class HudSettings
    {
    
    }

    public Camera cam;
    public MovementSettings movementSettings = new MovementSettings();
    public MouseLook mouseLook = new MouseLook();
    public AdvancedSettings advancedSettings = new AdvancedSettings();


    private CharacterController charController;
    private CapsuleCollider m_Capsule;
    private Vector3 m_YVel;
    private CircleBuffer m_CircleBuffer;
    private bool m_DoubleJumpReady, m_TimeSlow, m_PrevRevTime;

    public int m_hp;

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
    void Start()
    {
        m_hp = 100;
        anim = GetComponentInChildren<Animation>();
        weapon = 1;
        pellets = 10;
        SetLMG();
        currentAccuracy = minimumAccuracy;
        heat = 0;
        mouseLook.Init(transform, cam.transform);
        charController = GetComponent<CharacterController>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_CircleBuffer = new CircleBuffer(1000);
    }

    //----------------------------------------------------------------
    int weapon;
    int pellets;
    int bulletDamage;           //Damage the bullet deals on impact
    float fireRate;               //How much delay is added after every shot
    float fireDelay;              //Current delay. Weapon only fires when delay = 0
    float minimumAccuracy;      //Minimum size of hipfire
    float maximumAccuracy;      //Maximum size of hipfire
    float currentAccuracy;      //Current size of hipfire
    float accuracy;             //How much spread is added to hipfire after every shot
    float heatbuildup;          //How much heat is added after every shot
    float heat;                 //Current amount of heat. if heat reaches 100, weapons are disabled until heat falls to atleast 30
    float coolingRate;      //How quick the gun cools. Currently UNUSED
    bool overheat;              //States if weapon is overheated or not
    float maxRange;      //Range after which raycast stops
    //----------------------------------------------------------------

    private Animation anim;

    // Update is called once per frame
    void Update()
    {
        GetComponent<HUDInterface>().UpdateHUD();

        if (Input.GetButton("Ability2"))
        {
            m_PrevRevTime = true;
            Vector3 pos;
            Quaternion rot;
            Quaternion camRot;
            if (m_CircleBuffer.Pop(out pos, out rot, out camRot))
            {
                transform.position = pos;
                transform.rotation = rot;
                cam.transform.rotation = camRot;
            }
            return;
        }
        else if (m_PrevRevTime)
        {
            m_PrevRevTime = false;
            mouseLook.Init(transform, cam.transform);
        }

        if (Input.GetButtonDown("Ability1"))
        {
            if (m_TimeSlow)
            {
                m_TimeSlow = false;
                Time.timeScale = 1;
            }
            else
            {
                m_TimeSlow = true;
                Time.timeScale = 0.1f;
            }
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
        

        mouseLook.LookRotation(transform, cam.transform, false);

        Vector2 input = GetInput();

        charController.Move(transform.rotation * new Vector3(input.x, 0, input.y) * Time.deltaTime * movementSettings.Speed);
        charController.Move(m_YVel * Time.deltaTime);
        
        if (CheckGround())
        {
            m_DoubleJumpReady = true;
            if (m_YVel.y < 0)
            {
                m_YVel = new Vector3(0, 0, 0);
            }
            if (Input.GetButtonDown("Jump"))
            {
                m_YVel = new Vector3(0, movementSettings.JumpForce, 0);
            }
        }
        else
        {
            m_YVel += -Vector3.up * Time.deltaTime * 0.1f * advancedSettings.gravity;

            if (Input.GetButtonDown("Jump") && m_DoubleJumpReady)
            {
                m_DoubleJumpReady = false;
                m_YVel = new Vector3(0, movementSettings.JumpForce, 0);
            }
        }

        m_CircleBuffer.Push(transform.position, transform.rotation, cam.transform.rotation);

        if (Input.GetButton("Fire1") && overheat == false && fireDelay <= 0)
        {
            Debug.Log("Weaponmode: " + weapon + "Pelletnumber: " + pellets);
            anim.Play("Shoot");
            heat += heatbuildup;
            fireDelay = fireRate;
            fireWeapon();
            if (heat >= 100)
            {
                overheat = true;
            }
            pellets++;
            getNewAccuracy();
        }
        else if (Input.GetButton("Fire1") == false)
        {
            if (pellets > 10)
            {
                pellets--;
            }
            currentAccuracy -= accuracy;
            if (currentAccuracy < minimumAccuracy)
            {
                currentAccuracy = minimumAccuracy;
            }
            if (heat > 0)
            {
                heat -= coolingRate;
                if (heat < 0)
                {
                    heat = 0;
                }
            }
        }
        if (overheat && heat <= 30)
            overheat = false;
        if (fireDelay > 0)
        {
            fireDelay -= Time.deltaTime;
        }
        Debug.Log("Current HEAT-LeveL: " + heat);
        if (Input.GetButtonDown("Fire2"))
        {
            weapon--;
            if (weapon == 0)
            {
                weapon = 3;
            }
            changeWeapon();
        }
        else if (Input.GetButtonDown("Fire3"))
        {
            weapon++;
            if (weapon == 4)
            {
                weapon = 1;
            }
            changeWeapon();
        }
    }

    void fireWeapon()
    {
        if (weapon == 1 || weapon == 2)
        {
            fireBullet();
        }
        else if (weapon == 3)
        {
            for (int i = 0; i < pellets; i++)
            {
                fireBullet();
            }
        }
    }

    void fireBullet()
    {
        Vector3 direction = cam.transform.rotation * Vector3.forward;
        Debug.DrawRay(transform.position, direction * maxRange, Color.red);
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, direction, out hitInfo, maxRange))
        {
            if (hitInfo.collider.gameObject.GetComponent<Enemy>() != null)
            {
                hitInfo.collider.gameObject.GetComponent<Enemy>().Damage = bulletDamage;
                Debug.Log("Actually hit something");
            }
        }
    }

    /// <summary>
    /// Changes the size of the "hip fire"
    /// </summary>
    void getNewAccuracy()
    {
        currentAccuracy += accuracy;
        if (currentAccuracy > maximumAccuracy)
        {
            currentAccuracy = maximumAccuracy;
        }
    }

    void changeWeapon()
    {
        if (weapon == 1)
        {
            SetLMG();
        }
        else if (weapon == 2)
        {
            SetSniper();
        }
        else if (weapon == 3)
        {
            SetShotgun();
        }
    }

    /// <summary>
    /// sets weapon mode to "LMG" config
    /// </summary>
    public void SetLMG()
    {
        bulletDamage = 30;
        fireRate = 0.5f;
        minimumAccuracy = 0.05f;
        maximumAccuracy = 0.20f;
        currentAccuracy = maximumAccuracy;
        accuracy = 0.01f;
        heatbuildup = 2;
        coolingRate = 1;
        maxRange = 99999;
    }

    /// <summary>
    /// sets Weapon mode to "Sniper" config
    /// </summary>
    public void SetSniper()
    {
        bulletDamage = 100;
        fireRate = 2f;
        minimumAccuracy = maximumAccuracy = 0;
        currentAccuracy = 0;
        accuracy = 0f;
        heatbuildup = 25;
        coolingRate = 1;
        maxRange = 99999;
    }

    /// <summary>
    /// sets Weapon mode to "Shotgun" config
    /// </summary>
    public void SetShotgun()
    {
        bulletDamage = 15;
        fireRate = 1f;
        minimumAccuracy = maximumAccuracy = currentAccuracy = 0.4f;
        accuracy = 0f;
        heatbuildup = 10;
        coolingRate = 5;
        maxRange = 25;
    }

    public bool CheckGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, m_Capsule.height / 2 + 0.1f))
        {
            return true;
        }
        return false;
    }
}