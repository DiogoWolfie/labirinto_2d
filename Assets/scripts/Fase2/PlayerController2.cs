using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerControlller : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private Vector2 movement;
    public Animator animator;
    public TextMeshProUGUI chaveText;
    private bool hasKey = false;

    private float targetX = 75.49525f;


    void Start()
    {
        UpdateKeyText();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);


        if (hasKey && rb.position.x > targetX)
        {
            SceneManager.LoadScene(2);
        }
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
            UpdateKeyText();
            Destroy(collision.gameObject);
        }
    }

    void UpdateKeyText()
    {
        if (hasKey)
        {
            chaveText.text = "Chave 1/1";
        }
        else
        {
            chaveText.text = "Chave 0/1";
        }
    }
}
