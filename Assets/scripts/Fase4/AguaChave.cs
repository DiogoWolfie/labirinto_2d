using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AguaChave : InteractableBase
{
    public Sprite newSprite; // Sprite que será exibida quando a porta for ativada
    private SpriteRenderer spriteRenderer;
    public AudioClip getWater; // Som de abertura da porta
    private AudioSource audioSource;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    protected override void OnInteract()
    {
        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
            audioSource.PlayOneShot(getWater);
        }
    }
}
