using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    Transform objective;
    NavMeshAgent agent;
    Animator animator;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        objective = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Start()
    {
        animator.SetFloat("MovementX", 0);
        animator.SetFloat("MovmementY", 1);
        StartCoroutine(UpdateDestination());
    }
    IEnumerator UpdateDestination()
    {
        while (true)
        {
            agent.destination = objective.position;
            yield return null;
        }
    }
    private void Update()
    {
        animator.SetBool("IsMoving", agent.velocity != Vector3.zero);
        animator.SetBool("IsFalling", agent.isOnOffMeshLink);
    }

    private void OnTriggerEnter(Collider other)
    {
        FindObjectOfType<GameStateManager>().Lose();
    }
}
