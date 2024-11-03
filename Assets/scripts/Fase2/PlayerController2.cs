using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    private Vector2 movement;
    public Animator animator;
    public TextMeshProUGUI chaveText;
    public TextMeshProUGUI timerText; // Texto para exibir o tempo restante
    public AudioSource chaveAudio; // Som da chave
    public AudioSource portaAudio; // Som da porta

    public float totalTime = 60f; // Tempo total em segundos
    private float remainingTime;

    private bool hasKey = false;
    private bool isTransitioning = false;

    private float targetX = 75.49525f;

    void Start()
    {
        remainingTime = totalTime; // Inicializa o tempo restante com o tempo total
        UpdateKeyText();
        UpdateTimerText();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        if (hasKey && rb.position.x > targetX && !isTransitioning)
        {
            StartCoroutine(LoadNextScene());
        }

        // Atualiza a contagem regressiva do tempo
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerText();

            if (remainingTime <= 0)
            {
                remainingTime = 0;
                GameOver(); // Chama o game over quando o tempo acaba
            }
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
            chaveAudio.Play();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("porta2") && hasKey && !isTransitioning)
        {
            StartCoroutine(PlayDoorSoundAndLoadScene());
        }
    }

    IEnumerator PlayDoorSoundAndLoadScene()
    {
        isTransitioning = true;
        portaAudio.Play();
        yield return new WaitForSeconds(portaAudio.clip.length);
        SceneManager.LoadSceneAsync(2);
    }

    IEnumerator LoadNextScene()
    {
        isTransitioning = true;
        SceneManager.LoadSceneAsync(2);
        yield return null;
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

    void UpdateTimerText()
    {
        timerText.text = "Tempo: " + Mathf.CeilToInt(remainingTime).ToString();
    }

    void GameOver()
    {
        SceneManager.LoadScene(3); // Carrega a cena de game over
    }
}
