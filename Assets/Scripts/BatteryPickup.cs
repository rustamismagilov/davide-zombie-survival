using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    [SerializeField] float restoreAngle = 20f;
    [SerializeField] float restoreIntensity = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponentInChildren<Flashlight>().IncreaseLightAngle(restoreAngle);
            other.GetComponentInChildren<Flashlight>().IncreaseLightIntensity(restoreIntensity);
            Destroy(gameObject);
        }
    }
}
