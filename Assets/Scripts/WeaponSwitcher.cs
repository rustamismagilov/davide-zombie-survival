using System.Collections;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] int currentWeaponIndex = 0;

    int previousWeaponIndex;
    bool isSwitching;

    Pose[] initialRootPoses;


    void Awake()
    {
        InitRootPoses();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisableAllWeapon();
        StartCoroutine(SetWeaponActive(currentWeaponIndex));
    }

    // Update is called once per frame
    void Update()
    {
        if (isSwitching) return;

        previousWeaponIndex = currentWeaponIndex;

        ProcessKeyInput();
        ProcessScrollWheelInput();

        if (previousWeaponIndex != currentWeaponIndex)
        {
            StartCoroutine(SwitchWeaponRoutine(previousWeaponIndex, currentWeaponIndex));
        }
    }

    void InitRootPoses()
    {
        int childCount = transform.childCount;
        initialRootPoses = new Pose[childCount];
        for (int i = 0; i < childCount; i++)
        {
            Transform weapon = transform.GetChild(i);
            initialRootPoses[i] = new Pose
            {
                localPosition = weapon.localPosition,
                localRotation = weapon.localRotation,
                localScale = weapon.localScale
            };
        }
    }

    void RestoreRootPose(int weaponIndex)
    {
        Transform weapon = transform.GetChild(weaponIndex);
        weapon.localPosition = initialRootPoses[weaponIndex].localPosition;
        weapon.localRotation = initialRootPoses[weaponIndex].localRotation;
        weapon.localScale = initialRootPoses[weaponIndex].localScale;
    }

    void DisableAllWeapon()
    {
        foreach (Transform weapon in transform)
        {
            weapon.gameObject.SetActive(false);
        }
    }

    IEnumerator SwitchWeaponRoutine(int prevWeaponIndex, int nextWeaponIndex)
    {
        Weapon prevWeapon = transform.GetChild(prevWeaponIndex)?.GetComponent<Weapon>();
        Weapon nextWeapon = transform.GetChild(nextWeaponIndex)?.GetComponent<Weapon>();

        prevWeapon.canAim = false;
        nextWeapon.canAim = false;
        prevWeapon.canReload = false;
        nextWeapon.canReload = false;
        prevWeapon.canShoot = false;
        nextWeapon.canShoot = false;
        isSwitching = true;

        yield return SetWeaponDeactive(prevWeaponIndex);
        yield return SetWeaponActive(nextWeaponIndex);

        isSwitching = false;
        prevWeapon.canAim = true;
        nextWeapon.canAim = true;
        prevWeapon.canReload = true;
        nextWeapon.canReload = true;
        prevWeapon.canShoot = true;
        nextWeapon.canShoot = true;
    }

    IEnumerator SetWeaponDeactive(int weaponIndex)
    {
        Transform weapon = transform.GetChild(weaponIndex);
        Animator animator = weapon.GetComponent<Animator>();

        int beforeHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

        animator.SetTrigger("putAway");

        // wait the end of the animation before
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).fullPathHash != beforeHash);

        yield return null; // Wait one frame for the Animator to process the trigger
        yield return new WaitWhile(() => animator.IsInTransition(0));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);

        weapon.gameObject.SetActive(false);
    }

    IEnumerator SetWeaponActive(int weaponIndex)
    {
        RestoreRootPose(weaponIndex);

        Transform weapon = transform.GetChild(weaponIndex);
        weapon.gameObject.SetActive(true);
        Animator animator = weapon.GetComponent<Animator>();

        int beforeHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

        animator.Rebind();
        animator.Update(0f);
        animator.SetTrigger("pullOut");

        // wait the end of the animation before
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).fullPathHash != beforeHash);

        yield return null; // Wait one frame for the Animator to process the trigger
        yield return new WaitWhile(() => animator.IsInTransition(0));
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
    }

    void ProcessKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentWeaponIndex = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) currentWeaponIndex = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) currentWeaponIndex = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) currentWeaponIndex = 3;
    }

    void ProcessScrollWheelInput()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (currentWeaponIndex >= transform.childCount - 1)
            {
                currentWeaponIndex = 0;
            }
            else
            {
                currentWeaponIndex++;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (currentWeaponIndex <= 0)
            {
                currentWeaponIndex = transform.childCount - 1;
            }
            else
            {
                currentWeaponIndex--;
            }
        }
    }
}
