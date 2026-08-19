using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float rotateSpeed;
    [SerializeField] GameObject torso;

    public float hitFlinch;
   
    private Health playerHealth;
    private float verticalInput;
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isGameOver)
        {
            verticalInput = Input.GetAxis("Vertical");

            RotateTorso();
        }

        if (playerHealth.currentHealth <= 0)
        {
            GameManager.Instance.isGameOver = true;
        }
    }

    // ABSTRACTION
    void RotateTorso()
    {
        Vector3 rotation = torso.transform.localEulerAngles;

        float x = rotation.x;

        if (x > 180f)
        {
            x -= 360f;
        }

        x += -rotateSpeed * Time.deltaTime * verticalInput;

        x = Mathf.Clamp(x, -35f, 35f);

        rotation.x = x;

        torso.transform.localEulerAngles = rotation;
    }

    // ABSTRACTION
    public void AddRecoil(float amount)
    {
        torso.transform.Rotate(Vector3.left, amount);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            GameManager.Instance.isGameOver = true;
            Debug.Log("GameOver");
        }
    }
}
