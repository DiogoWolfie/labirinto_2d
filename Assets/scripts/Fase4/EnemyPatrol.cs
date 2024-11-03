using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    private Vector2 direction;
    public Animator anim;

    public float orbitSpeed = 50f; // Velocidade de rotação ao redor do jogador
    public float orbitDistance = 1.5f; // Distância do inimigo ao jogador
    private Transform player;
    private bool isOrbiting = false;

    void Start()
    {
        // Define uma direção inicial aleatória para a patrulha
        direction = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    void Update()
    {
        if (isOrbiting && player != null)
        {
            // Quando em órbita, chama a função de órbita
            OrbitAroundPlayer();
        }
        else
        {
            // Quando patrulhando, move o inimigo na direção definida e atualiza a animação
            Patrol();
        }
    }

    private void Patrol()
    {
        transform.Translate(direction * speed * Time.deltaTime);
        anim.SetFloat("Horizontal", direction.x);
    }

    private void OrbitAroundPlayer()
    {
        // Calcula o vetor de direção entre o inimigo e o jogador
        Vector3 directionToPlayer = (transform.position - player.position).normalized;
        // Define a posição de órbita com a distância constante
        Vector3 orbitPosition = player.position + directionToPlayer * orbitDistance;

        // Move o inimigo para a posição de órbita
        transform.position = Vector3.MoveTowards(transform.position, orbitPosition, Time.deltaTime * speed);

        // Rotaciona ao redor do jogador sem mudar o eixo Z do próprio inimigo
        Vector3 offset = transform.position - player.position;
        offset = Quaternion.Euler(0, 0, orbitSpeed * Time.deltaTime) * offset;
        transform.position = player.position + offset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Inverte a direção ao colidir com uma borda durante a patrulha
        if (!isOrbiting)
        {
            if (collision.CompareTag("BordaEsquerda") || collision.CompareTag("BordaDireita"))
            {
                direction.x = -direction.x; // Inverte apenas o eixo X
            }
            else if (collision.CompareTag("BordaSuperior") || collision.CompareTag("BordaInferior"))
            {
                direction.y = -direction.y; // Inverte apenas o eixo Y
            }
        }

        // Inicia a órbita ao colidir com o jogador
        if (collision.CompareTag("Player") && !isOrbiting)
        {
            player = collision.transform;
            isOrbiting = true; // Ativa a órbita e desativa a patrulha
            anim.SetFloat("Horizontal", 1f);
            speed = 3f;

            // Incrementa a contagem de fantasmas no controller do jogador
            Player_Movement playerController = player.GetComponent<Player_Movement>();
            if (playerController != null)
            {
                playerController.AddGhost();
            }
        }
    }
}
