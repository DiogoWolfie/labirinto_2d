using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AguaChave : InteractableBase
{
    public Sprite newSprite; // Sprite que será exibida quando a porta for ativada
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void OnInteract()
    {
        Debug.Log("Apague o Fogo!");
        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }
}
