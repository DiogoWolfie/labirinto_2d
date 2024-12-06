using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements; // Para Unity Ads
using System.Diagnostics;

public class DoorOut : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public InteractionManager interactionManager;
    public AudioClip winning; // Som de abertura da porta
    private AudioSource AudioSource;
    // Adicionando Unity Ads
    [SerializeField] private string gameId = "Interstitial_Android"; // Substitua pelo seu Game ID
    [SerializeField] private string interstitialAdUnitId = "Interstitial_Android"; // Seu Placement ID
    [SerializeField] private bool testMode = true;

    private int nextSceneIndex; // Armazena o índice da próxima cena

    private void Start()
    {
        AudioSource = gameObject.AddComponent<AudioSource>();
        Advertisement.Initialize(gameId, testMode, this); // Inicializa o Unity Ads

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && interactionManager.conditionMet)
        {
            AudioSource.PlayOneShot(winning);
            ShowAdAndLoadNextScene(7);
        }
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
