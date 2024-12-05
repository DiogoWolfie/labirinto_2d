using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.Advertisements; // Para Unity Ads
using System.Diagnostics;

public class PlayerController : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
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

    // Adicionando referência ao joystick
    public FloatingJoystick joystick; // Referência ao Floating Joystick

    // Adicionando Unity Ads
    [SerializeField] private string gameId = "YOUR_GAME_ID"; // Substitua pelo seu Game ID
    [SerializeField] private string interstitialAdUnitId = "Interstitial_Android"; // Seu Placement ID
    [SerializeField] private bool testMode = true;

    private int nextSceneIndex; // Armazena o índice da próxima cena


    void Start()
    {
        remainingTime = totalTime; // Inicializa o tempo restante com o tempo total
        UpdateKeyText();
        UpdateTimerText();

        // Salva o �ndice da fase atual no PlayerPrefs
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        PlayerPrefs.Save(); // Garante que o valor seja salvo imediatamente

        Advertisement.Initialize(gameId, testMode, this); // Inicializa o Unity Ads
    }

    void Update()
    {
        // Obtém input do joystick ou do teclado (fallback)
        movement.x = joystick.Horizontal != 0 ? joystick.Horizontal : Input.GetAxisRaw("Horizontal");
        movement.y = joystick.Vertical != 0 ? joystick.Vertical : Input.GetAxisRaw("Vertical");

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
        ShowAdAndLoadNextScene(4);
    }

    IEnumerator LoadNextScene()
    {
        isTransitioning = true;
        ShowAdAndLoadNextScene(4);
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
        ShowAdAndLoadNextScene(6); // Carrega a cena de game over
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
