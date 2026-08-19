using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // ENCAPSULATION
    private float m_moveSpeed = 200;
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


    public int damage;

    private Rigidbody rb;

    private float boundary = 180;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * m_moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > boundary)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponent<Health>();

            health.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
