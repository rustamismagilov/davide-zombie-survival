using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Canvas gameOverCanvas;
    [SerializeField] Canvas pauseCanvas;

    [SerializeField] TextMeshProUGUI gameTimeTextbox;
    [SerializeField] TextMeshProUGUI enemiesKilledTextbox;

    FirstPersonController firstPersonController;
    WeaponSwitcher weaponSwitcher;

    float gameTime = 0f;
    int enemiesKilled = 0;

    void Awake()
    {
        firstPersonController = FindFirstObjectByType<FirstPersonController>();
        weaponSwitcher = FindFirstObjectByType<WeaponSwitcher>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverCanvas.enabled = false;
        pauseCanvas.enabled = false;
        PauseGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOverCanvas.enabled && Input.GetKeyDown(KeyCode.Escape)) PauseGame();
        
        DisplayGameTime();
        DisplayEnemiesKilled();
    }
    void OnDestroy()
    {
        EnableWorld();
    }

    private void DisplayGameTime()
    {
        gameTime += Time.deltaTime * Time.timeScale;    // to handle also slowmotion

        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(gameTime);

        int hours = (int)timeSpan.TotalHours; // unlimited hours
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;

        gameTimeTextbox.text = $"Time: {hours:00}:{minutes:00}:{seconds:00}";
    }

    private void DisplayEnemiesKilled()
    {
        enemiesKilledTextbox.text = $"Enemies: {enemiesKilled}";
    }

    public void PauseGame()
    {
        pauseCanvas.enabled = true;
        DisableWorld();
    }

    public void PlayGame()
    {
        pauseCanvas.enabled = false;
        EnableWorld();
    }

    public void ReloadGame()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void GameOver()
    {
        gameOverCanvas.enabled = true;
        DisableWorld();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AddEnemiesKill(int enemies)
    {
        enemiesKilled += enemies;
    }


    void EnableWorld()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (firstPersonController != null) firstPersonController.enabled = true;
        if (weaponSwitcher != null) weaponSwitcher.enabled = true;
        Weapon weapon = FindFirstObjectByType<Weapon>();
        if (weapon != null) weapon.enabled = true;
    }
    void DisableWorld()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (firstPersonController != null) firstPersonController.enabled = false;
        if (weaponSwitcher != null) weaponSwitcher.enabled = false;
        Weapon weapon = FindFirstObjectByType<Weapon>();
        if (weapon != null) weapon.enabled = false;
    }
}
