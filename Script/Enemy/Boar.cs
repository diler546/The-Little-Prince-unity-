using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Boar : MonoBehaviour
{
    public int maxHelth = 100;
    public int currentHelth;

    public Animator animator;
    public Finish fin;

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

        if(currentHelth <= 0) 
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
