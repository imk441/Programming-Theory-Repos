using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // ENCAPSULATION
    public static GameManager Instance { get; private set; }

    public GameObject[] weapons;

    [SerializeField] List<GameObject> enemyPrefabs;
    [SerializeField] List<KeyCode> inventoryKeys;
    [SerializeField] Animator playerAnim;

    [SerializeField] Transform spawnPoint;
    public bool isGameOver;
    public bool canSwitch;

    [Header("Score Management")]
    public int score;
    [SerializeField] TextMeshProUGUI scoreText;

    [Header("UI Text Display")]
    private Weapon weaponScript;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] TextMeshProUGUI rewardText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI gateHealthText;

    private float startDelay = 2;
    private float repeatRate = 5;

    public int currentWeapon = 0;

    private Health playerHealth;
    private Health gateHealth;
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GameObject.Find("Player").GetComponent<Health>();
        gateHealth = GameObject.FindWithTag("Gates").GetComponent<Health>();
        score = 0;
        isGameOver = false;
        canSwitch = true;

        weaponScript = weapons[currentWeapon].GetComponent<Weapon>();

        playerAnim.SetBool("AR_Equipped", currentWeapon == 0);
        playerAnim.SetBool("Rifle_Equipped", currentWeapon == 1);

        InvokeRepeating("SpawnEnemies", startDelay, repeatRate);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        TextDisplay();

        for (int i = 0; i < inventoryKeys.Count; i++)
        {    
            if (Input.GetKeyDown(inventoryKeys[i]) && canSwitch)
            {
                currentWeapon = i;
                if (!isGameOver)
                {
                    SwitchWeapons();
                    playerAnim.SetBool("AR_Equipped", currentWeapon == 0);
                    playerAnim.SetBool("Rifle_Equipped", currentWeapon == 1);
                }
            }
        }
    }

    // ABSTRACTION
    void SpawnEnemies()
    {
        if (!isGameOver)
        {
            int spawnIndex = Random.Range(0, enemyPrefabs.Count);
            Vector3 spawnPointPos = spawnPoint.position;

            Instantiate(enemyPrefabs[spawnIndex], new Vector3(spawnPointPos.x,spawnPointPos.y, enemyPrefabs[spawnIndex].transform.position.z), enemyPrefabs[spawnIndex].transform.rotation);
        }
    }

    // ABSTRACTION
    void SwitchWeapons()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
        weapons[currentWeapon].SetActive(true);
        weaponScript = weapons[currentWeapon].GetComponent<Weapon>();
    }

    // ABSTRACTION
    void TextDisplay()
    {
        healthText.text = "Health: " + playerHealth.currentHealth;
        gateHealthText.text = "Gate: " + gateHealth.currentHealth;
        ammoText.text = "Ammo: " + weaponScript.currentAmmo + "/" + weaponScript.currentCarriedAmmo;
        scoreText.text = "Score: " + score;
    }

    public void ShowAmmoReward(string weaponName, int amount)
    {
        StopAllCoroutines();
        StartCoroutine(RewardMessage(weaponName, amount));
    }

    IEnumerator RewardMessage(string weaponName, int amount)
    {
        rewardText.gameObject.SetActive(true);
        rewardText.text = $"+{amount} {weaponName} Ammo";

        yield return new WaitForSeconds(2f);

        rewardText.gameObject.SetActive(false);
    }
}
