using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    Animator animator;
    Collider collider;
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
    }
    bool Openable = false;
    public void CollectKeys()
    {
        promptMessage = "Open door";
        Openable = true;
    }
    public override void Interact()
    {
        if (Openable)
        {

            animator.SetTrigger("Open");
            Openable = false;
            collider.enabled = false;
        }
    }
}
