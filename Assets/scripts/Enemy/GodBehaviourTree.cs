using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;
using UnityEngine.AI;
public class WaitUntilTargetClose : TaskBT
{
    Transform Self { get; }
    Transform Target { get; }
    float DistanceThreshold { get; }
    public WaitUntilTargetClose(Transform self, Transform target, float distanceThreshold)
    {
        Self = self;
        Target = target;
        DistanceThreshold = distanceThreshold;
    }
    public override TaskState Execute()
    {
        return Vector3.Distance(Target.position, Self.position) < DistanceThreshold ? TaskState.Success : TaskState.Running;
    }
}

public class ShockwaveTask : TaskBT
{
    bool hasStartedOnce = false;
    Animator Animator { get; }
    public ShockwaveTask(Animator animator)
    {
        Animator = animator;
    }
    public override TaskState Execute()
    {
        if (!hasStartedOnce)
        {
            hasStartedOnce = true;
            Animator.SetTrigger("Shockwave");
            Animator.SetBool("IsDoingShockwave", true); // apparently OnStateEnter isn't called during SetTrigger
        }

        if (Animator.GetBool("IsDoingShockwave"))
            return TaskState.Running;
        else
        {
            hasStartedOnce = false;
            return TaskState.Success;
        }
    }
}

public class GodBehaviourTree : MonoBehaviour
{
    private Node rootBT;
    [SerializeField] float thresholdDistance;
    [SerializeField] float timeToWaitBeforeGameOver;
    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");

        TaskBT[] tasks0 = new TaskBT[]
        {
            new WaitUntilTargetClose(transform, player.transform, thresholdDistance)
        };
        TaskBT[] tasks1 = new TaskBT[]
        {
            new ShockwaveTask(GetComponent<Animator>())
        };
        TaskBT[] tasks2 = new TaskBT[]
        {
            new Wait(timeToWaitBeforeGameOver, null, delegate
            {
                GameObject.FindObjectOfType<GameStateManager>().Lose("You were slain.........\nYou lasted ");
            })
        };

        TaskNode waitUntilNode = new TaskNode("waitUntil", tasks0);
        TaskNode shockwaveNode = new TaskNode("dothefunnies", tasks1);
        TaskNode waitGameOverNode = new TaskNode("waitGameOver", tasks2);

        rootBT = new Sequence("seq1", new[] { waitUntilNode, shockwaveNode, waitGameOverNode });
    }

    void Update()
    {
        rootBT.Evaluate();
    }
}
