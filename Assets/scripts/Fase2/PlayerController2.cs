using UnityEngine;
using TMPro; // Certifique-se de usar isso para acessar o TextMeshProUGUI
using UnityEngine.SceneManagement;

public class PlayerControlller : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private Vector2 movement;
    public Animator animator;
    public TextMeshProUGUI chaveText; // Referência ao texto do UI
    private bool hasKey = false; // Variável para verificar se o player pegou a chave

    void Start()
    {
        UpdateKeyText(); // Atualiza o texto no início do jogo
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("chaves") && !hasKey)
        {
            hasKey = true;
            UpdateKeyText(); // Atualiza o texto para 1/1
            Destroy(collision.gameObject); // Destroi a chave após a coleta
        }
    }

    void UpdateKeyText()
    {
        if (hasKey)
        {
            chaveText.text = "Chave 1/1";
            SceneManager.LoadSceneAsync(2); //temporário
        }
        else
        {
            chaveText.text = "Chave 0/1";
        }
    }
}
