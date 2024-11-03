using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;

public class player_controller : MonoBehaviour
{
    public float moveSpeed = 4f;
    public Rigidbody2D rb;
    Vector2 movement;
    public Animator animator;
    public Animator anime_attack;

    // Collider para a área de ataque
    public Collider2D attackCollider;

    // Condições da vela e chave
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

    void Start()
    {
        findCandle = false;
        findKey = false;
        key = 0;
        candle = 0;
        time = 200;
        //attackCollider.enabled = false;  // Collider desativado no início

        // Salva o índice da fase atual no PlayerPrefs
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        PlayerPrefs.Save(); // Garante que o valor seja salvo imediatamente
    }

    // Update is called once per frame
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
            SceneManager.LoadSceneAsync(5);
        }

        // Verificar se o jogador atacou (tecla Enter ou botão esquerdo do mouse)
        //if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) // 0 é o botão esquerdo do mouse
        //{
        //    anime_attack.SetTrigger("attack"); // Ativa o Trigger "Attack" no Animator
        //    attackCollider.enabled = true; // Ativa o Collider durante o ataque
        //    GameObject prefab = Instantiate(audio_ataque, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Quaternion.identity);
       //     Destroy(prefab.gameObject, 1f);
       // }
       // else
       // {
        //    attackCollider.enabled = false; // Desativa o Collider quando o ataque terminar
       // }
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
        // Coleta a vela
        if (other.gameObject.CompareTag("vela"))
        {
            GameObject prefab = Instantiate(audio_coletavel, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Quaternion.identity);
            Destroy(prefab.gameObject, 1f);

            other.gameObject.SetActive(false);
            findCandle = true;
            candle++;
        }

        // Coleta a chave
        if (other.gameObject.CompareTag("chave1"))
        {
            GameObject prefab = Instantiate(audio_coletavel, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Quaternion.identity);
            Destroy(prefab.gameObject, 1f);

            other.gameObject.SetActive(false);
            findKey = true;
            key++;
        }

        // Verifica se a porta pode ser destrancada (condição completa)
        if (other.gameObject.CompareTag("door1") && findKey && findCandle)
        {
            SceneManager.LoadSceneAsync(2); // Carrega a cena nova
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("enemy"))
        {
            //if (attackCollider.enabled) // O inimigo colide com o Collider de ataque
            //{
            //    GameObject prefab = Instantiate(enemy_destroy, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Quaternion.identity);
            //    Destroy(prefab.gameObject, 2f);
            ///    time += 20;


            //    collision.gameObject.SetActive(false);

            // }
            //else
            //{
                // Jogador sofre dano (perde 10 segundos)
                time -= 5;
               // if (time <= 0)
               // {
               //     time = 0;
               //     SceneManager.LoadSceneAsync(5); // Se o tempo acabar, chama a cena de game over
               // }
           // }
        }
    }

    void UpdateDoorInteraction()
    {
        // Se ambas as condições forem satisfeitas, transforma a porta em um Trigger para permitir a interação
        if (findKey && findCandle)
        {
            door1.GetComponent<TilemapCollider2D>().isTrigger = true; // Transforma a porta em um Trigger para permitir a passagem
        }
    }
}

