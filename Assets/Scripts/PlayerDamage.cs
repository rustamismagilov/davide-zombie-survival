using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] List<GameObject> bloodSplatterImages;
    [SerializeField] float effectDuration = 0.3f;

    CinemachineImpulseSource cinemachineImpulseSource;    // impulse for hit

    void Awake()
    {
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();

        ResetHit();
        DisableImages();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisableImages()
    {
        foreach (GameObject image in bloodSplatterImages)
        {
            image.SetActive(false);
        }
    }

    void ResetHit()
    {
        if (cinemachineImpulseSource)
        {
            CinemachineImpulseManager.Instance.Clear();
        }
    }

    public void ShowDamageImpact()
    {
        ProcessHit();
        StartCoroutine(ShowBloodSplatters());
    }

    void ProcessHit()
    {
        if (cinemachineImpulseSource)
        {
            cinemachineImpulseSource.GenerateImpulse();
        }
    }

    IEnumerator ShowBloodSplatters()
    {
        int randomIndex = Random.Range(0, bloodSplatterImages.Count);
        GameObject selectedImage = bloodSplatterImages[randomIndex];

        selectedImage.SetActive(true);
        yield return new WaitForSeconds(effectDuration);
        selectedImage.SetActive(false);
    }
}
