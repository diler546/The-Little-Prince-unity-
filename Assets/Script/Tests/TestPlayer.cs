using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    //[Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;           // Amount of maxSpeed applied to crouching movement. 1 = 100%
    //[Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;   // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }


    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                //move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            //m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHelth(int helth)
    {
        slider.maxValue = helth;
        slider.value = helth;
    }
    public void SetHelth(int helth)
    {
        slider.value = helth;
    }
}

public class Boar : MonoBehaviour
{
    public int maxHelth = 100;
    public int currentHelth;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        currentHelth = maxHelth;
        // navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void TakeDamage(int damage)
    {
        currentHelth -= damage;

        animator.SetTrigger("Hurt");

        if (currentHelth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Один есть");
        animator.SetBool("IsDead", true);
        GetComponent<CircleCollider2D>().enabled = false;
        this.enabled = false;

    }
    // Update is called once per frame
    void Update()
    {
        // navMeshAgent.SetDestination(playerTransform.position);
    }
}

public class Player : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;
    public float horizontalMove = 0f;
    bool jump = false;

    public int maxHelth = 100;
    public int currentHelth = 0;
    public HealthBar healthBar;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    public int attackDamage = 10;
   public void Start()
    {
        currentHelth = maxHelth;
        healthBar.SetMaxHelth(maxHelth);
    }
    // Update is called once per frame
    void Update()
    {
        if (currentHelth <= 0)
        {
            return;
        }
        else
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
            }
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Attack();
            }
        }

    }

    private bool isColliding = false;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!isColliding)
        {
            isColliding = true;

            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Player has collided with an enemy!");
                TakeDamage(20);
                StartCoroutine(ResetCollision());
            }
        }

    }
    private IEnumerator ResetCollision()
    {
        yield return new WaitForSeconds(1);
        isColliding = false;
    }
    public void Attack()
    {
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Boar>().TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    public void TakeDamage(int damage)
    {
        currentHelth -= damage;
        ;

        if (currentHelth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        animator.SetBool("IsDead", true);
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }
    public void FixedUpdate()
    {
        if (currentHelth <= 0)
        {
            return;
        }
        else
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
            jump = false;
        }

    }
}
public class TestPlayer
{
    [Test]
    public void Start_InitializesHealthAndMaxHealth()
    {
        // Arrange
        var player = new GameObject().AddComponent<Player>();
        player.maxHelth = 100;

        // Act
        player.Start();

        // Assert
        Assert.AreEqual(player.maxHelth, player.currentHelth);
    }

    [Test]
    public void TestPlayerSimplePasses()
    {
        var player = new GameObject().AddComponent<Player>();
        player.currentHelth = 100;

        // Act
        player.TakeDamage(20);

        // Assert
        Assert.AreEqual(80, player.currentHelth);
    }

    [Test]
    public void Die_SetsIsDeadToTrue()
    {
        // Arrange
        var player = new GameObject().AddComponent<Player>();

        // Act
        player.Die();

        // Assert
        Assert.AreEqual(true, player.animator.GetBool("IsDead"));
    }

    [Test]
    public void Attack_AppliesDamageToEnemy()
    {
        // Arrange
        var player = new GameObject().AddComponent<Player>();
        var enemy = new GameObject().AddComponent<Boar>();
        enemy.transform.position = Vector3.zero; // Позиция врага близко к игроку

        // Act
        player.Attack();

        // Assert
        Assert.AreEqual(player.attackDamage, enemy.currentHelth);
    }

    //[Test]
    //public void OnTriggerEnter2D_ReducesHealthWhenCollidingWithEnemy()
    //{
    //    // Arrange
    //    var player = new GameObject().AddComponent<Player>();
    //    player.currentHelth = 100;
    //    var enemy = new GameObject().AddComponent<Boar>();
    //    enemy.tag = "Enemy";

    //    // Act
    //    player.OnTriggerEnter2D(new Collider2D { gameObject = enemy });

    //    // Assert
    //    Assert.AreEqual(80, player.currentHelth);
    //}

    //[Test]
    //public void FixedUpdate_MovesPlayerWhenHorizontalMoveChanges()
    //{
    //    // Arrange
    //    var player = new GameObject().AddComponent<Player>();
    //    player.controller = new CharacterController2D(); // Создайте поддельный CharacterController2D
    //    player.horizontalMove = 10f;

    //    // Act
    //    player.FixedUpdate();

    //    // Assert
    //    Assert.AreEqual(10f * Time.fixedDeltaTime, player.controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
    //}
}
