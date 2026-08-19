using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Weapon : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject muzzle;

    public Animator playerAnim;
    public bool isBoltAction;
    public ParticleSystem muzzleFlash;

    [Header("Weapon Properties")]
    [SerializeField] private int damage;
    [SerializeField] private float coolDown;
    [SerializeField] private float reloadTime;
    [SerializeField] private float recoil;

    [Header("Ammo Management")]
    [SerializeField] private int maxAmmo;
    public int currentCarriedAmmo;
    public int maxCarriableAmmo;

    // ENCAPSULATION
    public int currentAmmo { get; private set; }

    [Header("Sound Effects")]
    private AudioSource gunAudio;
    [SerializeField] AudioClip shoot;
    [SerializeField] AudioClip reload;
    [SerializeField] AudioClip bolt;

    [Header("Aiming Line")]
    [SerializeField] private float distance;
    private LineRenderer line;

    private bool canShoot;
    private bool isReloading;
    private PlayerControl playerControl;

    // Start is called before the first frame update
    void Start()
    {
        gunAudio = GetComponent<AudioSource>();
        canShoot = true;
        isReloading = false;
        currentAmmo = maxAmmo;
        line = muzzle.gameObject.GetComponent<LineRenderer>();
        line.positionCount = 2;
        currentCarriedAmmo = maxCarriableAmmo / 2;
        playerControl = GameObject.Find("Player").GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            CreateLine();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            line.enabled = false;
        }

        if (Input.GetMouseButton(0) && canShoot && currentAmmo > 0 && !isReloading && !GameManager.Instance.isGameOver)
        {
            Shoot();
            canShoot = false;
            StartCoroutine(coolDownTimer());
        }

        if (!isReloading && currentAmmo < maxAmmo && !GameManager.Instance.isGameOver && currentCarriedAmmo > 0 && (currentAmmo == 0 || Input.GetKeyDown(KeyCode.R)))
        {
            canShoot = false;
            isReloading = true;
            StartCoroutine(Reloading());
        }

        if (currentCarriedAmmo > maxCarriableAmmo)
        {
            currentCarriedAmmo = maxCarriableAmmo;
            Debug.Log("Cannot carry more ammo");
        }
    }

    void Shoot()
    {
        gunAudio.PlayOneShot(shoot, 0.7f);
        playerControl.AddRecoil(recoil);
        
        GameObject bullet = Instantiate(bulletPrefab,muzzle.transform.position, muzzle.transform.rotation);
        muzzleFlash.Play();

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        bulletScript.damage = damage;

        currentAmmo--;
    }

    void CreateLine()
    {
        Vector3 start = muzzle.transform.position;
        Vector3 end = start + muzzle.transform.forward * distance;

        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.enabled = true;
    }

    IEnumerator coolDownTimer()
    {
        GameManager.Instance.canSwitch = false;
        if (isBoltAction)
        {
            playerAnim.SetTrigger("Rifle_Cocking");
            gunAudio.PlayOneShot(bolt, 0.7f);
        }
        yield return new WaitForSeconds(coolDown);
        GameManager.Instance.canSwitch = true;
        canShoot = true;
    }

    IEnumerator Reloading()
    {
        GameManager.Instance.canSwitch = false;
        playerAnim.SetTrigger("Reloading");
        gunAudio.PlayOneShot(reload, 0.7f);
        if (isBoltAction)
        {
            StartCoroutine(PlayBoltAfterReload());
        }

        yield return new WaitForSeconds(reloadTime);

        int bulletsNeeded = maxAmmo - currentAmmo;
        int bulletsToReload = Mathf.Min(bulletsNeeded, currentCarriedAmmo);

        currentAmmo += bulletsToReload;
        currentCarriedAmmo -= bulletsToReload;

        isReloading = false;
        GameManager.Instance.canSwitch = true;

        canShoot = true;
    }

    IEnumerator PlayBoltAfterReload()
    {
        yield return new WaitForSeconds(reload.length);
        gunAudio.PlayOneShot(bolt, 0.7f);
    }
}
