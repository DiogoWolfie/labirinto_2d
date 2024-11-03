using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para carregar cenas

public class Player_Movement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    public Animator anim;
    public int maxGhosts = 1;
    Vector2 movement;
    private int ghostCount = 0; // Contador de fantasmas ao redor do jogador
    // Update is called once per frame
    void Start()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentLevel", levelIndex);
        PlayerPrefs.Save();
    }
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Speed", movement.sqrMagnitude);
    }

    // Método para incrementar a contagem
    public void AddGhost()
    {
        ghostCount++;
        Debug.Log("Fantasmas ao redor: " + ghostCount);
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
        Debug.Log("Game Over! Carregando cena de fim de jogo...");
        SceneManager.LoadScene("GameOver"); // Certifique-se de que a cena "EndGame" está incluída nas cenas do projeto
    }
}
