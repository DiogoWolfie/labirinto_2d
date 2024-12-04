using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using TMPro;
using UnityEngine.Advertisements; // Para Unity Ads
using System.Diagnostics;

public class player_controller : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public FloatingJoystick joystick; // Referência ao Floating Joystick

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

    // Adicionando Unity Ads
    [SerializeField] private string gameId = "YOUR_GAME_ID"; // Substitua pelo seu Game ID
    [SerializeField] private string interstitialAdUnitId = "Interstitial_Android"; // Seu Placement ID
    [SerializeField] private bool testMode = true;

    private int nextSceneIndex; // Armazena o índice da próxima cena

    private void Start()
    {
        findCandle = false;
        findKey = false;
        key = 0;
        candle = 0;
        time = 200;

        ShowPopup();

        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        PlayerPrefs.Save();

        Advertisement.Initialize(gameId, testMode, this); // Inicializa o Unity Ads
    }

    void Update()
    {
        movement.x = joystick.Horizontal != 0 ? joystick.Horizontal : Input.GetAxisRaw("Horizontal");
        movement.y = joystick.Vertical != 0 ? joystick.Vertical : Input.GetAxisRaw("Vertical");

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
            ShowAdAndLoadNextScene(6);
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
            

            other.gameObject.SetActive(false);
            findCandle = true;
            candle++;

            GameObject prefab = Instantiate(audio_coletavel, transform.position, Quaternion.identity);
            Destroy(prefab.gameObject, 1f);
        }

        if (other.gameObject.CompareTag("chave1"))
        {
           
           
            //UnityEngine.Debug.Log("funciona porra");
            other.gameObject.SetActive(false);
            findKey = true;
            key++;

            
            GameObject prefab = Instantiate(audio_coletavel, transform.position, Quaternion.identity);
            Destroy(prefab.gameObject, 1f);
        }

        if (other.gameObject.CompareTag("door1") && findKey && findCandle)
        {
            ShowAdAndLoadNextScene(3);
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

    private void ShowAdAndLoadNextScene(int nextSceneIndex)
    {
        this.nextSceneIndex = nextSceneIndex; // Armazena o índice da próxima cena
        Advertisement.Load(interstitialAdUnitId, this); // Carrega o anúncio

        Advertisement.Show(interstitialAdUnitId, this); // Exibe o anúncio
    }

    // Implementação dos métodos de IUnityAdsInitializationListener
    public void OnInitializationComplete()
    {
        UnityEngine.Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        UnityEngine.Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    // Implementação dos métodos de IUnityAdsLoadListener
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        UnityEngine.Debug.Log($"Ad Loaded: {adUnitId}");
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        UnityEngine.Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    // Implementação dos métodos de IUnityAdsShowListener
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId == interstitialAdUnitId && showCompletionState == UnityAdsShowCompletionState.COMPLETED)
        {
            SceneManager.LoadSceneAsync(nextSceneIndex); // Carrega a próxima cena após o anúncio
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        UnityEngine.Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
}
