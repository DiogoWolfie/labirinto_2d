using UnityEngine;

public class SquareSpawner : MonoBehaviour
{
    public GameObject keyObject; // O prefab da chave
    public Vector2 rectSize = new Vector2(150, 60); // Tamanho do retângulo do labirinto
    public Vector2 rectPosition = new Vector2(0, 0); // Posição central do retângulo do labirinto

    private bool keySpawned = false; // Variável para controlar se a chave já foi gerada

    void Start()
    {
        SpawnKey(); // Gera a chave ao iniciar o jogo
    }

    void SpawnKey()
    {
        if (keySpawned) return; // Se a chave já foi gerada, interrompe a execução

        // Tenta gerar a chave em até 100 tentativas
        for (int attempts = 0; attempts < 100; attempts++)
        {
            // Gera uma posição aleatória dentro do retângulo
            Vector2 randomPosition = new Vector2(
                Random.Range(rectPosition.x - rectSize.x / 2, rectPosition.x + rectSize.x / 2),
                Random.Range(rectPosition.y - rectSize.y / 2, rectPosition.y + rectSize.y / 2)
            );

            // Verifica se a posição está livre de paredes
            if (!IsPositionBlocked(randomPosition))
            {
                // Posiciona a chave na posição gerada
                keyObject.transform.position = new Vector3(randomPosition.x, randomPosition.y, 0f);
                keySpawned = true; // Define que a chave foi gerada
                Debug.Log($"Chave gerada na posição: {randomPosition}");
                return; // Sai do método assim que a chave é gerada
            }
        }

        // Se falhar ao gerar uma chave, exibe uma mensagem
        Debug.Log("Falha ao gerar a chave: Nenhuma posição válida encontrada.");
    }

    bool IsPositionBlocked(Vector2 position)
    {
        // Checa se há colisão com a tag "Wall" na posição desejada
        Collider2D hitCollider = Physics2D.OverlapCircle(position, 0.5f); // Ajuste o raio conforme o tamanho da chave
        return hitCollider != null && hitCollider.CompareTag("Wall");
    }
}
