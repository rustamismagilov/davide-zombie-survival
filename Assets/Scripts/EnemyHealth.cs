using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float health = 100f;
    [SerializeField] float destroyAfterDeadTime = 10f;

    SceneLoader sceneLoader;

    bool isDead = false;

    void Awake()
    {
        sceneLoader = FindFirstObjectByType<SceneLoader>();
    }

    public bool IsDead
    {
        get { return isDead; }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        BroadcastMessage(nameof(EnemyController.OnDamageTaken));

        health -= damage;
        if (health <= 0)
        {
            Die();
            sceneLoader.AddEnemiesKill(1);
            Destroy(gameObject, destroyAfterDeadTime);
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        GetComponentInChildren<Animator>().SetTrigger("Dead");
    }
}
