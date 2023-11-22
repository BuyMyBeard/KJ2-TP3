using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    [ContextMenu("Shockwave Attack")]
    void Shockwave()
    {
        animator.SetTrigger("Shockwave");
    }
    // Update is called once per frame
    void Update()
    {
    }
}
