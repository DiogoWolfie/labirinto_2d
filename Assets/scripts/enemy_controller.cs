using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_controller : MonoBehaviour
{
    public float moveSpeed = 1f; // Velocidade de movimento do morcego
    public float changeDirectionTime = 2f; // Tempo até mudar de direção

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float timeToChangeDirection;

    public Animator animator;

    // Referência ao jogador
    public GameObject player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timeToChangeDirection = changeDirectionTime;
        ChangeDirection();
    }

    void Update()
    {

        // Mudar direção após certo tempo
        timeToChangeDirection -= Time.deltaTime;
        if (timeToChangeDirection <= 0)
        {
            ChangeDirection();
            timeToChangeDirection = changeDirectionTime;
        }

        animator.SetFloat("Horizontal",moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    void ChangeDirection()
    {
        // Gera uma direção aleatória para o movimento do morcego
        float randomX = UnityEngine.Random.Range(-1f, 1f);
        float randomY = UnityEngine.Random.Range(-1f, 1f);
        moveDirection = new Vector2(randomX, randomY).normalized; // Normaliza para garantir a velocidade constante
    }

    // Detecta colisões com o player ou paredes
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Se o morcego tocar no player, ele desaparece
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            // Se colidir com uma parede, muda de direção
            ChangeDirection();
        }
    }
}
