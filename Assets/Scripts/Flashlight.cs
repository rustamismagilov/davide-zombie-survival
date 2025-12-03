using TMPro;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] float lightDecay = 0.4f;
    [SerializeField] float angleDecay = 1f;
    [SerializeField] float minAngle = 40f;
    [SerializeField] float deltaSpotAngle = 5f;
    [SerializeField] TextMeshProUGUI flashlightTextbox;

    Light myLight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        myLight = GetComponent<Light>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myLight.spotAngle = myLight.innerSpotAngle + deltaSpotAngle;    // always add o short angle to make it more realistic
    }

    // Update is called once per frame
    void Update()
    {
        DecreaseLightAngle();
        DecreaseLightIntensity();
        DisplayFlashlight();
    }

    private void DisplayFlashlight()
    {
        int intensityValue = Mathf.RoundToInt(myLight.intensity);
        flashlightTextbox.text = $"Flashlight: {intensityValue:D3}%";
    }

    void DecreaseLightAngle()
    {
        if (myLight.spotAngle > minAngle)
        {
            myLight.spotAngle -= angleDecay * Time.deltaTime;
            myLight.innerSpotAngle = myLight.spotAngle - deltaSpotAngle;
        }
    }

    void DecreaseLightIntensity()
    {
        myLight.intensity -= lightDecay * Time.deltaTime;
    }

    public void IncreaseLightAngle(float restoreAngle)
    {
        myLight.spotAngle += restoreAngle;
        myLight.innerSpotAngle = myLight.innerSpotAngle - deltaSpotAngle;
    }

    public void IncreaseLightIntensity(float restoreIntensity)
    {
        myLight.intensity += restoreIntensity;
    }
}
