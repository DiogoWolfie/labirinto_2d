using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements; // Para Unity Ads
using System.Diagnostics;

public class PlayerMovement : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
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

    // Adicionando Unity Ads
    [SerializeField] private string gameId = "YOUR_GAME_ID"; // Substitua pelo seu Game ID
    [SerializeField] private string interstitialAdUnitId = "Interstitial_Android"; // Seu Placement ID
    [SerializeField] private bool testMode = true;

    private int nextSceneIndex; // Armazena o índice da próxima cena



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

        Advertisement.Initialize(gameId, testMode, this); // Inicializa o Unity Ads
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
        ShowAdAndLoadNextScene(5);
        gameLost = true;
    }

    void GameOver()
    {
        ShowAdAndLoadNextScene(6); // Carrega a cena de game over
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = "Time: " + $"{minutes:00}:{seconds:00}";
    }

    private void ShowAdAndLoadNextScene(int sceneIndex)
    {
        nextSceneIndex = sceneIndex; // Salva a próxima cena no índice
        Advertisement.Load(interstitialAdUnitId, this); // Carrega o anúncio
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
        if (adUnitId == interstitialAdUnitId)
        {
            UnityEngine.Debug.Log("Ad Loaded. Showing ad.");
            Advertisement.Show(adUnitId, this);
        }
    }

    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        UnityEngine.Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    // Implementação dos métodos de IUnityAdsShowListener
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId == interstitialAdUnitId)
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
            {
                UnityEngine.Debug.Log($"Ad completed. Loading next scene: {nextSceneIndex}");
                SceneManager.LoadSceneAsync(nextSceneIndex); // Carrega a cena correta
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Ad not completed. Skipping to next scene: {nextSceneIndex}");
                SceneManager.LoadSceneAsync(nextSceneIndex); // Ainda carrega a cena
            }
        }
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        UnityEngine.Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
}
