using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractionManager : MonoBehaviour
{
    public InteractableBase[] interactables; // Array de objetos interativos
    public Sprite openDoorSprite; // Sprite para quando a porta abrir
    public Sprite openDoorSprite1; // Sprite para quando a porta abrir
    public GameObject door; // Referência ao objeto da porta
    public GameObject door1; // Referência ao objeto da porta
    public AudioClip openDoorSound; // Som de abertura da porta

    private bool conditionMet = false;
    private AudioSource audioSource;
    private SpriteRenderer doorSpriteRenderer;
    private SpriteRenderer door1SpriteRenderer;

    void Start()
    {
        // Inicializa o AudioSource para tocar o som
        audioSource = gameObject.AddComponent<AudioSource>();
        doorSpriteRenderer = door.GetComponent<SpriteRenderer>();
        door1SpriteRenderer = door1.GetComponent<SpriteRenderer>();
    }
    public void CheckInteractions()
    {
        // Verifica se todos os objetos foram ativados
        conditionMet = true;
        foreach (var interactable in interactables)
        {
            if (!interactable.isActivated) // Verifica o estado de cada objeto
            {
                conditionMet = false;
                break;
            }
        }

        // Se todos foram ativados, aciona a condição
        if (conditionMet)
        {
            if (conditionMet)
            {
                OpenDoor();
            }
        }
    }

    private void OpenDoor()
    {
        Debug.Log("Condição atendida! Abrindo a porta...");

        // Altera a sprite da porta para a sprite aberta
        if (openDoorSprite != null && doorSpriteRenderer != null)
        {
            doorSpriteRenderer.sprite = openDoorSprite;
            door1SpriteRenderer.sprite = openDoorSprite1;
        }

        // Toca o som de abertura da porta
        if (openDoorSound != null)
        {
            audioSource.PlayOneShot(openDoorSound);
        }
    }
}
