using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    public TextMeshProUGUI countKeysText;
    public TextMeshProUGUI winText;
    public Animator animator;
    private Vector2 movement;
    private int countKeys;
    private GameObject door;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        countKeys = 0;

        door = GameObject.FindWithTag("Door");

        winText.gameObject.SetActive(false);

        // Salva o índice da fase atual no PlayerPrefs
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        PlayerPrefs.Save(); // Garante que o valor seja salvo imediatamente
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
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
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
        winText.gameObject.SetActive(true);
    }
}
