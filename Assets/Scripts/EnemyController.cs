using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float chaseRange = 10f;
    [SerializeField] float turnSpeed = 5f;

    [SerializeField] float roamRadius = 10f;
    [SerializeField] float waitTime = 2f;

    Transform target;

    NavMeshAgent navMeshAgent;
    Animator animator;
    EnemyHealth enemyHealth;
    EnemyAttack enemyAttack;

    float distanceToTarget = Mathf.Infinity;
    bool isProvoked = false;

    float waitTimer;

    // Start is called before the first frame update
    void Start()
    {
        target = FindFirstObjectByType<PlayerHealth>().transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        // check dead
        if (enemyHealth.IsDead)
        {
            enabled = false;
            enemyAttack.enabled = false;
            navMeshAgent.enabled = false;
            return;
        }

        // check distance
        distanceToTarget = Vector3.Distance(target.position, transform.position);
        if (!isProvoked && distanceToTarget <= chaseRange)
        {
            isProvoked = true;
        }

        if (isProvoked)
        {
            EngageTarget();
        }
        else
        {
            MoveRandomly();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    public void OnDamageTaken()
    {
        isProvoked = true;

        // animate hit just from walking
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) 
        {
            animator.SetTrigger("Hit");
        }
    }

    void MoveRandomly()
    {
        CheckVelocity();

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTime)
            {
                MoveToRandomPoint();

                waitTimer = 0f;
            }
        }
    }

    void MoveToRandomPoint()
    {
        Vector3 randomPos = Random.insideUnitSphere * roamRadius + transform.position;

        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
    }

    void EngageTarget()
    {
        CheckVelocity();

        if (distanceToTarget >= navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }

        if (distanceToTarget <= navMeshAgent.stoppingDistance)
        {
            FaceTarget();
            AttackTarget();
        }
    }

    void ChaseTarget()
    {
        if (navMeshAgent && navMeshAgent.enabled && target)
            navMeshAgent.SetDestination(target.position);
    }

    void AttackTarget()
    {
        animator.SetTrigger("Attack");
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
    }

    void CheckVelocity()
    {
        Vector3 averageSpeed = new Vector3(navMeshAgent.velocity.x, 0, navMeshAgent.velocity.z);
        float speed = averageSpeed.magnitude;

        animator.SetFloat("MoveSpeed", speed);
    }
}

