using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireKey : InteractableBase
{

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void OnInteract()
    {
        Debug.Log("Fogo apagado!");
        spriteRenderer.enabled = false;
    }
}
