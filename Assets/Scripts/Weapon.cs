using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;


public class Weapon : MonoBehaviour
{
    [Header("Shoot / Fire")]
    [SerializeField] ParticleSystem muzzleFlash;    // particle for the shooting
    [SerializeField] GameObject bulletImpact;    // target impact effect
    [SerializeField] HitMode hitMode = HitMode.Automatic;
    [SerializeField] FireMode fireMode = FireMode.Single;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 20f;
    [SerializeField] float impulseForce = 10f;

    [Header("Aim / Zoom")]
    [SerializeField] float defaultZoom = 40f;
    [SerializeField] float aimZoom = 30f;
    [SerializeField] Image crossHair;
    [SerializeField] Image crossHairHit;
    [SerializeField] float showHitTime = 0.2f;

    [Header("Ammo")]
    [SerializeField] Ammo ammoSlot;    // ammo slot
    [SerializeField] AmmoType ammoType;    // ammo type it needs
    [SerializeField] int ammoPerShoot = 1;
    [SerializeField] TextMeshProUGUI ammoTextbox;

    CinemachineCamera cinemachineCamera;
    CinemachineImpulseSource cinemachineImpulseSource;    // impulse for recoil
    WeaponMagazine weaponMagazine;    // magazine ammo
    Animator animator;

    [HideInInspector] public bool canAim = true;
    [HideInInspector] public bool canShoot = true;
    [HideInInspector] public bool canReload = true;

    float nextFireTime = 0f;
    bool isAiming = false;
    bool isReloading = false;
    float currentWeight = 0f;
    float aimTransitionSpeed = 5f;

    Coroutine showHitCoroutine;

    void Awake()
    {
        cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        weaponMagazine = GetComponent<WeaponMagazine>();
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crossHairHit.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // aim logig
        if (canAim && Input.GetMouseButton(1))
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }
        UpdateAim();

        // shooting logic
        if (canShoot && !isReloading)
        {
            if (fireMode == FireMode.Single && Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
            if (fireMode == FireMode.Auto && Input.GetButton("Fire1"))
            {
                Shoot();
            }
        }

        // reload logic
        if (canReload && !isReloading && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }

        // ammo
        DisplayAmmo();
    }

    private void OnDisable()
    {
        nextFireTime = 0f;
        isAiming = false;
        isReloading = false;
        currentWeight = 0f;
    }

    void UpdateAim()
    {
        float targetWeight;
        float targetAimZoom;

        if (isAiming)
        {
            targetWeight = 1f;
            crossHair.gameObject.SetActive(false);
            targetAimZoom = aimZoom;
        }
        else
        {
            targetWeight = 0f;
            crossHair.gameObject.SetActive(true);
            targetAimZoom = defaultZoom;
        }

        currentWeight = Mathf.MoveTowards(currentWeight, targetWeight, aimTransitionSpeed * Time.deltaTime);
        cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(cinemachineCamera.Lens.FieldOfView, targetAimZoom, aimTransitionSpeed * Time.deltaTime);

        animator.SetLayerWeight(1, currentWeight);
    }

    void Shoot()
    {
        // check fire rate
        if (Time.time < nextFireTime) return;

        // check ammo
        if (ammoType != AmmoType.None)
        {
            if (weaponMagazine.GetCurrentAmmo() <= 0) return;

            int ammoToUse = ammoPerShoot;
            if (weaponMagazine.GetCurrentAmmo() - ammoToUse < 0) ammoToUse = weaponMagazine.GetCurrentAmmo();
            weaponMagazine.UseAmmo(ammoToUse);
        }

        // animation
        //animator.Play("Gun_shoot", 0, 0f);
        animator.ResetTrigger("shoot");
        animator.SetTrigger("shoot");

        // check hit
        if (hitMode == HitMode.Automatic)
        {
            ShootHit();
        }

        // fire rate
        nextFireTime = Time.time + fireRate;
    }

    void ShootHit()
    {
        ProcessRaycast();
        ProcessRecoil();
        PlayMuzzleFlash();
    }

    IEnumerator Reload()
    {
        if (ammoType == AmmoType.None) yield break;

        int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);
        if (currentAmmo <= 0) yield break;

        int ammoToReload = weaponMagazine.GetCapacity() - weaponMagazine.GetCurrentAmmo();
        if (ammoToReload <= 0) yield break;


        int beforeHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

        animator.ResetTrigger("reload");
        animator.SetTrigger("reload");
        isReloading = true;

        // wait the end of the animation before
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).fullPathHash != beforeHash);

        yield return null; // Wait one frame for the Animator to process the trigger
        yield return new WaitWhile(() => animator.IsInTransition(0));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);


        if (currentAmmo - ammoToReload < 0) ammoToReload = ammoSlot.GetCurrentAmmo(ammoType);
        ammoSlot.ReduceCurrentAmmo(ammoType, ammoToReload);
        weaponMagazine.ReloadAmmo(ammoToReload);

        isReloading = false;
    }

    void ProcessRaycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(cinemachineCamera.transform.position, cinemachineCamera.transform.forward, out hit, range))
        {
            //Debug.Log("Hit this thing: " + hit.transform.name);

            // if i hit something
            CreateHitImpact(hit);

            // if rigidbody 
            if (hit.rigidbody != null)
            {
                Debug.Log("ci");
                Vector3 forceDirection = cinemachineCamera.transform.forward;
                hit.rigidbody.AddForce(forceDirection * impulseForce, ForceMode.Impulse);
            }

            // if has helth
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            if (target != null)
            {
                if (showHitCoroutine != null) StopCoroutine(showHitCoroutine);
                showHitCoroutine = StartCoroutine(ShowHit());
                target.TakeDamage(damage);
            }
        }
    }

    IEnumerator ShowHit()
    {
        crossHairHit.gameObject.SetActive(true);
        yield return new WaitForSeconds(showHitTime);
        crossHairHit.gameObject.SetActive(false);
    }

    void ProcessRecoil()
    {
        if (cinemachineImpulseSource)
        {
            cinemachineImpulseSource.GenerateImpulse();
        }
    }

    void PlayMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
    }

    void CreateHitImpact(RaycastHit hit)
    {
        if (bulletImpact)
        {
            GameObject impact = Instantiate(bulletImpact, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact, 0.1f);
        }
    }

    void DisplayAmmo()
    {
        if (ammoType != AmmoType.None)
        {
            int currentAmmo = ammoSlot.GetCurrentAmmo(ammoType);
            int weaponMagazineAmmo = weaponMagazine.GetCurrentAmmo();
            ammoTextbox.text = $"{weaponMagazineAmmo} | {currentAmmo}";
        }
        else
        {
            ammoTextbox.text = "-";
        }
    }
}
