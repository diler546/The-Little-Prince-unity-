using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;
    float horizontalMove = 0f;
    bool jump = false;

    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    public int attackDamage = 10;

    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 1f; // ����� ���������� ���������� ����� ��������� �����

    public int coinCount = 0;
    public TMPro.TextMeshProUGUI coinText; // ���������� TextMeshPro ��� ����������� ������


    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (currentHealth <= 0)
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInvulnerable)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Player has collided with an enemy!");
                TakeDamage(20);
            }
            if (other.gameObject.CompareTag("Trap"))
            {
                Debug.Log("����������");
                TakeDamage(30);
            }
            
        }

        if (other.gameObject.CompareTag("DeathZone"))
        {
            Debug.Log("Player has entered the death zone!");
            Die(); // ���������� ����� ������
        }

        if (other.gameObject.CompareTag("Coin"))
        {
            Debug.Log("Picked up a coin!");
            coinCount++;
            coinText.text = "Coins: " + coinCount;
            Destroy(other.gameObject);  // ������� ������ ����� �������
        }

        if (other.gameObject.CompareTag("Health"))
        {
            Debug.Log("Picked up a health item!");
            Heal(20);  // ������� ���������� ������������������ ��������
            Destroy(other.gameObject);  // ������� ������, ���� �� �����������
        }

        if (other.gameObject.CompareTag("Finish"))
        {
            SceneManager.LoadScene(2);
        }
    }

    private void Attack()
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
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth > 0)
        {
            StartCoroutine(BecomeInvulnerable());
        }
        else
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetHealth(currentHealth);
    }

    private IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
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

    void FixedUpdate()
    {
        if (currentHealth <= 0)
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
