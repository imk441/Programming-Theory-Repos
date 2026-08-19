using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;

    // ENCAPSULATION
    public float currentHealth {  get; private set; }

    [SerializeField] AudioClip takeDamage;

    [SerializeField] float destroyTime;

    [SerializeField] Animator anim;

    private Collider objectCollider;
    private Rigidbody rb;
    private AudioSource audioSource;
    private PlayerControl playerControl;
    // Start is called before the first frame update
    void Start()
    {
        objectCollider = GetComponent<Collider>();
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        if (CompareTag("Player"))
        {
            playerControl = GetComponent<PlayerControl>();
        }
        else
        {
            playerControl = null;
        }

        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    // ABSTRACTION
    public void TakeDamage(int damage)
    {
        // Take Damage
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damage;
        
        if (audioSource != null)
        {
            audioSource.PlayOneShot(takeDamage);
        }

        // Check for rigidbody
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }

        // Health Check
        if (currentHealth <= 0)
        {
            objectCollider.excludeLayers = LayerMask.GetMask("Bullets", "Enemies");
            anim.SetTrigger("Hit");
            anim.SetBool("isDead", true);
            StartCoroutine(Dying());
        }
        else
        {
            if (playerControl != null)
            {
                playerControl.AddRecoil(playerControl.hitFlinch);
            }
            anim.SetTrigger("Hit");
        }
    }

    IEnumerator Dying()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
    }
}
