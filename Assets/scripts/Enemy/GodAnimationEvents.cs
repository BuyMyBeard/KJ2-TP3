using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GodAnimationEvents : MonoBehaviour
{
    public void OnShouldPlayDrop()
    {
        GetComponent<AudioSource>().Play();
    }
    public void OnAttackHit()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var fallApart = player.GetComponentInParent<FallApart>();
        fallApart.Decompose();
        player.GetComponentInParent<PlayerMovement>().movementFrozen = true;
        GetComponent<Animator>().SetBool("IsDoingShockwave", false);
        GameObject.FindObjectOfType<Timer>().IsRunning = false;
    }
}
