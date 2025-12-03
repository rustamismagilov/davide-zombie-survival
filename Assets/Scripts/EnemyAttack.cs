using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float damage = 25f; 
    [SerializeField] float attackRange = 1f;

    PlayerHealth target;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        target = FindFirstObjectByType<PlayerHealth>();
    }

    public void AttackHit()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackRange) return;

        target.TakeDamage(damage);
        target.GetComponent<PlayerDamage>().ShowDamageImpact();
    }
}
