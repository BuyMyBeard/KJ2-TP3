using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShockwavePass : MonoBehaviour
{
    [SerializeField] Material material;
    RenderTexture intermediate;

    [ExecuteInEditMode]
    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, intermediate, material);
        Graphics.Blit(intermediate, dst);
    }
}
