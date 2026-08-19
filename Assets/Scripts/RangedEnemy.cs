using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class RangedEnemy : Enemy
{
    private float rotationSpeed = 1;

    [SerializeField] GameObject torso;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject muzzle;

    [SerializeField] AudioClip shoot;

    [SerializeField] float maxOffset;
    [SerializeField] float minOffset;

    private float coolDown;
    private float stopPosition;

    private Transform player;

    private bool canShoot;
    private bool hasStopped;
    private bool isDead;

    private AudioSource audioSource;

    private Health playerHealth;
    void Awake()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        
        if (playerObj != null)
        {
            player = playerObj.GetComponent<Transform>();
            playerHealth = player.gameObject.GetComponent<Health>();
        }

        audioSource = GetComponent<AudioSource>();
        m_moveSpeed = 13;
        canShoot = false;
        stopPosition = Random.Range(45, 90);
    }

    private void Update()
    {
        DisableConstraints();
    }

    private void LateUpdate()
    {
        if (health.currentHealth > 0)
        {
            if (transform.position.x <= stopPosition)
            {
                Aim();
            }
        }
        else if (!isDead)
        {
            isDead = true;
            randomWeapon = GameManager.Instance.weapons[Random.Range(0, GameManager.Instance.weapons.Length)].GetComponent<Weapon>();
            GiveReward();
        }
    }

    // POLYMORPHISM
    protected override void Move()
    {
        if (transform.position.x > stopPosition)
        {
            if (isGrounded && health.currentHealth > 0 && !GameManager.Instance.isGameOver)
            {
                rb.AddForce(Vector3.left * m_moveSpeed, ForceMode.Force);
            }
        }
        else
        {
            rb.velocity = Vector3.zero;

            if (!hasStopped)
            {
                hasStopped = true;
                StartCoroutine(CoolDownTimer());
            }
        }
    }

    // ABSTRACTION
    void Aim()
    {
        if (player == null)
        {
            return;
        }

        if (!GameManager.Instance.isGameOver && playerHealth.currentHealth > 0)
        {
            anim.SetBool("isAiming", true);

            float t = Mathf.PingPong(Time.time * rotationSpeed, 1f);
            float angle = Mathf.Lerp((player.position.y - minOffset), (player.position.y + maxOffset), t);

            torso.transform.localRotation = Quaternion.Euler(0, 0, angle);

            if (canShoot)
            {
                Shoot();
                canShoot = false;
                StartCoroutine(CoolDownTimer());
            }
        }
        
    }

    void Shoot()
    {
        audioSource.PlayOneShot(shoot, 1.0f);
        Instantiate(bulletPrefab, muzzle.transform.position, muzzle.transform.rotation);
    }

    private void DisableConstraints()
    {
        if (isDead)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
        else
        {
            return;
        }
    }

    IEnumerator CoolDownTimer()
    {
        coolDown = Random.Range(1f, 2f);
        yield return new WaitForSeconds(coolDown);
        canShoot = true;
    }


    // POLYMORPHISM
    protected override void GiveReward()
    {
        if (!rewardGiven)
        {
            scoreToIncrease = 2;
            if (randomWeapon.isBoltAction)
            {
                ammoToAdd = Random.Range(5, 10);
                Debug.Log("Secondary ammo awarded");
            }
            else
            {
                ammoToAdd = Random.Range(30, 60);
                Debug.Log("Primary ammo awarded");
            }
            base.GiveReward();
            rewardGiven = true;
        }
    }
}
