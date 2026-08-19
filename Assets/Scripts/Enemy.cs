using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // ENCAPSULATION
    protected float m_moveSpeed;

    public float moveSpeed
    {
        get { return m_moveSpeed; }

        set
        {
            if (value < 0.0f)
            {
                Debug.LogWarning("Move Speed cannot be negative!");
            }
            else
            {
                m_moveSpeed = value;
            }
        }

    }

    protected int scoreToIncrease;
    protected int ammoToAdd;

    protected bool isGrounded;
    protected bool rewardGiven;

    protected Weapon randomWeapon;
    protected Rigidbody rb;
    protected Health health;
    protected Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        anim = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // POLYMORPHISM
    protected virtual void Move()
    {
        if (isGrounded && health.currentHealth > 0 && !GameManager.Instance.isGameOver)
        {
            rb.AddForce(Vector3.left * m_moveSpeed, ForceMode.Force);
        }
    }

    // POLYMORPHISM
    protected virtual void GiveReward()
    {
        randomWeapon.currentCarriedAmmo += ammoToAdd;
        GameManager.Instance.score += scoreToIncrease;

        GameManager.Instance.ShowAmmoReward(randomWeapon.name, ammoToAdd);
    }
}
