using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas
using UnityEngine.Advertisements; // Para Unity Ads
using System.Diagnostics;

public class Player_Movement : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public Animator anim;
    public int maxGhosts = 1;
    Vector2 movement;
    private int ghostCount = 0; // Contador de fantasmas ao redor do jogador
    public GameObject cutsceneCanvas;
    // Adicionando referência ao joystick
    public FloatingJoystick joystick; // Referência ao Floating Joystick

    // Adicionando Unity Ads
    [SerializeField] private string gameId = "YOUR_GAME_ID"; // Substitua pelo seu Game ID
    [SerializeField] private string interstitialAdUnitId = "Interstitial_Android"; // Seu Placement ID
    [SerializeField] private bool testMode = true;

    private int nextSceneIndex; // Armazena o índice da próxima cena

    // Update is called once per frame
    void Start()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        PlayerPrefs.Save();
        Time.timeScale = 0f; // Pausa o jogo
        cutsceneCanvas.SetActive(true); // Ativa o Canvas

        Advertisement.Initialize(gameId, testMode, this); // Inicializa o Unity Ads

    }
    void Update()
    {
        // Obtém input do joystick ou do teclado (fallback)
        movement.x = joystick.Horizontal != 0 ? joystick.Horizontal : Input.GetAxisRaw("Horizontal");
        movement.y = joystick.Vertical != 0 ? joystick.Vertical : Input.GetAxisRaw("Vertical");

        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", movement.sqrMagnitude);
    }

    // Método para incrementar a contagem
    public void AddGhost()
    {
        ghostCount++;
        UnityEngine.Debug.Log("Fantasmas ao redor: " + ghostCount);
        if (ghostCount > maxGhosts)
        {
            EndGame();
        }
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // Método para obter a contagem atual (opcional, útil para outros scripts)
    public int GetGhostCount()
    {
        return ghostCount;
    }
    private void EndGame()
    {
        UnityEngine.Debug.Log("Game Over! Carregando cena de fim de jogo...");
        ShowAdAndLoadNextScene(6); // Certifique-se de que a cena "EndGame" está incluída nas cenas do projeto
    }
    public void EndCutscene()
    {
        cutsceneCanvas.SetActive(false); // Esconde o Canvas
        Time.timeScale = 1f; // Retoma o jogo
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
