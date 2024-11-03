using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;
using System.Diagnostics;

public class player_controller : MonoBehaviour
{
    public float moveSpeed = 4f;
    public Rigidbody2D rb;
    Vector2 movement;
    public Animator animator;
    public Animator anime_attack;

    public Collider2D attackCollider;

    private bool findCandle;
    private bool findKey;

    public GameObject door1;

    public TextMeshProUGUI countTime;
    private float countdown = 1f;

    private int key;
    private int candle;
    private int time;

    public TextMeshProUGUI getKey;
    public TextMeshProUGUI getCandle;

    public GameObject audio_coletavel;
    public GameObject enemy_destroy;
    public GameObject audio_ataque;

    public GameObject popup;

    private bool isPopupActive = false;
    private float popupTimer = 4f; // Define o tempo para o popup desaparecer

    void Start()
    {
        findCandle = false;
        findKey = false;
        key = 0;
        candle = 0;
        time = 200;

        ShowPopup();
        //popup.SetActive(false); 

        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        PlayerPrefs.Save();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        UpdateDoorInteraction();

        countdown -= Time.deltaTime;

        if (countdown <= 0f)
        {
            time -= 1;
            SetCountText();
            countdown = 1f;
        }

        if (time <= 0)
        {
            SceneManager.LoadSceneAsync(6);
        }

        if (isPopupActive)
        {
            popupTimer -= Time.deltaTime;
            if (popupTimer <= 0f)
            {
                popup.SetActive(false); // Oculta o popup após o tempo
                isPopupActive = false;  // Reseta a condição
                
            }
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    void SetCountText()
    {
        countTime.text = "Time: " + time.ToString() + " s";
        getKey.text = "Key: " + key.ToString() + " /1";
        getCandle.text = "Candle: " + candle.ToString() + " /1";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("vela"))
        {
            GameObject prefab = Instantiate(audio_coletavel, transform.position, Quaternion.identity);
            Destroy(prefab.gameObject, 1f);

            other.gameObject.SetActive(false);
            findCandle = true;
            candle++;
        }

        if (other.gameObject.CompareTag("chave1"))
        {
            GameObject prefab = Instantiate(audio_coletavel, transform.position, Quaternion.identity);
            Destroy(prefab.gameObject, 1f);

            other.gameObject.SetActive(false);
            findKey = true;
            key++;
        }

        if (other.gameObject.CompareTag("door1") && findKey && findCandle)
        { 
            SceneManager.LoadSceneAsync(3);
        }
    }

    private void ShowPopup()
    {
        popup.SetActive(true);  // Mostra o popup
        isPopupActive = true;   // Define a flag como ativa
        popupTimer = 4f;        // Reinicia o temporizador para 4 segundos
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            time -= 5;
        }
    }

    void UpdateDoorInteraction()
    {
        if (findKey && findCandle)
        {
            door1.GetComponent<TilemapCollider2D>().isTrigger = true;
        }
    }
}
