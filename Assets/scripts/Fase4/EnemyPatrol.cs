using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    private Vector2 direction;
    public Animator anim;


    void Start()
    {
        // Define uma direção inicial aleatória
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    void Update()
    {
        // Move o inimigo na direção definida
        transform.Translate(direction * speed * Time.deltaTime);
        anim.SetFloat("Horizontal", direction.x);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Inverte a direção ao colidir com uma borda
        if (collision.CompareTag("BordaEsquerda") || collision.CompareTag("BordaDireita"))
        {
            direction.x = -direction.x; // Inverte apenas o eixo X
        }
        else if (collision.CompareTag("BordaSuperior") || collision.CompareTag("BordaInferior"))
        {
            direction.y = -direction.y; // Inverte apenas o eixo Y
        }
    }
}
