using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.Assertions;
using System.Runtime.CompilerServices;
public class Player : MonoBehaviour
     {
        public CharacterController2D controller;
        public Animator animator;
        public float runSpeed = 40f;
        float horizontalMove = 0f;
        bool jump = false;

        public int maxHelth = 100;
        public int currentHelth;
        public HealthBar healthBar;

        public Transform attackPoint;
        public float attackRange = 0.5f;
        public LayerMask enemyLayers;

        public int attackDamage = 10;
        void Start()
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

        void OnTriggerEnter2D(Collider2D other)
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
        void Attack()
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
            healthBar.SetHelth(currentHelth);

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
        void FixedUpdate()
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

