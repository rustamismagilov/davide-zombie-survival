using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField] TextMeshProUGUI healthTextbox;

    SceneLoader sceneLoader;

    void Awake()
    {
        sceneLoader = FindFirstObjectByType<SceneLoader>();
    }
    // Update is called once per frame
    void Update()
    {
        DisplayHealth();
    }

    private void DisplayHealth()
    {
        healthTextbox.text = $"Health: {health}";
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            sceneLoader.GameOver();
        }
    }

    public void RecoverHealth(float amount)
    {
        health += amount;
    }
}
