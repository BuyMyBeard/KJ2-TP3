using System.Collections;
using System.Collections.Generic;
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
        var fallApart = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<FallApart>();
        fallApart.Activate();
    }
}
