using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    // Adicionando referência ao joystick
    public FloatingJoystick joystick; // Referência ao Floating Joystick

    public float speed = 5f;
    private Rigidbody2D rb;
    public TextMeshProUGUI countKeysText;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI timerText;
    public Animator animator;
    private Vector2 movement;
    private int countKeys;
    private GameObject door;
    AudioManager3 audioManager;

    private float timeRemaining = 180f; // 3 minutos em segundos
    private bool gameLost = false;

    

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager3>();
    }    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        countKeys = 0;

        door = GameObject.FindWithTag("Door");

        winText.gameObject.SetActive(false);

        UpdateTimerText(); 

        // Salva o �ndice da fase atual no PlayerPrefs
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        PlayerPrefs.Save(); // Garante que o valor seja salvo imediatamente
    }

    void Update()
    {
        if (!gameLost)
        {
            // Obtém input do joystick ou do teclado (fallback)
            movement.x = joystick.Horizontal != 0 ? joystick.Horizontal : Input.GetAxisRaw("Horizontal");
            movement.y = joystick.Vertical != 0 ? joystick.Vertical : Input.GetAxisRaw("Vertical");

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);

            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerText();
            }
            else
            {
                GameOver();
            }
        }
    }

     void FixedUpdate()
    {
        if (!gameLost)
        {
            rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            audioManager.playSFX();
            other.gameObject.SetActive(false);
            countKeys = countKeys + 1;

            SetCountText();
        }

         else if (other.gameObject.CompareTag("Chegada"))
        {
            if (countKeys == 4) 
            {
                WinGame(); 
            }
        }
    }

    void SetCountText() 
   {
       countKeysText.text = countKeys.ToString() + " / 4";

       if (countKeys == 4)
       {
        door.SetActive(false);
       }
   }

    void WinGame()
    {
        SceneManager.LoadSceneAsync(5);
        gameLost = true;
    }

    void GameOver()
    {
        SceneManager.LoadScene(6); // Carrega a cena de game over
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = "Time: " + $"{minutes:00}:{seconds:00}";
    }
}
