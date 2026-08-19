using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class BaseEnemy : Enemy
{
    public float attackCooldown;
    public int damage;

    private GameObject target;
    private bool isOnCooldown;
    private Health gateHealth;
    private Health playerHealth;

    private bool isDead;
    private void Awake()
    {
        m_moveSpeed = 15;
        target = GameObject.FindWithTag("Target");
        playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();

        GameObject gate = GameObject.FindWithTag("Gates");

        if (gate != null)
        {
            gateHealth = gate.GetComponent<Health>();
        }
    }
    private void Update()
    {
        if (playerHealth != null && playerHealth.currentHealth > 0)
        {
            Attack();
            if (health.currentHealth <= 0 && !isDead)
            {
                isDead = true;
                randomWeapon = GameManager.Instance.weapons[Random.Range(0, GameManager.Instance.weapons.Length)].GetComponent<Weapon>();
                GiveReward();
            }
        }
    }
    void Attack()
    {
        float distanceToTarget = transform.position.x - target.transform.position.x;

        if (gateHealth != null)
        {
            float distanceToGate = transform.position.x - gateHealth.transform.position.x;

            if (distanceToGate <= 15 && gateHealth.currentHealth > 0 && !isOnCooldown)
            {
                anim.SetBool("gateReached", true);
                StartCoroutine(AttackCooldown());
            }
        }

        if (distanceToTarget <= 10)
        {
            GameManager.Instance.isGameOver = true;
        }
    }

    IEnumerator AttackCooldown()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(attackCooldown);

        if (gateHealth != null && gateHealth.currentHealth > 0)
        {
            anim.SetTrigger("Attack");
        }
        else
        {
            anim.SetBool("gateReached", false);
        }

        isOnCooldown = false;
    }

    public void DealGateDamage()
    {
        if (gateHealth != null && gateHealth.currentHealth > 0)
        {
            gateHealth.TakeDamage(damage);
        }
        else
        {
            return;
        }
    }

    // POLYMORPHISM
    protected override void GiveReward()
    {
        if (!rewardGiven)
        {
            scoreToIncrease = 1;
            if (randomWeapon.isBoltAction)
            {
                ammoToAdd = Random.Range(4, 8);
                Debug.Log("Secondary ammo awarded");
            }
            else
            {
                ammoToAdd = Random.Range(10, 30);
                Debug.Log("Primary ammo awarded");
            }
            base.GiveReward();
            rewardGiven = true;
        }
    }
}
