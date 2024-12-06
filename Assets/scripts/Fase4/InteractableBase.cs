using UnityEngine;

public abstract class InteractableBase : MonoBehaviour
{
    private bool isPlayerInRange = false;
    public bool isActivated = false;
    public InteractionManager interactionManager;
    public GameObject UI;
    public EnemySpawner enemySpawner;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (UI != null && !isActivated)
            {
                UI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Saiu da área de interação.");
            if (UI != null)
            {
                UI.SetActive(false);
            }
        }
    }

    public void Interact()
    {
        if (!isActivated)
        {
            isActivated = true;
            OnInteract(); // Chama a lógica específica do objeto
            interactionManager.CheckInteractions(); // Notifica o gerenciador
        }
    }

    // Método para ser implementado nas subclasses, definindo a lógica específica de cada objeto
    protected abstract void OnInteract();
}