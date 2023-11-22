using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FallApart : MonoBehaviour
{
    [SerializeField] float density = 10.0f;
    Collider[] parts;
    Animator animator;
    GameObject model;
    GameObject character;
    public Transform explosionSource = null;
    [SerializeField] float explosionStrength = 10f;
    [SerializeField] float explosionRadius = 10f;
    [SerializeField] float upwardsModifier = 2f;
    [SerializeField] int partsLayer = 0;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        character = animator.gameObject;
        model = character.transform.GetChild(0).gameObject;
        parts = model.GetComponentsInChildren<Collider>();
    }

    [ContextMenu("Activate")]
    public void Activate()
    {
        animator.enabled = false;
        GameObject ragdoll = new("Ragdoll");
        foreach (Collider part in parts)
        {
            DetachAndSetUp(part, ragdoll);
        }
        Destroy(gameObject);
    }
    void DetachAndSetUp(Collider part, GameObject ragdoll)
    {
        Transform parent = part.transform.parent;
        Transform wireframe = null;
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                if (child.name.Contains("Wireframe"))
                {
                    wireframe = child;
                    break;
                }
            }
        }
        part.transform.parent = null;
        if (wireframe != null)
            wireframe.parent = part.transform;
        if (!part.TryGetComponent<Rigidbody>(out var rb))
        {
            rb = part.AddComponent<Rigidbody>();
            rb.useGravity = true;
        }
        part.gameObject.layer = partsLayer;
        part.enabled = true;
        part.isTrigger = false;
        // Approximation of mass with bounding volume
        rb.mass = density * part.bounds.size.x * part.bounds.size.y * part.bounds.size.z;
        rb.SetDensity(density);
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        part.transform.parent = ragdoll.transform;
        rb.AddExplosionForce(explosionStrength, explosionSource != null ? explosionSource.position : transform.position, explosionRadius, upwardsModifier);
    }

    [ContextMenu("Decompose")]
    public void Decompose()
    {
        GameObject dummy = Instantiate(character);
        dummy.transform.SetPositionAndRotation(character.transform.position, character.transform.rotation);
        model.SetActive(false);
        dummy.GetComponent<FallApart>().Activate();
    }
}



public class MissingGameObjectException : UnityException
{
    public MissingGameObjectException(string message) : base(message) { }
}
